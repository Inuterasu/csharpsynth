//Filter from: http://www.musicdsp.org
//BiQuad highpass filter by Robert Bristow-Johnson
//link1:http://www.musicdsp.org/files/Audio-EQ-Cookbook.txt
//link2:http://www.musicdsp.org/archive.php?classid=3
using System;

namespace CSharpSynth.Wave.DSP
{
    public class BiQuadHighPass
    {
        //--Variables
        private float[,] xbuffer;
        private float[,] ybuffer;
        private float b0da0, b1da0, b2da0, a1da0, a2da0; 
        private int channels;
        //--Public Methods
        public BiQuadHighPass(int channels, double cornerfrequency, double Q)
        {
            this.channels = channels;
            xbuffer = new float[channels, 2];
            ybuffer = new float[channels, 2];
            ChangeFilterConstants(cornerfrequency, Q);
        }
        public void ChangeFilterConstants(double cornerfrequency, double Q)
        {
            double w0 = 2.0 * Math.PI * cornerfrequency;
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
            Array.Clear(xbuffer, 0, channels * xbuffer.GetLength(1));
            Array.Clear(ybuffer, 0, channels * ybuffer.GetLength(1));
        }
        public void ApplyFilter(float[,] inputBuffer)
        {
            int bufferlength = inputBuffer.GetLength(1);
            float sample;
            for (int x = 0; x < channels; x++)
            {
                for (int x2 = 0; x2 < bufferlength; x2++)
                {
                    sample = inputBuffer[x, x2];
                    inputBuffer[x, x2] = (b0da0) * sample + (b1da0) * xbuffer[x, 0] + (b2da0) * xbuffer[x, 1] - (a1da0) * ybuffer[x, 0] - (a2da0) * ybuffer[x, 1];
                    xbuffer[x, 1] = xbuffer[x, 0]; //n - 2
                    xbuffer[x, 0] = sample;        //n - 1
                    ybuffer[x, 1] = ybuffer[x, 0];      //n - 2
                    ybuffer[x, 0] = inputBuffer[x, x2]; //n - 1
                }
            }
        }
        public float ApplyFilter(int channel, float sample)
        {
            float sample2 = sample;
            sample = (b0da0) * sample + (b1da0) * xbuffer[channel, 0] + (b2da0) * xbuffer[channel, 1] - (a1da0) * ybuffer[channel, 0] - (a2da0) * ybuffer[channel, 1];
            xbuffer[channel, 1] = xbuffer[channel, 0]; //n - 2
            xbuffer[channel, 0] = sample2;             //n - 1
            ybuffer[channel, 1] = ybuffer[channel, 0]; //n - 2
            ybuffer[channel, 0] = sample;              //n - 1
            return sample;
        }
        //--Public Static
        //1. normal process with no volume correction
        public static float[,] OfflineProcess(double cornerfrequency, double Q, float[,] data)
        {
            //Set Up Constants
            double w0 = 2 * Math.PI * cornerfrequency;
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
            int length = data.GetLength(1);
            if (length < 2)
                return data;
            float[,] buffer = new float[data.GetLength(0), length];
            for (int x = 0; x < data.GetLength(0); x++)
            {
                buffer[x, 0] = (float)((b0da0) * data[x, 0]);
                buffer[x, 1] = (float)((b0da0) * data[x, 1] + (b1da0) * data[x, 0] - (a1da0) * buffer[x, 0]);
                for (int n = 2; n < length; n++)
                {
                    buffer[x, n] = (float)((b0da0) * data[x, n] + (b1da0) * data[x, n - 1] + (b2da0) * data[x, n - 2] - (a1da0) * buffer[x, n - 1] - (a2da0) * buffer[x, n - 2]);
                }
            }
            return buffer;
        }
        //2. process with extra loop to correct volume loss during filtering
        public static float[,] OfflineProcessWithVolumeCorrection(double cornerfrequency, double Q, float[,] data)
        {
            //Set Up Constants
            double w0 = 2 * Math.PI * cornerfrequency;
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
            float abs;
            float highestOLD = 0.0f;
            float highestNew = 0.0f;
            //-----
            int length = data.GetLength(1);
            if (length < 2)
                return data;
            float[,] buffer = new float[data.GetLength(0), length];
            for (int x = 0; x < data.GetLength(0); x++)
            {
                buffer[x, 0] = (float)((b0da0) * data[x, 0]);
                buffer[x, 1] = (float)((b0da0) * data[x, 1] + (b1da0) * data[x, 0] - (a1da0) * buffer[x, 0]);
                abs = Math.Abs(data[x, 0]);
                if (abs > highestOLD)
                    highestOLD = abs;
                abs = Math.Abs(data[x, 1]);
                if (abs > highestOLD)
                    highestOLD = abs;
                abs = Math.Abs(buffer[x, 0]);
                if (abs > highestNew)
                    highestNew = abs;
                abs = Math.Abs(buffer[x, 1]);
                if (abs > highestNew)
                    highestNew = abs;
                for (int n = 2; n < length; n++)
                {
                    abs = Math.Abs(data[x, n]);
                    if (abs > highestOLD)
                        highestOLD = abs;
                    
                    buffer[x, n] = (float)((b0da0) * data[x, n] + (b1da0) * data[x, n - 1] + (b2da0) * data[x, n - 2] - (a1da0) * buffer[x, n - 1] - (a2da0) * buffer[x, n - 2]);
                    
                    abs = Math.Abs(buffer[x, n]);
                    if (abs > highestNew)
                        highestNew = abs;
                }
                //normalize volume
                abs = highestOLD / highestNew;
                for (int n = 0; n < length; n++)
                {
                    buffer[x, n] *= abs;
                }
            }
            return buffer;
        }
    }
}
