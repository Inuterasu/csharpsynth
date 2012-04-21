using CSharpSynth.Wave;
using CSharpSynth.Synthesis;

namespace CSharpSynth.Banks
{
    public abstract class Instrument
    {
        //--Variables
        private Sample[] instrumentSamples;
        private string instrumentName;
        private int sampleRate;
        //--Virtual Methods
        public virtual float getSampleAtTime(int note, int channel, int synthSampleRate, ref double time)
        {
            return 0.0f;
        }
        public virtual int getDelay(int note)
        {
            return SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_DELAY);
        }
        public virtual int getAttack(int note)
        {
            return SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_ATTACK);
        }
        public virtual int getRelease(int note)
        {
            return SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_RELEASE);
        }
        public virtual int getHold(int note)
        {
            return SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_HOLD);
        }
        public virtual int getDecay(int note)
        {
            return SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_DECAY);
        }
        public virtual float getSustainLevel(int note)
        {
            return SynthHelper.DEFAULT_SUSTAIN_LEVEL;
        }
        public virtual float getAttackLevel(int note)
        {
            return SynthHelper.DEFAULT_ATTACK_LEVEL;
        }
        //--Abstract Methods
        public abstract void enforceSampleRate(int sampleRate);
        public abstract bool allSamplesSupportDualChannel();
        //--Public Properties
        public string Name
        {
            get { return instrumentName; }
            set { instrumentName = value; }
        }
        public Sample[] SampleList
        {
            get { return instrumentSamples; }
            set { instrumentSamples = value; }
        }
        public int SampleRate
        {
            get { return sampleRate; }
            set { sampleRate = value; }
        }
    }
}
