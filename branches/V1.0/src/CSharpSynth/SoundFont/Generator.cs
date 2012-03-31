namespace NAudio.SoundFont
{
    using System;

    public class Generator
    {
        private GeneratorEnum generatorType;
        private NAudio.SoundFont.Instrument instrument;
        private ushort rawAmount;
        private NAudio.SoundFont.SampleHeader sampleHeader;

        public override string ToString()
        {
            if (this.generatorType == GeneratorEnum.Instrument)
            {
                return string.Format("Generator Instrument {0}", this.instrument.Name);
            }
            if (this.generatorType == GeneratorEnum.SampleID)
            {
                return string.Format("Generator SampleID {0}", this.sampleHeader);
            }
            return string.Format("Generator {0} {1}", this.generatorType, this.rawAmount);
        }

        public GeneratorEnum GeneratorType
        {
            get
            {
                return this.generatorType;
            }
            set
            {
                this.generatorType = value;
            }
        }

        public byte HighByteAmount
        {
            get
            {
                return (byte) ((this.rawAmount & 0xff00) >> 8);
            }
            set
            {
                this.rawAmount = (ushort) (this.rawAmount & 0xff);
                this.rawAmount = (ushort) (this.rawAmount + ((ushort) (value << 8)));
            }
        }

        public NAudio.SoundFont.Instrument Instrument
        {
            get
            {
                return this.instrument;
            }
            set
            {
                this.instrument = value;
            }
        }

        public short Int16Amount
        {
            get
            {
                return (short) this.rawAmount;
            }
            set
            {
                this.rawAmount = (ushort) value;
            }
        }

        public byte LowByteAmount
        {
            get
            {
                return (byte) (this.rawAmount & 0xff);
            }
            set
            {
                this.rawAmount = (ushort) (this.rawAmount & 0xff00);
                this.rawAmount = (ushort) (this.rawAmount + value);
            }
        }

        public NAudio.SoundFont.SampleHeader SampleHeader
        {
            get
            {
                return this.sampleHeader;
            }
            set
            {
                this.sampleHeader = value;
            }
        }

        public ushort UInt16Amount
        {
            get
            {
                return this.rawAmount;
            }
            set
            {
                this.rawAmount = value;
            }
        }
    }
}

