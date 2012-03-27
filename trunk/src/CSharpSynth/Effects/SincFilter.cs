using CSharpSynth.Wave.DSP;
using CSharpSynth.Synthesis;

namespace CSharpSynth.Effects
{
    public class SincFilter : BasicAudioEffect
    {
        //--Variables
        private SincLowPass sfilter;
        //--Public Methods
        public SincFilter(StreamSynthesizer synth, int filtersize, double cornerfreq)
            : base()
        {
            sfilter = new SincLowPass(synth.Channels, filtersize, cornerfreq);
        }
        public override void doEffect(float[,] inputBuffer)
        {
            sfilter.ApplyFilter(inputBuffer);
        }
    }
}
