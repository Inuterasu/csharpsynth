namespace NAudio.SoundFont
{
    using System;

    public class ModulatorType
    {
        private ControllerSourceEnum controllerSource;
        private bool direction;
        private bool midiContinuousController;
        private ushort midiContinuousControllerNumber;
        private bool polarity;
        private SourceTypeEnum sourceType;

        internal ModulatorType(ushort raw)
        {
            this.polarity = (raw & 0x200) == 0x200;
            this.direction = (raw & 0x100) == 0x100;
            this.midiContinuousController = (raw & 0x80) == 0x80;
            this.sourceType = (SourceTypeEnum) ((raw & 0xfc00) >> 10);
            this.controllerSource = ((ControllerSourceEnum) raw) & ((ControllerSourceEnum) 0x7f);
            this.midiContinuousControllerNumber = (ushort) (raw & 0x7f);
        }

        public override string ToString()
        {
            if (this.midiContinuousController)
            {
                return string.Format("{0} CC{1}", this.sourceType, this.midiContinuousControllerNumber);
            }
            return string.Format("{0} {1}", this.sourceType, this.controllerSource);
        }
    }
}

