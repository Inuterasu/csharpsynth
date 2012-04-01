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

using CSharpSynth.Midi;
using System.Collections.Generic;

namespace CSharpSynth.Synthesis
{
    public partial class StreamSynthesizer
    {	
        private struct ShortMessageStruct
	    {
	        public int command;
	        public int data1;
	        public int data2;
	     	
	        public ShortMessageStruct(int command, int data1, int data2)
	        {
	            this.command = command;
	            this.data1 = data1;
	            this.data2 = data2;
	        }
	    }
		
		Stack<ShortMessageStruct> servermessages_ = new Stack<ShortMessageStruct>(400);
		
		public void ShortMessage(int aCommand, int data1, int data2)
        {
			servermessages_.Push(new ShortMessageStruct{command = aCommand, data1 = shortMessage.data1, data2 = shortMessage.data2});
		}
		
		
		ShortMessageStruct shortMessage;
		private void ProcessServerMessages(){
			while(servermessages_.Count > 0){
				shortMessage = servermessages_.Pop();
				int channel = (shortMessage.command & 0xF);
				int command = (shortMessage.command >> 4);
				
				switch (command)
	            {
	                case 0x08: //NoteOff
	                  	NoteOff(channel, shortMessage.data1);
		                break;
	                case 0x09: //NoteOn
	                	if(shortMessage.data2 == 0) NoteOff(channel, shortMessage.data1);
						else NoteOn(channel, shortMessage.data1, shortMessage.data2, instruments_[channel]);
	                    break;
	                case 0x0A: //NoteAftertouch
	                    break;
	                case 0x0B: //Controller
	                    {				
							switch(shortMessage.data1){
	                        case 0x7B: //Note Off All
								NoteOffAll(true);						
								break;
	                        case 0x07: //Channel Volume
								volPositions_[channel] = MidiHelper.GetLogarithmicVolume(shortMessage.data2);						
								break;
	                        case 0x0A: //Pan
								panPositions_[channel] = (shortMessage.data2 - 64) == 63 ? 1.00f : (shortMessage.data2 - 64) / 64.0f;						
								break;
	                        case 0x01: //Modulation
								vibraPositions_[channel] = (shortMessage.data2 / 127.0) / 20.0;						
								break;
	                        case 0x64: //Fine Select
	                            RPF[channel] = (byte)shortMessage.data2;
								break;
	                        case 0x65: //Coarse Select
	                            RPC[channel] = (byte)shortMessage.data2;
								break;
	                        case 0x06: // DataEntry Coarse
	                            if (RPC[channel] == 0)//change semitone
	                                pitchWheelSemitoneRange_[channel] = pitchWheelSemitoneRange_[channel] - ((int)pitchWheelSemitoneRange_[channel]) + shortMessage.data2;                          
	                            break;
	                        case 0x26: // DataEntry Fine
	                            if (RPF[channel] == 0)//change cents
	                                pitchWheelSemitoneRange_[channel] = ((int)pitchWheelSemitoneRange_[channel]) + (shortMessage.data2 / 100.0);
	                            break;
	                        case 0x79: // Reset All
	                            resetSynthControls();
	                            break;
	                        default:
	                            return;
							}
	                    }
	                    break;
	                case 0x0C: //Program Change
	                    instruments_[channel] = shortMessage.data1;
	                    break;
	                case 0x0D: //Channel Aftertouch
	                    break;
	                case 0x0E: //pitch bend
	                    //pitchbend is -1 to 1 and effects tune in the semitone range given by the pitchwheel
	                    int s = ((shortMessage.data1 << 7) | shortMessage.data2); // 0 to 16383
	                    tunePositions_[channel] = ((s - 8192.0) / 8192.0) * pitchWheelSemitoneRange_[channel];
	                    break;
	                default:
	                    return;
				}
			}
		}
	}
}