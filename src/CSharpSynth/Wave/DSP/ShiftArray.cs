using System;

namespace CSharpSynth.Wave.DSP
{
    public class ShiftArray
    {
        public double[] baseArray;
        public int indexOffset;
        public int shiftwindowSize;
        public ShiftArray(int shiftwindowSize, int totalCapacity)
        {
            this.shiftwindowSize = shiftwindowSize;
            baseArray = new double[totalCapacity];
        }
        public void shiftDown()
        {
            indexOffset++;
            if (indexOffset + shiftwindowSize > baseArray.Length)
            {
                Array.Copy(baseArray, baseArray.Length + 1 - shiftwindowSize, baseArray, 0, shiftwindowSize - 1);
                indexOffset = 0;
            }
        }
        public void reset()
        {
            indexOffset = 0;
            Array.Clear(baseArray, 0, baseArray.Length);
        }
    }
}
