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
		private Queue<ShortMessageStruct> servermessages_ = new Queue<ShortMessageStruct>(400);
		//Add msg to queue
		public void AddShortMessage(int aCommand, int aData1, int aData2)
        {
			servermessages_.Enqueue(new ShortMessageStruct{command = aCommand, data1 = aData1, data2 = aData2});
		}		
		//Process all msgs in queue
        private void ProcessAllShortMessages()
        {
            ShortMessageStruct shortMessage;
            while(servermessages_.Count > 0)
            {
                shortMessage = servermessages_.Dequeue();
				ProcessShortMessage(shortMessage);
			}
		}
        //short msg sequencer process - process single msg
        private void ProcessShortMessage(ShortMessageStruct shortMessage)
        {
            int channel = (shortMessage.command & 0xF);
            int command = (shortMessage.command >> 4);

            switch (command)
            {
                case 0x08: //NoteOff
                    NoteOff(channel, shortMessage.data1);
                    break;
                case 0x09: //NoteOn
                    if (shortMessage.data2 == 0) NoteOff(channel, shortMessage.data1);
                    else NoteOn(channel, shortMessage.data1, shortMessage.data2, instruments_[channel]);
                    break;
                case 0x0A: //NoteAftertouch
                    break;
                case 0x0B: //Controller
                    {
                        switch (shortMessage.data1)
                        {
                            case 0x7B: //Note Off All
                                NoteOffAll(true);
                                break;
                            case 0x07: //Channel Volume
                                volPositions_[channel] = shortMessage.data2 / 127.0f;
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
        //sample sequencer process - process single msg
        public void ProcessSampleMessage(MidiEvent midiEvent) 
        {
            if (midiEvent.midiChannelEvent != MidiHelper.MidiChannelEvent.None)
            {
                switch (midiEvent.midiChannelEvent)
                {
                    case MidiHelper.MidiChannelEvent.Program_Change:
                        if (midiEvent.channel != 9)
                        {
                            if (midiEvent.parameter1 < bank.InstrumentCount)
                                instruments_[midiEvent.channel] = midiEvent.parameter1;
                        }
                        else //its the drum channel
                        {
                            if (midiEvent.parameter1 < bank.DrumCount)
                                instruments_[midiEvent.channel] = midiEvent.parameter1;
                        }
                        break;
                    case MidiHelper.MidiChannelEvent.Note_On:
                        NoteOn(midiEvent.channel, midiEvent.parameter1, midiEvent.parameter2, instruments_[midiEvent.channel]);
                        break;
                    case MidiHelper.MidiChannelEvent.Note_Off:
                        NoteOff(midiEvent.channel, midiEvent.parameter1);
                        break;
                    case MidiHelper.MidiChannelEvent.Pitch_Bend:
                        //Store PitchBend as the # of semitones higher or lower
                        tunePositions_[midiEvent.channel] = (double)midiEvent.Parameters[1] * pitchWheelSemitoneRange_[midiEvent.channel];
                        break;
                    case MidiHelper.MidiChannelEvent.Controller:
                        switch (midiEvent.GetControllerType())
                        {
                            case MidiHelper.ControllerType.AllNotesOff:
                                NoteOffAll(true);
                                break;
                            case MidiHelper.ControllerType.MainVolume:
                                volPositions_[midiEvent.channel] = midiEvent.parameter2 / 127f;
                                break;
                            case MidiHelper.ControllerType.Pan:
                                panPositions_[midiEvent.channel] = (midiEvent.parameter2 - 64) == 63 ? 1.00f : (midiEvent.parameter2 - 64) / 64.0f;
                                break;
                            case MidiHelper.ControllerType.Modulation:
                                vibraPositions_[midiEvent.channel] = (midiEvent.parameter2 / 127.0) / 20.0;
                                break;
                            case MidiHelper.ControllerType.RegisteredParameterLSB:
                                RPF[midiEvent.channel] = midiEvent.parameter2;
                                break;
                            case MidiHelper.ControllerType.RegisteredParameterMSB:
                                RPC[midiEvent.channel] = midiEvent.parameter2;
                                break;
                            case MidiHelper.ControllerType.DataEntry:
                                if (RPC[midiEvent.channel] == 0)//change semitone
                                    pitchWheelSemitoneRange_[midiEvent.channel] = pitchWheelSemitoneRange_[midiEvent.channel] - ((int)pitchWheelSemitoneRange_[midiEvent.channel]) + midiEvent.parameter2;
                                break;
                            case MidiHelper.ControllerType.DataEntryLSB:
                                if (RPF[midiEvent.channel] == 0)//change cents
                                    pitchWheelSemitoneRange_[midiEvent.channel] = ((int)pitchWheelSemitoneRange_[midiEvent.channel]) + (midiEvent.parameter2 / 100.0);
                                break;
                            case MidiHelper.ControllerType.ResetControllers:
                                resetSynthControls();
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (midiEvent.midiMetaEvent)
                {
                    case MidiHelper.MidiMetaEvent.Tempo:
                        //_MidiFile.BeatsPerMinute = MidiHelper.MicroSecondsPerMinute / System.Convert.ToUInt32(midiEvent.Parameters[0]);
                        break;
                    default:
                        break;
                }
            }
        }
	}
}