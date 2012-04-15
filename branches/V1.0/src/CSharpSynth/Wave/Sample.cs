using System;
using System.IO;

namespace CSharpSynth.Wave
{
    public class Sample
    {
        //--Variables
        private float[,] data;
        private int sampleRate;
        private int originalRate;
        private string name;
        //--Public Methods
        public Sample(string filename)
        {
            if (PlatformHelper.FileExists(filename) == false)
                throw new FileNotFoundException("Sample not found: " + Path.GetFileNameWithoutExtension(filename));
            name = Path.GetFileNameWithoutExtension(filename);
            WaveFileReader WaveReader = new WaveFileReader(filename);
            IChunk[] chunks = WaveReader.ReadAllChunks();
            WaveReader.Close(); //Close the reader and the underlying stream.
            DataChunk dChunk = null;
            FormatChunk fChunk = null;
            for (int x = 0; x < chunks.Length; x++)
            {
                if (chunks[x].GetChunkType() == WaveHelper.WaveChunkType.Format)
                    fChunk = (FormatChunk)chunks[x];
                else if (chunks[x].GetChunkType() == WaveHelper.WaveChunkType.Data)
                    dChunk = (DataChunk)chunks[x];
            }
            if (fChunk == null || dChunk == null)
                throw new ArgumentException("Wave file is in unrecognized format!");
            if (fChunk.wBitsPerSample != 16)
                WaveHelper.ChangeBitsPerSample(fChunk, dChunk, 16);
            int channels = fChunk.nChannels;
            sampleRate = fChunk.nSamplesPerSec;
            originalRate = sampleRate;
            data = WaveHelper.GetSampleData(fChunk, dChunk);
        }
        public Sample(int sampleRate)
        {
            data = new float[2, 1];
            data[0, 0] = 0.0f;
            data[1, 0] = 0.0f;
            this.sampleRate = sampleRate;
            originalRate = sampleRate;
            name = "";
        }
        public Sample(string name, int sampleRate, float[,] data)
        {
            this.data = data;
            this.sampleRate = sampleRate;
            this.originalRate = sampleRate;
            this.name = name;
        }
        public override bool Equals(object obj)
        {
            Sample s = obj as Sample;
            if (s != null && this.name.Equals(s.name) && (this.SamplesPerChannel == s.SamplesPerChannel)
                && (this.NumberofChannels == s.NumberofChannels) && (this.sampleRate == s.sampleRate))
                return true;
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public float getSample(int channel, int index)
        {
            return data[channel, index];
        }
        public void setSample(int channel, int index, float value)
        {
            data[channel, index] = value;
        }
        public float[,] getAllSampleData()
        {
            return data;
        }
        public void setAllSampleData(float[,] value)
        {
            data = value;
        }
        public int getMemoryUseage()
        {
            return sizeof(float) * data.GetLength(0) * data.GetLength(1);
        }
        //--Public Properties
        public int NumberofChannels
        {
            get { return data.GetLength(0); }
        }
        public string Name
        {
            get { return name; }
        }
        public bool isDualChannel
        {
            get { return data.GetLength(0) == 2; }
        }
        public int SampleRate
        {
            get { return sampleRate; }
            set { sampleRate = value; }
        }
        public int OriginalSampleRate
        {
            get { return originalRate; }
        }
        public int SamplesPerChannel
        {
            get { return data.GetLength(1); }
        }
    }
}
