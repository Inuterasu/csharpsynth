using System;
using System.Collections.Generic;
using CSharpSynth.Banks;
using CSharpSynth.Sequencer;
using CSharpSynth.Effects;

namespace CSharpSynth.Synthesis
{
    public partial class StreamSynthesizer
    {
		public void NoteOn(int channel, int note, int velocity, int program)
        {
            // Grab a free voice
            Voice freeVoice = getFreeVoice();
            if (freeVoice == null)
            {
                // If there are no free voices steal an active one.
                freeVoice = getUsedVoice(activeVoices.First.Value.getKey());
                // If there are no voices to steal then leave this method.
                if (freeVoice == null)
                    return;
            }
            // Create a key for this event
            NoteRegistryKey r = new NoteRegistryKey((byte)channel, (byte)note);
            // Get the correct instrument depending if it is a drum or not
            if (channel == 9)
                freeVoice.setInstrument(bank.getInstrument(program, true));
            else
                freeVoice.setInstrument(bank.getInstrument(program, false));
            //Check if key already exists in registry
            if (keyRegistry.ContainsKey(r))
            {//If there are too many of these notes playing then stop the first of them before playing another
                if (keyRegistry[r].Count >= maxnotepoly)
                {
                    keyRegistry[r][0].Stop();
                    keyRegistry[r].RemoveAt(0);
                }
                keyRegistry[r].Add(freeVoice);
            }
            else//The first noteOn of it's own type will create a list for multiple occurences
            {
                List<Voice> Vlist = new List<Voice>(maxnotepoly);
                Vlist.Add(freeVoice);
                keyRegistry.Add(r, Vlist);
            }
            freeVoice.Start(channel, note, velocity);
            activeVoices.AddLast(freeVoice);
        }
		
        public void NoteOff(int channel, int note)
        {
            NoteRegistryKey r = new NoteRegistryKey((byte)channel, (byte)note);
            List<Voice> voice;
            if (keyRegistry.TryGetValue(r, out voice))
            {
                if (voice.Count > 0)
                {
                    voice[0].Stop();
                    voice.RemoveAt(0);
                }
            }
        }
		
        public void NoteOffAll(bool immediate)
        {
            if (keyRegistry.Keys.Count == 0 && activeVoices.Count == 0)
                return;
            LinkedListNode<Voice> node = activeVoices.First;
            while (node != null)
            {
                if (immediate)
                    node.Value.StopImmediately();
                else
                    node.Value.Stop();
                node = node.Next;
            }
            keyRegistry.Clear();
        }

        public void NoteOffAll(bool immediate, int channel)
        {
            LinkedListNode<Voice> node = activeVoices.First;
            while (node != null)
            {
                if (node.Value.Channel == channel)
                {
                    if (immediate)
                        node.Value.StopImmediately();
                    else
                        node.Value.Stop();
                }
                node = node.Next;
            }
        }
	}
}
