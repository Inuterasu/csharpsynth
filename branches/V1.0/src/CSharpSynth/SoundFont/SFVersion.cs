﻿namespace CSharpSynth.SoundFont
{
    using System;

    public class SFVersion
    {
        private short major;
        private short minor;

        public short Major
        {
            get
            {
                return this.major;
            }
            set
            {
                this.major = value;
            }
        }

        public short Minor
        {
            get
            {
                return this.minor;
            }
            set
            {
                this.minor = value;
            }
        }
    }
}

