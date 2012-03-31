namespace NAudio.SoundFont
{
    using System;

    public class SampleHeader
    {
        public uint End;
        public uint EndLoop;
        public byte OriginalPitch;
        public sbyte PitchCorrection;
        public ushort SampleLink;
        public string SampleName;
        public uint SampleRate;
        public NAudio.SoundFont.SFSampleLink SFSampleLink;
        public uint Start;
        public uint StartLoop;

        public override string ToString()
        {
            return this.SampleName;
        }
    }
}

