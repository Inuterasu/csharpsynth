namespace CSharpSynth.SoundFont
{
    using System;
    using System.IO;
    using System.Text;

    internal class SampleHeaderBuilder : StructureBuilder
    {
        public override object Read(BinaryReader br)
        {
            SampleHeader header = new SampleHeader();
            string str = Encoding.ASCII.GetString(br.ReadBytes(20));
            if (str.IndexOf('\0') >= 0)
            {
                str = str.Substring(0, str.IndexOf('\0'));
            }
            header.SampleName = str;
            header.Start = br.ReadUInt32();
            header.End = br.ReadUInt32();
            header.StartLoop = br.ReadUInt32();
            header.EndLoop = br.ReadUInt32();
            header.SampleRate = br.ReadUInt32();
            header.OriginalPitch = br.ReadByte();
            header.PitchCorrection = br.ReadSByte();
            header.SampleLink = br.ReadUInt16();
            header.SFSampleLink = (SFSampleLink) br.ReadUInt16();
            base.data.Add(header);
            return header;
        }

        internal void RemoveEOS()
        {
            base.data.RemoveAt(base.data.Count - 1);
        }

        public override void Write(BinaryWriter bw, object o)
        {
            SampleHeader header = (SampleHeader) o;
        }

        public override int Length
        {
            get
            {
                return 0x2e;
            }
        }

        public SampleHeader[] SampleHeaders
        {
            get
            {
                return (SampleHeader[]) base.data.ToArray(typeof(SampleHeader));
            }
        }
    }
}

