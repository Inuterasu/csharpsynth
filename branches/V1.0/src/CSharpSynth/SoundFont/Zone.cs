namespace NAudio.SoundFont
{
    using System;

    public class Zone
    {
        internal ushort generatorCount;
        internal ushort generatorIndex;
        private Generator[] generators;
        internal ushort modulatorCount;
        internal ushort modulatorIndex;
        private Modulator[] modulators;

        public override string ToString()
        {
            return string.Format("Zone {0} Gens:{1} {2} Mods:{3}", new object[] { this.generatorCount, this.generatorIndex, this.modulatorCount, this.modulatorIndex });
        }

        public Generator[] Generators
        {
            get
            {
                return this.generators;
            }
            set
            {
                this.generators = value;
            }
        }

        public Modulator[] Modulators
        {
            get
            {
                return this.modulators;
            }
            set
            {
                this.modulators = value;
            }
        }
    }
}

