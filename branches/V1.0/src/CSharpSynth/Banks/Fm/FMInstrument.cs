using System;
using System.Collections.Generic;
using CSharpSynth.Synthesis;
using System.IO;

namespace CSharpSynth.Banks.Fm
{
    public class FMInstrument : Instrument
    {
        //--Variables
        private SynthHelper.WaveFormType baseWaveType;
        private SynthHelper.WaveFormType modWaveType;
        //These parameters are measured in samples...
        private int _attack;
        private int _release;
        private int _decay;
        private int _hold;
        private int _delay;
        private double start_time;
        private double end_time;
        private bool looping;
        private Envelope env;
        //modulator parameters
        private ModulatorAmplitudeFunction mamp;
        private ModulatorFrequencyFunction mfreq;
        //--Public Properties
        public int Attack
        {
            get { return _attack; }
            set { _attack = value; }
        }
        public int Release
        {
            get { return _release; }
            set { _release = value; }
        }
        public SynthHelper.WaveFormType WaveForm
        {
            get { return baseWaveType; }
            set { baseWaveType = value; }
        }
        //--Public Methods
        public FMInstrument(string fmProgramFile, int sampleRate)
            : base()
        {
            this.SampleRate = sampleRate;
            //Proper calculation of voice states
            _attack = SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_ATTACK);
            _release = SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_RELEASE);
            _decay = SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_DECAY);
            _hold = SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_HOLD);
            _delay = SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_DELAY);
            //open fm program file
            loadProgramFile(fmProgramFile);
            //set base attribute name
            base.Name = Path.GetFileNameWithoutExtension(fmProgramFile);
        }        
        public override bool allSamplesSupportDualChannel()
        {
            return false;
        }
        public override void enforceSampleRate(int sampleRate)
        {
            if (sampleRate != this.SampleRate)
            {
                //Proper calculation of voice states
                _attack = SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_ATTACK);
                _release = SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_RELEASE);
                _decay = SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_DECAY);
                _hold = SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_HOLD);
                _delay = SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_DELAY);
                this.SampleRate = sampleRate;
            }
        }
        public override int getAttack(int note)
        {
            return _attack;
        }
        public override int getRelease(int note)
        {
            return _release;
        }
        public override int getDecay(int note)
        {
            return _decay;
        }
        public override int getHold(int note)
        {
            return _hold;
        }
        public override int getDelay(int note)
        {
            return _delay;
        }
        public override float getSampleAtTime(int note, int channel, int synthSampleRate, ref double time)
        {
            //time
            if (time >= end_time)
            {
                if (looping)
                    time = time % start_time;
                else
                {
                    time = end_time;
                    return 0.0f;
                }
            }

            double freq = SynthHelper.NoteToFrequency(note);

            double timeM = time;
            double timeC = time;
            double delta = (1.0 / freq); //Position in wave form in 2PI * (time* frequency)
            timeM = time % delta;

            double c1 = mfreq.inputSelect == 0 ? freq : SynthHelper.DEFAULT_AMPLITUDE;
            double c2 = mfreq.inputSelect == 1 ? SynthHelper.DEFAULT_AMPLITUDE : freq; 

            //modulation
            switch (modWaveType)
            {
                case SynthHelper.WaveFormType.Sine:
                    freq += (SynthHelper.Sine(mfreq.doProcess(c1), timeC) * mamp.doProcess(c2));
                    break;
                case SynthHelper.WaveFormType.Sawtooth:
                    freq += (SynthHelper.Sawtooth(mfreq.doProcess(c1), timeC) * mamp.doProcess(c2));
                    break;
                case SynthHelper.WaveFormType.Square:
                    freq += (SynthHelper.Square(mfreq.doProcess(c1), timeC) * mamp.doProcess(c2));
                    break;
                case SynthHelper.WaveFormType.Triangle:
                    freq += (SynthHelper.Triangle(mfreq.doProcess(c1), timeC) * mamp.doProcess(c2));
                    break;
                case SynthHelper.WaveFormType.WhiteNoise:
                    freq += (SynthHelper.WhiteNoise(0, timeC) * mamp.doProcess(c2));
                    break;
                default:
                    break;
            }

            delta = (1.0 / freq);      //Position in wave form in 2PI * (time* frequency)
            timeC = time % delta;

            //carrier
            switch (baseWaveType)
            {
                case SynthHelper.WaveFormType.Sine:
                    return SynthHelper.Sine(freq, timeM) * env.doProcess(time);
                case SynthHelper.WaveFormType.Sawtooth:
                    return SynthHelper.Sawtooth(freq, timeM) * env.doProcess(time);
                case SynthHelper.WaveFormType.Square:
                    return SynthHelper.Square(freq, timeM) * env.doProcess(time);
                case SynthHelper.WaveFormType.Triangle:
                    return SynthHelper.Triangle(freq, timeM) * env.doProcess(time);
                case SynthHelper.WaveFormType.WhiteNoise:
                    return SynthHelper.WhiteNoise(note, timeM) * env.doProcess(time);
                default:
                    return 0.0f;
            }
        }
        private void loadProgramFile(string file)
        {
            StreamReader reader = new StreamReader(PlatformHelper.StreamLoad(file));
            if (!reader.ReadLine().Trim().ToUpper().Equals("[FM INSTRUMENT]"))
            {
                reader.Close();
                throw new Exception("Invalid Program file: Incorrect Header!");
            }
            string[] args = reader.ReadLine().Split(new string[] { "|" }, StringSplitOptions.None);
            if (args.Length < 4)
            {
                reader.Close();
                throw new Exception("Invalid Program file: Parameters are missing");
            }
            this.baseWaveType = SynthHelper.getTypeFromString(args[0]);
            this.modWaveType = SynthHelper.getTypeFromString(args[1]);
            this.mfreq = (ModulatorFrequencyFunction)getOpsAndValues(args[2], true);
            this.mamp = (ModulatorAmplitudeFunction)getOpsAndValues(args[3], false);
            args = reader.ReadLine().Split(new string[] { "|" }, StringSplitOptions.None);
            if (args.Length < 3)
            {
                reader.Close();
                throw new Exception("Invalid Program file: Parameters are missing");
            }
            if (int.Parse(args[0]) == 0)
                looping = true;
            start_time = double.Parse(args[1]);
            end_time = double.Parse(args[2]);
            args = reader.ReadLine().Split(new string[] { "|" }, StringSplitOptions.None);
            if (args.Length < 3)
            {
                reader.Close();
                throw new Exception("Invalid Program file: Parameters are missing");
            }
            switch (args[0].ToLower().Trim())
            {
                case "fadein":
                    env = Envelope.CreateBasicFadeIn(double.Parse(args[2]));
                    break;
                case "fadeout":
                    env = Envelope.CreateBasicFadeOut(double.Parse(args[2]));
                    break;
                case "fadein&out":
                    double p = double.Parse(args[2]) / 2.0;
                    env = Envelope.CreateBasicFadeInAndOut(p, p);
                    break;
                default:
                    env = Envelope.CreateBasicConstant();
                    break;
            }
            env.Peak = double.Parse(args[1]);
            reader.Close();
        }
        private IFMComponent getOpsAndValues(string arg, bool isFrequencyFunction)
        {
            arg = arg + "    ";
            char[] chars = arg.ToCharArray();
            List<byte> opList = new List<byte>();
            List<double> valueList = new List<double>();
            string start = arg.Substring(0, 4).ToLower();
            byte select = 0;
            if (!start.Contains("freq") && !start.Contains("amp"))
            {//if "freq" isnt used then we make sure the value passed in is negated by *0;
                opList.Add(0);
                valueList.Add(0);
            }
            else if (start.Contains("freq"))
            {
                select = 0;
            }
            else
            {
                select = 1;
            }
            bool opOcurred = false;
            bool neg = false;
            for (int x = 0; x < arg.Length; x++)
            {
                switch (chars[x])
                {
                    case '*':
                        if (opOcurred == false)
                        {
                            opList.Add(0);
                            opOcurred = true;
                        }
                        break;
                    case '/':
                        if (opOcurred == false)
                        {
                            opList.Add(1);
                            opOcurred = true;
                        }
                        break;
                    case '+':
                        if (opOcurred == false)
                        {
                            opList.Add(2);
                            opOcurred = true;
                        }
                        break;
                    case '-':
                        if (opOcurred == true)
                            neg = !neg;
                        else
                        {
                            opList.Add(3);
                            opOcurred = true;
                        }
                        break;
                    default:
                        string number = "";
                        while (Char.IsDigit(chars[x]) || chars[x] == '.')
                        {
                            number = number + chars[x];
                            x++;
                            if (x >= chars.Length)
                                break;
                        }
                        if (number.Length > 0)
                        {
                            x--;
                            opOcurred = false;
                            if (neg)
                                number = "-" + number;
                            neg = false;
                            valueList.Add(double.Parse(number));
                        }
                        break;
                }
            }
            while (opList.Count < valueList.Count)
                opList.Add(2);
            if (isFrequencyFunction)
            {
                ModulatorFrequencyFunction ifm = new ModulatorFrequencyFunction(opList.ToArray(), valueList.ToArray());
                ifm.inputSelect = select;
                return ifm;
            }
            else
            {
                ModulatorAmplitudeFunction ifm = new ModulatorAmplitudeFunction(opList.ToArray(), valueList.ToArray());
                ifm.inputSelect = select;
                return ifm;
            }
        }
        //--Private Classes
        private class ModulatorFrequencyFunction : IFMComponent
        {
            public byte inputSelect = 0;
            private byte[] ops; //0 = "*", 1 = "/", 2 = "+", 3 = "-"
            private double[] values;
            public ModulatorFrequencyFunction(byte[] ops, double[] values)
            {
                if (ops.Length != values.Length)
                    throw new Exception("Invalid FM Frequency function.");
                this.ops = ops;
                this.values = values;
            }
            public double doProcess(double value)
            {
                for (int x = 0; x < ops.Length; x++)
                {
                    switch (ops[x])
                    {
                        case 0:
                            value = value * values[x];
                            break;
                        case 1:
                            value = value / values[x];
                            break;
                        case 2:
                            value = value + values[x];
                            break;
                        case 3:
                            value = value - values[x];
                            break;
                    }
                }
                return value;
            }
        }
        private class ModulatorAmplitudeFunction : IFMComponent
        {
            public byte inputSelect = 1;
            private byte[] ops; //0 = "*", 1 = "/", 2 = "+", 3 = "-"
            private double[] values;
            public ModulatorAmplitudeFunction(byte[] ops, double[] values)
            {
                if (ops.Length != values.Length)
                    throw new Exception("Invalid FM Amplitude function.");
                this.ops = ops;
                this.values = values;
            }
            public double doProcess(double value)
            {
                for (int x = 0; x < ops.Length; x++)
                {
                    switch (ops[x])
                    {
                        case 0:
                            value = value * values[x];
                            break;
                        case 1:
                            value = value / values[x];
                            break;
                        case 2:
                            value = value + values[x];
                            break;
                        case 3:
                            value = value - values[x];
                            break;
                    }
                }
                return value;
            }
        }
    }
}
