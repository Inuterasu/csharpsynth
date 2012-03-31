/*
 * This partial class is intended to simulate MIDI Soft Server
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
using CSharpSynth.Midi;

namespace CSharpSynth.Synthesis
{
    public partial class StreamSynthesizer
    {
		byte[] instruments = new byte[16];
//		byte[] volumes = new byte[16];
//		byte[] pans = new byte[16];
//		byte[] pitches = new byte[16];
		
		//here all the parsing login needed for synth to play its sounds//
		int channel = 0;
		int command = 0;
		
		public void ShortMessage(byte aCommand, byte aData1, byte aData2){
			
			channel = (aCommand & 0xF);
			command = (aCommand >> 4);
			
			if(!System.Enum.IsDefined(typeof(MidiHelper.MidiChannelEvent),(MidiHelper.MidiChannelEvent)command)) return;
			
			switch ((MidiHelper.MidiChannelEvent)command)
            {
                case MidiHelper.MidiChannelEvent.Note_Off:
                  	NoteOff(channel, aData1);
	                break;
                case MidiHelper.MidiChannelEvent.Note_On:
                	if(aData2 == 0) NoteOff(channel, aData1);
					else NoteOn(channel, aData1, aData2, instruments[channel]);
                    break;
                case MidiHelper.MidiChannelEvent.Note_Aftertouch:
                    break;
                case MidiHelper.MidiChannelEvent.Controller: 
                    {
						if(!System.Enum.IsDefined(typeof(MidiHelper.ControllerType),(MidiHelper.ControllerType)aData1)) return;
				
						switch((MidiHelper.ControllerType)aData1){
						case MidiHelper.ControllerType.AllNotesOff:
							NoteOffAll(true);						
							break;
						case MidiHelper.ControllerType.MainVolume:
							VolPositions[channel] = MidiHelper.GetLogarithmicVolume(aData2);						
							break;
						case MidiHelper.ControllerType.Pan:
							PanPositions[channel] = (aData2 - 64) == 63 ? 1.00f : (aData2 - 64) / 64.0f;						
							break;
						case MidiHelper.ControllerType.Modulation:
							VibratoPositions[channel] = (aData2 / 127.0) / 20.0;						
							break;
						case MidiHelper.ControllerType.RegisteredParameterLSB:
                            RegisteredParameterFine[channel] = aData2;
							break;
						case MidiHelper.ControllerType.RegisteredParameterMSB:
							RegisteredParameterCoarse[channel] = aData2;
							break;
                        case MidiHelper.ControllerType.DataEntry:
                            if (RegisteredParameterCoarse[channel] == 0)
                            	PitchWheelPositions[channel] = PitchWheelPositions[channel] - ((int)PitchWheelPositions[channel]) + aData2;
                            
                            break;
						case MidiHelper.ControllerType.DataEntryLSB:
                            if (RegisteredParameterFine[channel] == 0)
                                    PitchWheelPositions[channel] = ((int)PitchWheelPositions[channel]) + (aData2 / 100.0);
                            break;
                        case MidiHelper.ControllerType.ResetControllers:
                            resetSynthControls();
                            break;
						}
						
                    }
                    break;
                case MidiHelper.MidiChannelEvent.Program_Change:
                    instruments[channel] = aData1;
                    break;
                case MidiHelper.MidiChannelEvent.Channel_Aftertouch:
                    break;
                case MidiHelper.MidiChannelEvent.Pitch_Bend:
                    break;
			}
		}
	}
}