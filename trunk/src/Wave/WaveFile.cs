namespace CSharpSynth.Wave
{
    public class WaveFile
    {
        //--Variables
        private IChunk[] waveChunks;
        private DataChunk dataChunk;
        private FormatChunk fmtChunk;
        //--Public Methods
        public WaveFile(IChunk[] WaveChunks)
        {
            this.waveChunks = WaveChunks;
            this.dataChunk = (DataChunk)GetChunk(WaveHelper.WaveChunkType.Data);
            this.fmtChunk = (FormatChunk)GetChunk(WaveHelper.WaveChunkType.Format);
        }
        public IChunk GetChunk(WaveHelper.WaveChunkType ChunkType)
        {
            for (int x = 0; x < waveChunks.Length; x++)
            {
                if (waveChunks[x].GetChunkType() == ChunkType)
                    return waveChunks[x];
            }
            return null;
        }
        public IChunk GetChunk(int startIndex, WaveHelper.WaveChunkType ChunkType)
        {
            if (startIndex >= waveChunks.Length)
                return null;
            for (int x = startIndex; x < waveChunks.Length; x++)
            {
                if (waveChunks[x].GetChunkType() == ChunkType)
                    return waveChunks[x];
            }
            return null;
        }
        public DataChunk Data
        {
            get { return dataChunk; }
        }
        public FormatChunk Format
        {
            get { return fmtChunk; }
        }
        public byte[] SampleData
        {
            get { return dataChunk.sampled_data; }
        }
        public int NumberOfChunks
        {
            get { return waveChunks.Length; }
        }
    }
}
