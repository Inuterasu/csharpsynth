namespace CSharpSynth.SoundFont
{
    using System;
    using System.IO;

    internal class SFVersionBuilder : StructureBuilder
    {
        public override object Read(BinaryReader br)
        {
            SFVersion version = new SFVersion {
                Major = br.ReadInt16(),
                Minor = br.ReadInt16()
            };
            base.data.Add(version);
            return version;
        }

        public override void Write(BinaryWriter bw, object o)
        {
            SFVersion version = (SFVersion) o;
            bw.Write(version.Major);
            bw.Write(version.Minor);
        }

        public override int Length
        {
            get
            {
                return 4;
            }
        }
    }
}

