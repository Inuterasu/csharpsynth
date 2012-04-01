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
        private void ShortMessage(int aCommand, int aData1, int aData2)
        {
			
			int channel = (aCommand & 0xF);
			int command = (aCommand >> 4);
			
			switch (command)
            {
                case 0x08: //NoteOff
                  	NoteOff(channel, aData1);
	                break;
                case 0x09: //NoteOn
                	if(aData2 == 0) NoteOff(channel, aData1);
					else NoteOn(channel, aData1, aData2, instruments_[channel]);
                    break;
                case 0x0A: //NoteAftertouch
                    break;
                case 0x0B: //Controller
                    {				
						switch(aData1){
                        case 0x7B: //Note Off All
							NoteOffAll(true);						
							break;
                        case 0x07: //Channel Volume
							volPositions_[channel] = MidiHelper.GetLogarithmicVolume(aData2);						
							break;
                        case 0x0A: //Pan
							panPositions_[channel] = (aData2 - 64) == 63 ? 1.00f : (aData2 - 64) / 64.0f;						
							break;
                        case 0x01: //Modulation
							vibraPositions_[channel] = (aData2 / 127.0) / 20.0;						
							break;
                        case 0x64: //Fine Select
                            RPF[channel] = (byte)aData2;
							break;
                        case 0x65: //Coarse Select
                            RPC[channel] = (byte)aData2;
							break;
                        case 0x06: // DataEntry Coarse
                            if (RPC[channel] == 0)//change semitone
                                pitchWheelSemitoneRange_[channel] = pitchWheelSemitoneRange_[channel] - ((int)pitchWheelSemitoneRange_[channel]) + aData2;                          
                            break;
                        case 0x26: // DataEntry Fine
                            if (RPF[channel] == 0)//change cents
                                pitchWheelSemitoneRange_[channel] = ((int)pitchWheelSemitoneRange_[channel]) + (aData2 / 100.0);
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
                    instruments_[channel] = aData1;
                    break;
                case 0x0D: //Channel Aftertouch
                    break;
                case 0x0E: //pitch bend
                    //pitchbend is -1 to 1 and effects tune in the semitone range given by the pitchwheel
                    int s = ((aData1 << 7) | aData2); // 0 to 16383
                    tunePositions_[channel] = ((s - 8192.0) / 8192.0) * pitchWheelSemitoneRange_[channel];
                    break;
                default:
                    return;
			}
		}
	}
}