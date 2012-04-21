namespace CSharpSynth.SoundFont
{
    using System;
    using System.IO;
    using System.Text;

    internal class InstrumentBuilder : StructureBuilder
    {
        private Instrument lastInstrument = null;

        public void LoadZones(Zone[] zones)
        {
            for (int i = 0; i < (base.data.Count - 1); i++)
            {
                Instrument instrument = (Instrument) base.data[i];
                instrument.Zones = new Zone[(instrument.endInstrumentZoneIndex - instrument.startInstrumentZoneIndex) + 1];
                Array.Copy(zones, instrument.startInstrumentZoneIndex, instrument.Zones, 0, instrument.Zones.Length);
            }
            base.data.RemoveAt(base.data.Count - 1);
        }

        public override object Read(BinaryReader br)
        {
            Instrument instrument = new Instrument();
            string str = Encoding.ASCII.GetString(br.ReadBytes(20));
            if (str.IndexOf('\0') >= 0)
            {
                str = str.Substring(0, str.IndexOf('\0'));
            }
            instrument.Name = str;
            instrument.startInstrumentZoneIndex = br.ReadUInt16();
            if (this.lastInstrument != null)
            {
                this.lastInstrument.endInstrumentZoneIndex = (ushort) (instrument.startInstrumentZoneIndex - 1);
            }
            base.data.Add(instrument);
            this.lastInstrument = instrument;
            return instrument;
        }

        public override void Write(BinaryWriter bw, object o)
        {
            //Instrument instrument = (Instrument) o;
        }

        public Instrument[] Instruments
        {
            get
            {
                return (Instrument[]) base.data.ToArray(typeof(Instrument));
            }
        }

        public override int Length
        {
            get
            {
                return 0x16;
            }
        }
    }
}

