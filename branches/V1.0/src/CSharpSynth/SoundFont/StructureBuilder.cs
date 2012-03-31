namespace NAudio.SoundFont
{
    using System;
    using System.Collections;
    using System.IO;

    internal abstract class StructureBuilder
    {
        protected ArrayList data;

        public StructureBuilder()
        {
            this.Reset();
        }

        public abstract object Read(BinaryReader br);
        public void Reset()
        {
            this.data = new ArrayList();
        }

        public abstract void Write(BinaryWriter bw, object o);

        public object[] Data
        {
            get
            {
                return this.data.ToArray();
            }
        }

        public abstract int Length { get; }
    }
}

