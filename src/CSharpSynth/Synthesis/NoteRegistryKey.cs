using System;

namespace CSharpSynth.Synthesis
{
    public struct NoteRegistryKey
    {
        //--Variables
        private readonly byte note;
        private readonly byte channel;
        
		//--Public Properties
        public byte Note { get { return note; } }
        public byte Channel { get { return channel; } }
		
        //--Public Methods
        public NoteRegistryKey(byte channel, byte note)
        {
            this.note = note;
            this.channel = channel;
        }
        public override bool Equals(object obj)
        {
            NoteRegistryKey r = (NoteRegistryKey)obj;
            return r.channel == this.channel && r.note == this.note;
        }
        public override int GetHashCode()
        {
            return BitConverter.ToInt32(new byte[4] { note, channel, 0, 0 }, 0);
        }
    }
}
