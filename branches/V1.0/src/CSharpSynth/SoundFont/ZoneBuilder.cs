namespace NAudio.SoundFont
{
    using System;
    using System.IO;

    internal class ZoneBuilder : StructureBuilder
    {
        private Zone lastZone = null;

        public void Load(Modulator[] modulators, Generator[] generators)
        {
            for (int i = 0; i < (base.data.Count - 1); i++)
            {
                Zone zone = (Zone) base.data[i];
                zone.Generators = new Generator[zone.generatorCount];
                Array.Copy(generators, zone.generatorIndex, zone.Generators, 0, zone.generatorCount);
                zone.Modulators = new Modulator[zone.modulatorCount];
                Array.Copy(modulators, zone.modulatorIndex, zone.Modulators, 0, zone.modulatorCount);
            }
            base.data.RemoveAt(base.data.Count - 1);
        }

        public override object Read(BinaryReader br)
        {
            Zone zone = new Zone {
                generatorIndex = br.ReadUInt16(),
                modulatorIndex = br.ReadUInt16()
            };
            if (this.lastZone != null)
            {
                this.lastZone.generatorCount = (ushort) (zone.generatorIndex - this.lastZone.generatorIndex);
                this.lastZone.modulatorCount = (ushort) (zone.modulatorIndex - this.lastZone.modulatorIndex);
            }
            base.data.Add(zone);
            this.lastZone = zone;
            return zone;
        }

        public override void Write(BinaryWriter bw, object o)
        {
            Zone zone = (Zone) o;
        }

        public override int Length
        {
            get
            {
                return 4;
            }
        }

        public Zone[] Zones
        {
            get
            {
                return (Zone[]) base.data.ToArray(typeof(Zone));
            }
        }
    }
}

