namespace CSharpSynth.SoundFont
{
    using System;

    public class Instrument
    {
        internal ushort endInstrumentZoneIndex;
        private string name;
        internal ushort startInstrumentZoneIndex;
        private Zone[] zones;

        public override string ToString()
        {
            return this.name;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public Zone[] Zones
        {
            get
            {
                return this.zones;
            }
            set
            {
                this.zones = value;
            }
        }
    }
}

