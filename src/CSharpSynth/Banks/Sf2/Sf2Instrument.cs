using System;
using System.IO;
using CSharpSynth.Wave;
using CSharpSynth.Synthesis;

namespace CSharpSynth.Banks.Sf2
{
    public class Sf2Instrument : Instrument
    {
        public Sf2Instrument(SoundFont.Instrument inst, int sampleRate)
            : base()
        {
            this.SampleRate = sampleRate;
            this.Name = inst.Name;
            loadStream(inst);
        }
        public override void enforceSampleRate(int sampleRate)
        {
            throw new NotImplementedException();
        }
        public override bool allSamplesSupportDualChannel()
        {
            return false;
        }
        private void loadStream(SoundFont.Instrument inst)
        {
            //Parse Zone Data
            for(int x=0;x<inst.Zones.Length;x++)
            {
                if (inst.Zones[x].Generators.Length > 0)
                {
                    //if the last generator isn't an instrument generator this is a global zone
                    if (inst.Zones[x].Generators[inst.Zones[x].Generators.Length - 1].GeneratorType != SoundFont.GeneratorEnum.Instrument)
                    {

                    }
                    else
                    {

                    }
                }
            }
            
        }
        private Sf2Region ZoneToRegion(SoundFont.Zone zone)
        {
            Sf2Region sfRegion = new Sf2Region(this.SampleRate);
            for (int x = 0; x < zone.Generators.Length; x++)
            {
                switch(zone.Generators[x].GeneratorType)
                {
                    case SoundFont.GeneratorEnum.ReleaseVolumeEnvelope:
                        sfRegion.Release = SynthHelper.getSampleFromTime(this.SampleRate, (float)Math.Pow(2, zone.Generators[x].Int16Amount / 1200.0));
                        break;
                    case SoundFont.GeneratorEnum.AttackVolumeEnvelope:
                        sfRegion.Attack = SynthHelper.getSampleFromTime(this.SampleRate, (float)Math.Pow(2, zone.Generators[x].Int16Amount / 1200.0));
                        break;
                    case SoundFont.GeneratorEnum.HoldVolumeEnvelope:
                        sfRegion.Hold = SynthHelper.getSampleFromTime(this.SampleRate, (float)Math.Pow(2, zone.Generators[x].Int16Amount / 1200.0));
                        break;
                    case SoundFont.GeneratorEnum.DecayVolumeEnvelope:
                        sfRegion.Decay = SynthHelper.getSampleFromTime(this.SampleRate, (float)Math.Pow(2, zone.Generators[x].Int16Amount / 1200.0));
                        break;
                    case SoundFont.GeneratorEnum.DelayVolumeEnvelope:
                        sfRegion.Delay = SynthHelper.getSampleFromTime(this.SampleRate, (float)Math.Pow(2, zone.Generators[x].Int16Amount / 1200.0));
                        break;
                    case SoundFont.GeneratorEnum.KeyNumberToVolumeEnvelopeDecay:

                        break;
                    case SoundFont.GeneratorEnum.KeyNumberToVolumeEnvelopeHold:
                        break;
                    case SoundFont.GeneratorEnum.StartAddressOffset:
                        sfRegion.startAddrsOffset = zone.Generators[x].Int16Amount;
                        break;
                    case SoundFont.GeneratorEnum.EndAddressOffset:
                        sfRegion.endAddrsOffset = zone.Generators[x].Int16Amount;
                        break;
                    case SoundFont.GeneratorEnum.StartLoopAddressOffset:
                        sfRegion.startloopAddrsOffset = zone.Generators[x].Int16Amount;
                        break;
                    case SoundFont.GeneratorEnum.EndLoopAddressOffset:
                        sfRegion.endloopAddrsOffset = zone.Generators[x].Int16Amount;
                        break;
                    case SoundFont.GeneratorEnum.StartAddressCoarseOffset:
                        sfRegion.startAddrsCoarseOffset = zone.Generators[x].Int16Amount * 32768;
                        break;
                    case SoundFont.GeneratorEnum.EndAddressCoarseOffset:
                        sfRegion.endAddrsCoarseOffset = zone.Generators[x].Int16Amount * 32768;
                        break;
                    case SoundFont.GeneratorEnum.StartLoopAddressCoarseOffset:
                        sfRegion.startloopAddrsCoarseOffset = zone.Generators[x].Int16Amount * 32768;
                        break;
                    case SoundFont.GeneratorEnum.EndLoopAddressCoarseOffset:
                        sfRegion.endloopAddrsCoarseOffset = zone.Generators[x].Int16Amount * 32768;
                        break;
                    case SoundFont.GeneratorEnum.Pan:
                        if (zone.Generators[x].Int16Amount < 0)
                            sfRegion.pan = (-500 + zone.Generators[x].Int16Amount) / 1000f;
                        else if (zone.Generators[x].Int16Amount > 0)
                            sfRegion.pan = (500 + zone.Generators[x].Int16Amount) / 1000f;
                        else
                            sfRegion.pan = 0.0f;
                        break;
                    case SoundFont.GeneratorEnum.CoarseTune:
                        sfRegion.coarseTune = zone.Generators[x].Int16Amount;
                        break;
                    case SoundFont.GeneratorEnum.FineTune:
                        sfRegion.coarseTune = (zone.Generators[x].Int16Amount / 100f);
                        break;
                    case SoundFont.GeneratorEnum.ScaleTuning:
                        sfRegion.scaleTuning = (zone.Generators[x].Int16Amount / 100f);
                        break;
                    case SoundFont.GeneratorEnum.OverridingRootKey:
                        sfRegion.overridingRootKey = zone.Generators[x].Int16Amount;
                        break;
                    case SoundFont.GeneratorEnum.KeyNumber:
                        sfRegion.keynum = zone.Generators[x].Int16Amount;
                        break;
                    case SoundFont.GeneratorEnum.Velocity:
                        sfRegion.velocity = zone.Generators[x].Int16Amount;
                        break;
                    case SoundFont.GeneratorEnum.KeyRange:
                        sfRegion.lowKey = zone.Generators[x].HighByteAmount;
                        sfRegion.highKey = zone.Generators[x].LowByteAmount;
                        break;
                    case SoundFont.GeneratorEnum.VelocityRange:
                        sfRegion.lowVel = zone.Generators[x].HighByteAmount;
                        sfRegion.highVel = zone.Generators[x].LowByteAmount;
                        break;
                }
            }
            return sfRegion;
        }      
    }
}
