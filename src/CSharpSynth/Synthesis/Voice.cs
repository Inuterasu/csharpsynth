using System;
using CSharpSynth.Banks;

namespace CSharpSynth.Synthesis
{
    public class Voice
    {
        //--Variables
        private Instrument inst;
        //voice parameters
        private int note;
        private float velocity;
        private ADSR_Envelope ADSR;
        private int channel;
        private float pan;
        private float rightpan;
        private float leftpan;
        //counters and modifiers
        private bool inUse;
        private StreamSynthesizer synth;
        private double time;
        private float fadeMultiplier;
        private float sustainGain = .2f;
        private float attackGain = .4f;
        //generators
        private double vibrafreq = 8;
        private double vibratime;
        //--Public Properties
        public bool isInUse
        {
            get { return inUse; }
        }
        public int Channel
        {
            get { return channel; }
        }
        public Instrument Instrument
        {
            get { return inst; }
            set { inst = value; }
        }
        //--Public Methods
        public Voice(StreamSynthesizer synth)
        {
            ADSR = new ADSR_Envelope(synth.SampleRate);
            resetVoice();
            this.synth = synth;
            this.inst = null;
        }
        public Voice(StreamSynthesizer synth, Instrument inst)
        {
            ADSR = new ADSR_Envelope(synth.SampleRate);
            resetVoice();
            this.synth = synth;
            this.inst = inst;
        }
        public void Start(int channel, int note, int velocity)
        {
            this.note = note;
            this.velocity = (velocity / 127.0f);
            this.channel = channel;
            time = 0.0;
            vibratime = 0.0;
            fadeMultiplier = 1.0f;

            //Set note parameters in samples
            ADSR.DelaySampleTime = inst.getDelay(note);
            ADSR.AttackSampleTime = inst.getAttack(note);
            ADSR.ReleaseSampleTime = inst.getRelease(note);
            ADSR.HoldSampleTime = inst.getHold(note);
            ADSR.DecaySampleTime = inst.getDecay(note);
            ADSR.SustainLevel = sustainGain;
            ADSR.AttackLevel = attackGain;

            //Set counters and initial state
            ADSR.State = ADSR_Envelope.EnvelopeState.delay;
            inUse = true;
        }
        public void Stop()
        {
            ADSR.State = ADSR_Envelope.EnvelopeState.release;
        }
        public void StopImmediately()
        {
            ADSR.State = ADSR_Envelope.EnvelopeState.none;
            inUse = false;
        }
        public void setPan(float pan)
        {
            if (pan >= -1.0f && pan <= 1.0f && this.pan != pan)
            {
                this.pan = pan;
                if (pan > 0.0f)
                {
                    rightpan = 1.00f;
                    leftpan = 1.00f - pan;
                }
                else
                {
                    leftpan = 1.0f;
                    rightpan = 1.00f + pan;
                }
            }
        }
        public float getPan()
        {
            return pan;
        }
        public NoteRegistryKey getKey()
        {
            return new NoteRegistryKey((byte)channel, (byte)note);
        }
        public void Process(float[,] workingBuffer, int startIndex, int endIndex)
        {
            if (inUse)
            {
                //quick checks to do before we go through our main loop
                if (synth.Channels == 2 && pan != synth.PanPositions[channel])
                    this.setPan(synth.PanPositions[channel]);
                //set sampleRate for tune
                double variableSampleRate = synth.SampleRate * Math.Pow(2.0, (synth.TunePositions[channel] * -1.0) / 12.0);
                //main loop
                for (int i = startIndex; i < endIndex; i++)
                {
                    //manage states and calculate volume level
                    fadeMultiplier = ADSR.getDAHDSREnvelopeValue();
                    if (ADSR.State == ADSR_Envelope.EnvelopeState.none)
                        inUse = false;
                    //end of state management
                    
                    //Decide how to sample based on channels available
                    
                    //mono output
                    if (synth.Channels == 1)
                    {
                        float sample = inst.getSampleAtTime(note, 0, synth.SampleRate, ref time);
                        sample = sample * velocity * synth.VolPositions[channel];
                        
                        workingBuffer[0, i] += (sample * fadeMultiplier);
                    }
                    //mono sample to stereo output
                    else
                    {
                        float sample = inst.getSampleAtTime(note, 0, synth.SampleRate, ref time);
                        sample = sample * velocity * synth.VolPositions[channel];

                        workingBuffer[0, i] += (sample * fadeMultiplier * leftpan);
                        workingBuffer[1, i] += (sample * fadeMultiplier * rightpan);
                    }
                    time += 1.0 / (variableSampleRate + SynthHelper.Sine(vibrafreq, vibratime) * (variableSampleRate * synth.VibratoPositions[channel]));
                    vibratime += 1.0 / variableSampleRate;
                    if (vibratime >= 1.0 / vibrafreq)
                        vibratime = vibratime % (1.0 / vibrafreq);
                    //bailout of the loop if there is no reason to continue.
                    if (inUse == false)
                        return;
                }
            }
        }
        //--Private Methods
        private void resetVoice()
        {
            inUse = false;
            ADSR.resetEnvelope();
            note = 0;
            time = 0.0;
            vibratime = 0.0;
            fadeMultiplier = 1.0f;
            pan = 0.0f;
            channel = 0;
            rightpan = 1.0f;
            leftpan = 1.0f;
            velocity = 1.0f;
        }
    }
}
