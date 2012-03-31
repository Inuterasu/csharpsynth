namespace NAudio.SoundFont
{
    using System;

    public class Preset
    {
        private ushort bank;
        internal ushort endPresetZoneIndex;
        internal uint genre;
        internal uint library;
        internal uint morphology;
        private string name;
        private ushort patchNumber;
        internal ushort startPresetZoneIndex;
        private Zone[] zones;

        public override string ToString()
        {
            return string.Format("{0}-{1} {2}", this.bank, this.patchNumber, this.name);
        }

        public ushort Bank
        {
            get
            {
                return this.bank;
            }
            set
            {
                this.bank = value;
            }
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

        public ushort PatchNumber
        {
            get
            {
                return this.patchNumber;
            }
            set
            {
                this.patchNumber = value;
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

