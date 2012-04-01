using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpSynth.Synthesis;

namespace CSharpSynth.Effects
{
    public class Reverb : BasicAudioEffect
    {
        private int[] delay;
        private float[] decay;
        private int channels;
        private int samplesperbuffer;

        public Reverb(StreamSynthesizer synth, float delay, float decay)
            : base()
        {
            this.channels = synth.Channels;
            this.samplesperbuffer = synth.SamplesPerBuffer;
            
            this.decay = new float[channels];
            for (int x = 0; x < channels; x++)
                this.decay[x] = decay + (float)(SynthHelper.getRandom() * (decay / 20.0));
            this.delay = new int[channels];
            for (int x = 0; x < channels; x++)
                this.delay[x] = SynthHelper.getSampleFromTime(synth.SampleRate, delay + (float)(SynthHelper.getRandom() * (delay / 20.0)));
            this.EffectBuffer = new float[synth.Channels, samplesperbuffer];
        }
        public override void doEffect(float[,] inputBuffer)
        {
            for (int c = 0; c < channels; c++)
            {
                for (int i = 0; i < samplesperbuffer - delay[c]; i++)
                {
                    inputBuffer[c, i + delay[c]] += inputBuffer[c, i] * decay[c];
                }
            }
        }
    }
}
