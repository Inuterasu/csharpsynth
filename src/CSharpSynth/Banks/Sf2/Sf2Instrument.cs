using System;
using CSharpSynth.Synthesis;
using System.Collections.Generic;
using CSharpSynth.Wave;

namespace CSharpSynth.Banks.Sf2
{
    public class Sf2Instrument : Instrument
    {
        private int[,] noteMap = new int[128,128]; //(note,velocity)
        private Sf2Region[] regions;

        public Sf2Instrument(SoundFont.Instrument inst, int sampleRate)
            : base()
        {
            this.SampleRate = sampleRate;
            this.Name = inst.Name;
            loadFromInstrument(inst);
        }
        public override void enforceSampleRate(int sampleRate)
        {
            for (int x = 0; x < regions.Length; x++)
                regions[x].enforceSampleRate(this.SampleRate, sampleRate);
            this.SampleRate = sampleRate;
        }
        public override bool allSamplesSupportDualChannel()
        {
            return false;
        }
        private void loadFromInstrument(SoundFont.Instrument inst)
        {
            Sf2Region globalRegion;
            List<Sf2Region> regions = new List<Sf2Region>();
            //Parse Zone Data into Regions (strips unused parameters and puts others into format usable by this synth)
            for(int x=0;x<inst.Zones.Length;x++)
            {
                if (inst.Zones[x].Generators.Length > 0)
                {
                    //if the last generator isn't an instrument generator this is a global zone
                    if (x == 0 && inst.Zones[x].Generators[inst.Zones[x].Generators.Length - 1].GeneratorType != SoundFont.GeneratorEnum.SampleID)
                    {
                        globalRegion = ZoneToRegion(inst.Zones[x]);
                    }
                    else
                    {
                        Sf2Region r = ZoneToRegion(inst.Zones[x]);
                        regions.Add(r);
                    }
                }
            }
            this.regions = regions.ToArray();
        }
        private Sf2Region ZoneToRegion(SoundFont.Zone zone)
        {
            Sf2Region sfRegion = new Sf2Region(this.SampleRate);
            SoundFont.SampleHeader shead = null;
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
                    case SoundFont.GeneratorEnum.SustainVolumeEnvelope:
                        if(zone.Generators[x].Int16Amount <= 0)
                            sfRegion.sustainVolEnv = 1.0f;
                        else
                            sfRegion.sustainVolEnv = SynthHelper.dBtoLinear((zone.Generators[x].Int16Amount / 10.0));
                        sfRegion.sustainVolEnv = SynthHelper.Clamp(sfRegion.sustainVolEnv, 0.0f, 1.0f);
                        break;
                    case SoundFont.GeneratorEnum.KeyNumberToVolumeEnvelopeDecay: //finish this !!
                        sfRegion.keynumToVolEnvDecay = 0;
                        break;
                    case SoundFont.GeneratorEnum.KeyNumberToVolumeEnvelopeHold: //finish this !!
                        sfRegion.keynumToVolEnvDecay = 0;
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
                        sfRegion.fineTune = (zone.Generators[x].Int16Amount / 100f);
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
                        sfRegion.lowKey = zone.Generators[x].LowByteAmount;
                        sfRegion.highKey = zone.Generators[x].HighByteAmount;
                        break;
                    case SoundFont.GeneratorEnum.VelocityRange:
                        sfRegion.lowVel = zone.Generators[x].LowByteAmount;
                        sfRegion.highVel = zone.Generators[x].HighByteAmount;
                        break;
                    case SoundFont.GeneratorEnum.SampleModes:
                        sfRegion.loopMode = zone.Generators[x].Int16Amount;
                        break;
                    case SoundFont.GeneratorEnum.SampleID:
                        sfRegion.sampleID = zone.Generators[x].Int16Amount;
                        shead = zone.Generators[x].SampleHeader;
                        break;
                    default:
                        break;
                }
            }
            if (shead != null)
            {
                if(sfRegion.overridingRootKey == -1)
                    sfRegion.overridingRootKey = shead.OriginalPitch;
                sfRegion.startIndex = (int)shead.Start;
                sfRegion.endIndex = (int)shead.End;
                sfRegion.loopstartIndex = (int)shead.StartLoop;
                sfRegion.loopendIndex = (int)shead.EndLoop;
                sfRegion.fineTune += shead.PitchCorrection / 100.0f;
            }

            return sfRegion;
        }      
        private void setupNotemap()
        {
            for (int x = 0; x < regions.Length; x++)
            {
                for (int n = regions[x].lowKey; n <= regions[x].highKey; n++)
                {
                    for (int v = regions[x].lowVel; v <= regions[x].highVel; v++)
                    {
                        noteMap[n, v] = x;
                    }
                }
            }
        }
    }
}
