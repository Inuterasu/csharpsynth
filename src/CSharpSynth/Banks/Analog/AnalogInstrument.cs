using CSharpSynth.Synthesis;

namespace CSharpSynth.Banks.Analog
{
    public class AnalogInstrument : Instrument
    {
        //--Variables
        private SynthHelper.WaveFormType type;
        private int _attack;
        private int _release;
        private int _decay;
        private int _hold;
        private int _delay;
        public SynthHelper.WaveFormType WaveForm
        {
            get { return type; }
            set { type = value; }
        }
        //--Public Methods
        public AnalogInstrument(SynthHelper.WaveFormType waveformtype, int sampleRate)
            : base()
        {
            //set type
            this.type = waveformtype;
            this.SampleRate = sampleRate;
            //Proper calculation of voice states
            _attack = SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_ATTACK);
            _release = SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_RELEASE);
            _decay = SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_DECAY);
            _hold = SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_HOLD);
            _delay = SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_DELAY);
            //set base attribute name
            base.Name = waveformtype.ToString();
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
                this.SampleRate = sampleRate;
            }
        }
        public override int getDelay(int note)
        {
            return _delay;
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
        public override float getSampleAtTime(int note, int channel, int synthSampleRate, ref double time)
        {
            double freq = SynthHelper.NoteToFrequency(note);
            double delta = (1.0 / freq); //Position in wave form in 2PI * (time* frequency)
            if (time >= delta)//Waveform repeates at 1.0 / freq         
                time = time % delta;
            switch (type)
            {
                case SynthHelper.WaveFormType.Sine:
                    return SynthHelper.Sine(freq, time) * SynthHelper.DEFAULT_AMPLITUDE;
                case SynthHelper.WaveFormType.Sawtooth:
                    return SynthHelper.Sawtooth(freq, time) * SynthHelper.DEFAULT_AMPLITUDE;
                case SynthHelper.WaveFormType.Square:
                    return SynthHelper.Square(freq, time) * SynthHelper.DEFAULT_AMPLITUDE;
                case SynthHelper.WaveFormType.Triangle:
                    return SynthHelper.Triangle(freq, time) * SynthHelper.DEFAULT_AMPLITUDE;
                case SynthHelper.WaveFormType.WhiteNoise:
                    return SynthHelper.WhiteNoise(note, time) * SynthHelper.DEFAULT_AMPLITUDE;
                default:
                    return 0.0f;
            }
        }
    }
}
