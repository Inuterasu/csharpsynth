namespace CSharpSynth.SoundFont
{
    using System;
    using System.IO;

    internal class GeneratorBuilder : StructureBuilder
    {
        public void Load(Instrument[] instruments)
        {
            foreach (Generator generator in this.Generators)
            {
                if (generator.GeneratorType == GeneratorEnum.Instrument)
                {
                    generator.Instrument = instruments[generator.UInt16Amount];
                }
            }
        }

        public void Load(SampleHeader[] sampleHeaders)
        {
            foreach (Generator generator in this.Generators)
            {
                if (generator.GeneratorType == GeneratorEnum.SampleID)
                {
                    generator.SampleHeader = sampleHeaders[generator.UInt16Amount];
                }
            }
        }

        public override object Read(BinaryReader br)
        {
            Generator generator = new Generator {
                GeneratorType = (GeneratorEnum) br.ReadUInt16(),
                UInt16Amount = br.ReadUInt16()
            };
            base.data.Add(generator);
            return generator;
        }

        public override void Write(BinaryWriter bw, object o)
        {
        }

        public Generator[] Generators
        {
            get
            {
                return (Generator[]) base.data.ToArray(typeof(Generator));
            }
        }

        public override int Length
        {
            get
            {
                return 4;
            }
        }
    }
}

