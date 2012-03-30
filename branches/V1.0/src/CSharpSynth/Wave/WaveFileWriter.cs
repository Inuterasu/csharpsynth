﻿using System;
using System.IO;

namespace CSharpSynth.Wave
{
    public class WaveFileWriter
    {
        //--Variables
        private BinaryWriter BW;
        private string ftemp;
        private string fname;
        private Int32 length;
        private int channels;
        private int bits;
        private int sRate;
        //--Public Methods
        public WaveFileWriter(int sampleRate, int channels, int bitsPerSample, string filename)
        {
            FileStream fs = File.Create(Path.GetDirectoryName(filename) + "RawWaveData_1tmp");
            BW = new System.IO.BinaryWriter(fs);
            ftemp = fs.Name;
            fname = filename;
            this.channels = channels;
            bits = bitsPerSample;
            sRate = sampleRate;
        }
        public void Write(byte[] buffer)
        {
            BW.Write(buffer);
            length += buffer.Length;
        }
        public void Close()
        {
            BW.Close();
			/* There is no Dispose() available in mono project */
            //BW.Dispose();
            BinaryWriter bw2 = new BinaryWriter(File.OpenWrite(fname));
            bw2.Write((Int32)1179011410);
            bw2.Write((Int32)44 + length - 8);
            bw2.Write((Int32)1163280727);
            bw2.Write((Int32)544501094);
            bw2.Write((Int32)16);
            bw2.Write((Int16)1);
            bw2.Write((Int16)channels);
            bw2.Write((Int32)sRate);
            bw2.Write((Int32)(sRate * channels * (bits / 8)));
            bw2.Write((Int16)(channels * (bits / 8)));
            bw2.Write((Int16)bits);
            bw2.Write((Int32)1635017060);
            bw2.Write((Int32)length);
            BinaryReader br = new BinaryReader(PlatformHelper.StreamLoad(ftemp));
            for (int x = 0; x < length; x++)
                bw2.Write(br.ReadByte());
            br.Close();
            bw2.Close();
            File.Delete(ftemp);
        }
    }
}
