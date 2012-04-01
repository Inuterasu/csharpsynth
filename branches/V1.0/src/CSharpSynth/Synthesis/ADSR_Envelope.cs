namespace CSharpSynth.Synthesis
{
    public class ADSR_Envelope
    {
        //--Variables
        public enum EnvelopeState { none, delay, attack, hold, decay, sustain, release };
        private EnvelopeState envState;
        private int sampleRate;
        private float peak_attackvalue;
        private float sustain_level;
        private float delay_time;
        private float attack_time;
        private float hold_time;          //how long to hold
        private float decay_time;         //how long to decay into 
        private float release_time;       //how long it takes for volume to reach zero level
        private float time;
        //--Methods
        public ADSR_Envelope(int sampleRate)
        {
            this.sampleRate = sampleRate;
            resetEnvelope();
        }
        public float getDAHDSREnvelopeValue()
        {
            switch (envState)
            {
                case EnvelopeState.none:
                    return 0.0f;
                case EnvelopeState.delay:
                    time++;
                    if (time < delay_time)
                    {
                        return 0.0f;
                    }
                    time = 0;
                    envState = EnvelopeState.attack;
                    return 0.0f;
                case EnvelopeState.attack:
                    time++;
                    if (time < attack_time)
                    {
                        return (time / attack_time) * peak_attackvalue;
                    }
                    time = 0;
                    envState = EnvelopeState.hold;
                    return peak_attackvalue;
                case EnvelopeState.hold:
                    time++;
                    if (time < hold_time)
                    {
                        return peak_attackvalue;
                    }
                    time = 0;
                    envState = EnvelopeState.decay;
                    return peak_attackvalue;
                case EnvelopeState.decay:
                    time++;
                    if (time < decay_time)
                    {
                        return peak_attackvalue - ((time / decay_time) * (peak_attackvalue - sustain_level));
                    }
                    time = 0;
                    envState = EnvelopeState.sustain;
                    return sustain_level;
                case EnvelopeState.sustain:
                    return sustain_level;
                case EnvelopeState.release:
                    time++;
                    if (time < release_time)
                    {
                        return sustain_level - ((time / attack_time) * sustain_level);
                    }
                    time = 0;
                    envState = EnvelopeState.none;
                    return 0.0f;
                default:
                    return 0.0f;
            }
        }  //Advanced 6 state envelope 
        public float getADSREnvelopeValue()
        {
            switch (envState)
            {
                case EnvelopeState.none:
                    return 0.0f;
                case EnvelopeState.attack:
                    time++;
                    if (time < attack_time)
                    {
                        return (time / attack_time) * peak_attackvalue;
                    }
                    time = 0;
                    envState = EnvelopeState.decay;
                    return peak_attackvalue;
                case EnvelopeState.decay:
                    time++;
                    if (time < decay_time)
                    {
                        return peak_attackvalue - ((time / decay_time) * (peak_attackvalue - sustain_level));
                    }
                    time = 0;
                    envState = EnvelopeState.sustain;
                    return sustain_level;
                case EnvelopeState.sustain:
                    return sustain_level;
                case EnvelopeState.release:
                    time++;
                    if (time < release_time)
                    {
                        return sustain_level - ((time / attack_time) * sustain_level);
                    }
                    time = 0;
                    envState = EnvelopeState.none;
                    return 0.0f;
                default:
                    return 0.0f;
            }
        }    //Simple 4 state envelope
        public void setDefaultValues()
        {
            time = 0.0f;
            peak_attackvalue = SynthHelper.Clamp(SynthHelper.DEFAULT_AMPLITUDE + (SynthHelper.DEFAULT_AMPLITUDE / 5f), 0f, 1f);
            sustain_level = SynthHelper.DEFAULT_AMPLITUDE;
            delay_time = SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_DELAY);
            attack_time = SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_ATTACK);
            hold_time = SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_HOLD);
            decay_time = SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_DECAY);
            release_time = SynthHelper.getSampleFromTime(sampleRate, SynthHelper.DEFAULT_RELEASE);
            envState = EnvelopeState.none;
        }
        public void resetEnvelope()
        {
            time = 0.0f;
        }
        //--Properties
        public EnvelopeState State
        {
            get { return envState; }
            set 
            { 
                envState = value;
                time = 0.0f;
            }
        }
        public float AttackLevel
        {
            get { return peak_attackvalue; }
            set { peak_attackvalue = value; }
        }
        public float SustainLevel
        {
            get { return sustain_level; }
            set { sustain_level = value; }
        }
        public float DelaySampleTime
        {
            get { return delay_time; }
            set { delay_time = value; }
        }
        public float AttackSampleTime
        {
            get { return attack_time; }
            set { attack_time = value; }
        }
        public float HoldSampleTime
        {
            get { return hold_time; }
            set { hold_time = value; }
        }
        public float DecaySampleTime
        {
            get { return decay_time; }
            set { decay_time = value; }
        }
        public float ReleaseSampleTime
        {
            get { return release_time; }
            set { release_time = value; }
        }
    }
}
