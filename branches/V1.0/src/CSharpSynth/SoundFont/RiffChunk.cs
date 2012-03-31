namespace NAudio.SoundFont
{
    using System;
    using System.IO;
    using System.Text;

    internal class RiffChunk
    {
        private long dataOffset;
        private string chunkID;
        private uint chunkSize;
        private BinaryReader riffFile;

        private RiffChunk(BinaryReader file)
        {
            this.riffFile = file;
            this.chunkID = "????";
            this.chunkSize = 0;
            this.dataOffset = 0L;
        }

        public byte[] GetData()
        {
            this.riffFile.BaseStream.Position = this.dataOffset;
            byte[] buffer = this.riffFile.ReadBytes((int) this.chunkSize);
            if (buffer.Length != this.chunkSize)
            {
                throw new ApplicationException(string.Format("Couldn't read chunk's data Chunk: {0}, read {1} bytes", this, buffer.Length));
            }
            return buffer;
        }

        public string GetDataAsString()
        {
            byte[] data = this.GetData();
            if (data == null)
            {
                return null;
            }
            string str = Encoding.ASCII.GetString(data);
            if (str.IndexOf('\0') >= 0)
            {
                str = str.Substring(0, str.IndexOf('\0'));
            }
            return str;
        }

        public object GetDataAsStructure(StructureBuilder s)
        {
            this.riffFile.BaseStream.Position = this.dataOffset;
            if (s.Length != this.chunkSize)
            {
                throw new ApplicationException(string.Format("Chunk size is: {0} so can't read structure of: {1}", this.chunkSize, s.Length));
            }
            return s.Read(this.riffFile);
        }

        public object[] GetDataAsStructureArray(StructureBuilder s)
        {
            this.riffFile.BaseStream.Position = this.dataOffset;
            if ((((ulong) this.chunkSize) % ((long) s.Length)) != 0L)
            {
                throw new ApplicationException(string.Format("Chunk size is: {0} not a multiple of structure size: {1}", this.chunkSize, s.Length));
            }
            int num = (int) (((ulong) this.chunkSize) / ((long) s.Length));
            object[] objArray = new object[num];
            for (int i = 0; i < num; i++)
            {
                objArray[i] = s.Read(this.riffFile);
            }
            return objArray;
        }

        public RiffChunk GetNextSubChunk()
        {
            if ((this.riffFile.BaseStream.Position + 8L) < (this.dataOffset + this.chunkSize))
            {
                RiffChunk chunk = new RiffChunk(this.riffFile);
                chunk.ReadChunk();
                return chunk;
            }
            return null;
        }

        public static RiffChunk GetTopLevelChunk(BinaryReader file)
        {
            RiffChunk chunk = new RiffChunk(file);
            chunk.ReadChunk();
            return chunk;
        }

        private void ReadChunk()
        {
            this.chunkID = this.ReadChunkID();
            this.chunkSize = this.riffFile.ReadUInt32();
            this.dataOffset = this.riffFile.BaseStream.Position;
        }

        public string ReadChunkID()
        {
            byte[] bytes = this.riffFile.ReadBytes(4);
            if (bytes.Length != 4)
            {
                throw new ApplicationException("Couldn't read Chunk ID");
            }
            return Encoding.ASCII.GetString(bytes);
        }

        public override string ToString()
        {
            return string.Format("RiffChunk ID: {0} Size: {1} Data Offset: {2}", this.ChunkID, this.ChunkSize, this.DataOffset);
        }

        public long DataOffset
        {
            get
            {
                return this.dataOffset;
            }
        }

        public string ChunkID
        {
            get
            {
                return this.chunkID;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("ChunkID may not be null");
                }
                if (value.Length != 4)
                {
                    throw new ArgumentException("ChunkID must be four characters");
                }
                this.chunkID = value;
            }
        }

        public uint ChunkSize
        {
            get
            {
                return this.chunkSize;
            }
        }
    }
}

