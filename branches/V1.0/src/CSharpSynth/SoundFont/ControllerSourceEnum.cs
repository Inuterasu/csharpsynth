namespace NAudio.SoundFont
{
    using System;

    public enum ControllerSourceEnum
    {
        ChannelPressure = 13,
        NoController = 0,
        NoteOnKeyNumber = 3,
        NoteOnVelocity = 2,
        PitchWheel = 14,
        PitchWheelSensitivity = 0x10,
        PolyPressure = 10
    }
}

