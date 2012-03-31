namespace NAudio.SoundFont
{
    using System;

    public class Modulator
    {
        private short amount;
        private GeneratorEnum destinationGenerator;
        private ModulatorType sourceModulationAmount;
        private ModulatorType sourceModulationData;
        private TransformEnum sourceTransform;

        public override string ToString()
        {
            return string.Format("Modulator {0} {1} {2} {3} {4}", new object[] { this.sourceModulationData, this.destinationGenerator, this.amount, this.sourceModulationAmount, this.sourceTransform });
        }

        public short Amount
        {
            get
            {
                return this.amount;
            }
            set
            {
                this.amount = value;
            }
        }

        public GeneratorEnum DestinationGenerator
        {
            get
            {
                return this.destinationGenerator;
            }
            set
            {
                this.destinationGenerator = value;
            }
        }

        public ModulatorType SourceModulationAmount
        {
            get
            {
                return this.sourceModulationAmount;
            }
            set
            {
                this.sourceModulationAmount = value;
            }
        }

        public ModulatorType SourceModulationData
        {
            get
            {
                return this.sourceModulationData;
            }
            set
            {
                this.sourceModulationData = value;
            }
        }

        public TransformEnum SourceTransform
        {
            get
            {
                return this.sourceTransform;
            }
            set
            {
                this.sourceTransform = value;
            }
        }
    }
}

