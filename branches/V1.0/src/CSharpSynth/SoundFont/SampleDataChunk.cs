namespace NAudio.SoundFont
{
    using System;

    internal class SampleDataChunk
    {
        private byte[] sampleData;

        public SampleDataChunk(RiffChunk chunk)
        {
            string str = chunk.ReadChunkID();
            if (str != "sdta")
            {
                throw new ApplicationException(string.Format("Not a sample data chunk ({0})", str));
            }
            this.sampleData = chunk.GetData();
        }

        public byte[] SampleData
        {
            get
            {
                return this.sampleData;
            }
        }
    }
}

