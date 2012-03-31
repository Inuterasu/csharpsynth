namespace CSharpSynth.SoundFont
{
    using System;
    using System.IO;

    internal class ModulatorBuilder : StructureBuilder
    {
        public override object Read(BinaryReader br)
        {
            Modulator modulator = new Modulator {
                SourceModulationData = new ModulatorType(br.ReadUInt16()),
                DestinationGenerator = (GeneratorEnum) br.ReadUInt16(),
                Amount = br.ReadInt16(),
                SourceModulationAmount = new ModulatorType(br.ReadUInt16()),
                SourceTransform = (TransformEnum) br.ReadUInt16()
            };
            base.data.Add(modulator);
            return modulator;
        }

        public override void Write(BinaryWriter bw, object o)
        {
        }

        public override int Length
        {
            get
            {
                return 10;
            }
        }

        public Modulator[] Modulators
        {
            get
            {
                return (Modulator[]) base.data.ToArray(typeof(Modulator));
            }
        }
    }
}

