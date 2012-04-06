using CSharpSynth.Synthesis;

namespace CSharpSynth.Banks.Sf2
{
    public class Sf2Region
    {
        public int Release;   //Samples releaseVolEnv
        public int Attack;    //Samples attackVolEnv
        public int Hold;      //Samples holdVolEnv
        public int Decay;     //Samples decayVolEnv
        public int Delay;     //Samples delayVolEnv
        public float sustainVolEnv = 0.0f;
        public int keynumToVolEnvHold = 0;
        public int keynumToVolEnvDecay = 0;
        //
        public int startAddrsOffset = 0;
        public int endAddrsOffset = 0;
        public int startloopAddrsOffset = 0;
        public int endloopAddrsOffset = 0;
        public int startAddrsCoarseOffset = 0;
        public int endAddrsCoarseOffset = 0;
        public int startloopAddrsCoarseOffset = 0;
        public int endloopAddrsCoarseOffset = 0;
        public float pan = 0.0f;
        public float coarseTune = 0.0f;
        public float fineTune = 0.0f;
        public float scaleTuning = 1.0f; //one semitone per note
        public int overridingRootKey = -1;
        public int keynum = -1;
        public int velocity = -1;
        public int lowKey = 0;
        public int highKey = 127;
        public int lowVel = 0;
        public int highVel = 127;

        public bool isInRegion(int note, int velocity)
        {
            return (note >= lowKey && note <= highKey) && (velocity >= lowVel && velocity <= highVel);
        }
        public Sf2Region(int sampleRate)
        {
            Attack = SynthHelper.getSampleFromTime(sampleRate, .001f);
            Release =Attack;
            Hold = Attack;
            Decay = Attack;
            Delay = Attack;
        }
        public void enforceSampleRate(int oldSampleRate, int newSampleRate)
        {
            float diff = (float)newSampleRate / (float)oldSampleRate;
            Release = (int)(Release * diff);
            Attack = (int)(Attack * diff);
            Hold = (int)(Hold * diff);
            Decay = (int)(Decay * diff);
            Delay = (int)(Delay * diff);
            keynumToVolEnvHold = (int)(keynumToVolEnvHold * diff);
            keynumToVolEnvDecay = (int)(keynumToVolEnvDecay * diff);
            startAddrsOffset = (int)(startAddrsOffset * diff);
            endAddrsOffset = (int)(endAddrsOffset * diff);
            startloopAddrsOffset = (int)(startloopAddrsOffset * diff);
            endloopAddrsOffset = (int)(endloopAddrsOffset * diff);
            startAddrsCoarseOffset = (int)(startAddrsCoarseOffset * diff);
            endAddrsCoarseOffset = (int)(endAddrsCoarseOffset * diff);
            startloopAddrsCoarseOffset = (int)(startloopAddrsCoarseOffset * diff);
            endloopAddrsCoarseOffset = (int)(endloopAddrsCoarseOffset * diff);
        }
    }
}
