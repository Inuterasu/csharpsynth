namespace NAudio.SoundFont
{
    using System;

    public enum SFSampleLink : ushort
    {
        LeftSample = 4,
        LinkedSample = 8,
        MonoSample = 1,
        RightSample = 2,
        RomLeftSample = 0x8004,
        RomLinkedSample = 0x8008,
        RomMonoSample = 0x8001,
        RomRightSample = 0x8002
    }
}

