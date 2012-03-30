/*
 * This partial class is intedet to simulate MIDI Soft Server
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */

using System;
using System.Collections.Generic;
using CSharpSynth.Banks;
using CSharpSynth.Sequencer;
using CSharpSynth.Effects;

namespace CSharpSynth.Synthesis
{
    public partial class StreamSynthesizer
    {
		byte[] volumes = new byte[16];
		byte[] pans = new byte[16];
		byte[] pitches = new byte[16];
		
		//here all the parsing login needed for synth to play its sounds//
		public void ShortMessage(byte Command, byte Data1, byte Data2){
			
		}
	}
}