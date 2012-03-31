namespace NAudio.SoundFont
{
    using System;
    using System.Text;

    public class PresetsChunk
    {
        private InstrumentBuilder instruments;
        private GeneratorBuilder instrumentZoneGenerators;
        private ModulatorBuilder instrumentZoneModulators;
        private ZoneBuilder instrumentZones;
        private PresetBuilder presetHeaders;
        private GeneratorBuilder presetZoneGenerators;
        private ModulatorBuilder presetZoneModulators;
        private ZoneBuilder presetZones;
        private SampleHeaderBuilder sampleHeaders;

        internal PresetsChunk(RiffChunk chunk)
        {
            RiffChunk chunk2;
            this.presetHeaders = new PresetBuilder();
            this.presetZones = new ZoneBuilder();
            this.presetZoneModulators = new ModulatorBuilder();
            this.presetZoneGenerators = new GeneratorBuilder();
            this.instruments = new InstrumentBuilder();
            this.instrumentZones = new ZoneBuilder();
            this.instrumentZoneModulators = new ModulatorBuilder();
            this.instrumentZoneGenerators = new GeneratorBuilder();
            this.sampleHeaders = new SampleHeaderBuilder();
            string str = chunk.ReadChunkID();
            if (str != "pdta")
            {
                throw new ApplicationException(string.Format("Not a presets data chunk ({0})", str));
            }
            while ((chunk2 = chunk.GetNextSubChunk()) != null)
            {
                switch (chunk2.ChunkID)
                {
                    case "PHDR":
                    case "phdr":
                        chunk2.GetDataAsStructureArray(this.presetHeaders);
                        break;

                    case "PBAG":
                    case "pbag":
                        chunk2.GetDataAsStructureArray(this.presetZones);
                        break;

                    case "PMOD":
                    case "pmod":
                        chunk2.GetDataAsStructureArray(this.presetZoneModulators);
                        break;

                    case "PGEN":
                    case "pgen":
                        chunk2.GetDataAsStructureArray(this.presetZoneGenerators);
                        break;

                    case "INST":
                    case "inst":
                        chunk2.GetDataAsStructureArray(this.instruments);
                        break;

                    case "IBAG":
                    case "ibag":
                        chunk2.GetDataAsStructureArray(this.instrumentZones);
                        break;

                    case "IMOD":
                    case "imod":
                        chunk2.GetDataAsStructureArray(this.instrumentZoneModulators);
                        break;

                    case "IGEN":
                    case "igen":
                        chunk2.GetDataAsStructureArray(this.instrumentZoneGenerators);
                        break;

                    case "SHDR":
                    case "shdr":
                        chunk2.GetDataAsStructureArray(this.sampleHeaders);
                        break;

                    default:
                        throw new ApplicationException(string.Format("Unknown chunk type {0}", chunk2.ChunkID));
                }
            }
            this.instrumentZoneGenerators.Load(this.sampleHeaders.SampleHeaders);
            this.instrumentZones.Load(this.instrumentZoneModulators.Modulators, this.instrumentZoneGenerators.Generators);
            this.instruments.LoadZones(this.instrumentZones.Zones);
            this.presetZoneGenerators.Load(this.instruments.Instruments);
            this.presetZones.Load(this.presetZoneModulators.Modulators, this.presetZoneGenerators.Generators);
            this.presetHeaders.LoadZones(this.presetZones.Zones);
            this.sampleHeaders.RemoveEOS();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Preset Headers:\r\n");
            foreach (Preset preset in this.presetHeaders.Presets)
            {
                builder.AppendFormat("{0}\r\n", preset);
            }
            builder.Append("Instruments:\r\n");
            foreach (Instrument instrument in this.instruments.Instruments)
            {
                builder.AppendFormat("{0}\r\n", instrument);
            }
            return builder.ToString();
        }

        public Instrument[] Instruments
        {
            get
            {
                return this.instruments.Instruments;
            }
        }

        public Preset[] Presets
        {
            get
            {
                return this.presetHeaders.Presets;
            }
        }

        public SampleHeader[] SampleHeaders
        {
            get
            {
                return this.sampleHeaders.SampleHeaders;
            }
        }
    }
}

