namespace CSharpSynth.SoundFont
{
    using System;
    using System.IO;

    public class SoundFont
    {
        private InfoChunk info;
        private PresetsChunk presetsChunk;
        private SampleDataChunk sampleData;

        public SoundFont(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                RiffChunk topLevelChunk = RiffChunk.GetTopLevelChunk(new BinaryReader(stream));
                if (!(topLevelChunk.ChunkID == "RIFF"))
                {
                    throw new ApplicationException("Not a RIFF file");
                }
                string str = topLevelChunk.ReadChunkID();
                if (str != "sfbk")
                {
                    throw new ApplicationException(string.Format("Not a SoundFont ({0})", str));
                }
                RiffChunk nextSubChunk = topLevelChunk.GetNextSubChunk();
                if (nextSubChunk.ChunkID != "LIST")
                {
                    throw new ApplicationException(string.Format("Not info list found ({0})", nextSubChunk.ChunkID));
                }
                this.info = new InfoChunk(nextSubChunk);
                RiffChunk chunk = topLevelChunk.GetNextSubChunk();
                this.sampleData = new SampleDataChunk(chunk);
                chunk = topLevelChunk.GetNextSubChunk();
                this.presetsChunk = new PresetsChunk(chunk);
            }
        }
		
		public SoundFont(Stream stream)
        {
            RiffChunk topLevelChunk = RiffChunk.GetTopLevelChunk(new BinaryReader(stream));
            if (!(topLevelChunk.ChunkID == "RIFF"))
            {
                throw new ApplicationException("Not a RIFF file");
            }
            string str = topLevelChunk.ReadChunkID();
            if (str != "sfbk")
            {
                throw new ApplicationException(string.Format("Not a SoundFont ({0})", str));
            }
            RiffChunk nextSubChunk = topLevelChunk.GetNextSubChunk();
            if (nextSubChunk.ChunkID != "LIST")
            {
                throw new ApplicationException(string.Format("Not info list found ({0})", nextSubChunk.ChunkID));
            }
            this.info = new InfoChunk(nextSubChunk);
            RiffChunk chunk = topLevelChunk.GetNextSubChunk();
            this.sampleData = new SampleDataChunk(chunk);
            chunk = topLevelChunk.GetNextSubChunk();
            this.presetsChunk = new PresetsChunk(chunk);
        }

        public override string ToString()
        {
            return string.Format("Info Chunk:\r\n{0}\r\nPresets Chunk:\r\n{1}", this.info, this.presetsChunk);
        }

        public InfoChunk FileInfo
        {
            get
            {
                return this.info;
            }
        }

        public Instrument[] Instruments
        {
            get
            {
                return this.presetsChunk.Instruments;
            }
        }

        public Preset[] Presets
        {
            get
            {
                return this.presetsChunk.Presets;
            }
        }

        public byte[] SampleData
        {
            get
            {
                return this.sampleData.SampleData;
            }
        }

        public SampleHeader[] SampleHeaders
        {
            get
            {
                return this.presetsChunk.SampleHeaders;
            }
        }
    }
}

