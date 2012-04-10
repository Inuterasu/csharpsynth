using System;

namespace CSharpSynth.Wave.DSP
{
    public class BiQuadHighPass
    {
         //--Variables
        private float[,] temps;
        private float b0da0, b1da0, b2da0, a1da0, a2da0; 
        private int channels;
        //--Public Methods
        public BiQuadHighPass(float sampleRate, int channels, float cornerfrequency)
        {
            this.channels = channels;
            temps = new float[channels, 2];
            ChangeFilterConstants(sampleRate, cornerfrequency, 1.0f);
        }
        public void ChangeFilterConstants(float sampleRate, float cornerFrequency, float Q)
        {
            double w0 = 2.0 * Math.PI * cornerFrequency / sampleRate;
            double cosw0 = Math.Cos(w0);
            double alpha = Math.Sin(w0) / (2.0 * Q);
            double a0, a1, a2, b0, b1, b2;
            b0 = ((1.0 + cosw0) / 2.0);
            b1 = (-1.0 - cosw0);
            b2 = ((1.0 + cosw0) / 2.0);
            a0 = (1.0 + alpha);
            a1 = (-2.0 * cosw0);
            a2 = (1.0 - alpha);
            b0da0 = (float)(b0 / a0);
            b1da0 = (float)(b1 / a0);
            b2da0 = (float)(b2 / a0);
            a1da0 = (float)(a1 / a0);
            a2da0 = (float)(a2 / a0);
        }
        public void Reset()
        {
            Array.Clear(temps, 0, channels * temps.GetLength(1));
        }
        public void ApplyFilter(float[,] inputBuffer)
        {
            int bufferlength = inputBuffer.GetLength(1);
            for (int x = 0; x < channels; x++)
            {
                for (int x2 = 0; x2 < bufferlength; x2++)
                {
                    inputBuffer[x, x2] = (b0da0) * inputBuffer[x, x2] + (b1da0) * temps[x, 0] + (b2da0) * temps[x, 1] - (a1da0) * temps[x, 0] - (a2da0) * temps[x, 1];
                    temps[x, 1] = temps[x, 0];
                    temps[x, 0] = inputBuffer[x, x2];
                }
            }
        }
        public float ApplyFilter(int channel, float sample)
        {
            sample = (b0da0) * sample + (b1da0) * temps[channel, 0] + (b2da0) * temps[channel, 1] - (a1da0) * temps[channel, 0] - (a2da0) * temps[channel, 1];
            temps[channel, 1] = temps[channel, 0];
            temps[channel, 0] = sample;
            return sample;
        }
        //--Public Static
        public static float[,] OfflineProcess(double sampleRate, double cornerFrequency, double Q, float[,] data)
        {
            //Set Up Constants
            double w0 = 2 * Math.PI * cornerFrequency / sampleRate;
            double cosw0 = Math.Cos(w0);
            double alpha = Math.Sin(w0) / (2 * Q);
            double b0, b1, b2, a0, a1, a2;
            b0 = ((1.0 + cosw0) / 2.0);
            b1 = (-1.0 - cosw0);
            b2 = ((1.0 + cosw0) / 2.0);
            a0 = (1.0 + alpha);
            a1 = (-2.0 * cosw0);
            a2 = (1.0 - alpha);
            double b0da0, b1da0, b2da0, a1da0, a2da0; 
            b0da0 = (b0 / a0);
            b1da0 = (b1 / a0);
            b2da0 = (b2 / a0);
            a1da0 = (a1 / a0);
            a2da0 = (a2 / a0);
            //-----
            //Apply the filter
            int length = data.GetLength(1);
            float[,] buffer = new float[data.GetLength(0), length];
            for (int x = 0; x < data.GetLength(0); x++)
            {
                for (int n = 0; n < length; n++)
                {
                    if (n == 0)
                    {
                        buffer[x, n] = (float)((b0da0) * data[x, n] + (b1da0) * 0 + (b2da0) * 0 - (a1da0) * 0 - (a2da0) * 0);
                    }
                    else if (n == 1)
                    {
                        buffer[x, n] = (float)((b0da0) * data[x, n] + (b1da0) * data[x, n - 1] + (b2da0) * 0 - (a1da0) * buffer[x, n - 1] - (a2da0) * 0);
                    }
                    else
                    {
                        buffer[x, n] = (float)((b0da0) * data[x, n] + (b1da0) * data[x, n - 1] + (b2da0) * data[x, n - 2] - (a1da0) * buffer[x, n - 1] - (a2da0) * buffer[x, n - 2]);
                    }
                }
            }
            return buffer;
        }
    }
}
