using System;

namespace CSharpSynth.Midi
{
    public static class MidiHelper
    {
        //--Constants
        public const uint MicroSecondsPerMinute = 60000000; //microseconds in a minute
        public const int Max_MidiChannel = 15;
        public const int Min_MidiChannel = 0;
        public const int Drum_Channel = 9;
        public const byte Max_NoteNumber = 127;
        public const byte Min_NoteNumber = 0;
        public const byte Max_Velocity = 127;
        public const byte Min_Velocity = 0;
        public const byte Max_Controller = 127;
        public const byte Min_Controller = 0;
        public const byte Max_GenericParameter = 127;
        public const byte Min_GenericParameter = 0;
        //--Public Methods
        public static float GetLogarithmicVolume(int value)
        {//uses logarithmic method
            if (value == 0)
                return 0.0f;
            return (float)(1.0 - (Math.Log10(value / 127.0) / -2.2));
        }
        //--Enum
        public enum MidiTimeFormat
        {
            TicksPerBeat,
            FamesPerSecond
        }
        public enum MidiChannelEvent
        {
            None 				= int.MaxValue,
            Note_On				= 0x09,
            Note_Off			= 0x08,
            Note_Aftertouch		= 0x0A,
            Controller			= 0x0B,
            Program_Change		= 0x0C,
            Channel_Aftertouch	= 0x0D,
            Pitch_Bend			= 0x0E,
            Unknown 			= int.MaxValue - 1
        }
        public enum ControllerType
        {
            None 						= int.MaxValue,
            BankSelect 					= 0x00,
            Modulation 					= 0x01,
            BreathController 			= 0x02,
            FootController				= 0x04,
            PortamentoTime				= 0x05,
            DataEntry					= 0x06,
            MainVolume					= 0x07,
            Balance						= 0x08,
            Pan							= 0x0A,
            ExpressionController		= 0x0B,
            EffectControl1				= 0x0C,
            EffectControl2				= 0x0D,
            GeneralPurposeController1	= 0x10,
            GeneralPurposeController2	= 0x11,
            GeneralPurposeController3	= 0x12,
            GeneralPurposeController4	= 0x13,
			BankSelectLSB				= 0x20,
            ModulationLSB				= 0x21,
            BreathControllerLSB			= 0x22,
            FootControllerLSB			= 0x24,
            PortamentoTimeLSB			= 0x25,
            DataEntryLSB				= 0x26,
            MainVolumeLSB				= 0x27,
            BalanceLSB					= 0x28,
            PanLSB						= 0x2A,
            ExpressionControllerLSB		= 0x2B,
            EffectControl1LSB			= 0x2C,
            EffectControl2LSB			= 0x2D,
            DamperPedal					= 0x40,
            Portamento					= 0x41,
            Sostenuto					= 0x42,
            SoftPedal					= 0x43,
            LegatoFootswitch			= 0x44,
            Hold2						= 0x45,
            SoundController1			= 0x46,
            SoundController2			= 0x47,
            SoundController3			= 0x48,
            SoundController4			= 0x49,
            SoundController5			= 0x4A,
			SoundController6			= 0x4B,
            SoundController7			= 0x4C,
            SoundController8			= 0x4D,
            SoundController9			= 0x4E,
            SoundController10			= 0x4F,
            GeneralPurposeController5	= 0x50,
            GeneralPurposeController6	= 0x51,
            GeneralPurposeController7	= 0x52,
            GeneralPurposeController8	= 0x53,
            PortamentoControl			= 0x54,
            Effects1Depth				= 0x5B,
            Effects2Depth				= 0x5C,
            Effects3Depth				= 0x5D,
            Effects4Depth				= 0x5E,
            Effects5Depth				= 0x5F,
            DataIncrement				= 0x60,
            DataDecrement				= 0x61,
            NonRegisteredParameteLSB	= 0x62,
			NonRegisteredParameteMSB	= 0x63,
			RegisteredParameterLSB		= 0x64,
            RegisteredParameterMSB		= 0x65,
            AllSoundOff					= 0x78,
			ResetControllers			= 0x79,
            AllNotesOff					= 0x7B,
			OmniModeOff					= 0x7C,
			OmniModeOn					= 0x7D,
            Unknown 					= int.MaxValue - 1
        }
        public enum MidiMetaEvent
        {
            None,
            Sequence_Number,
            Text_Event,
            Copyright_Notice,
            Sequence_Or_Track_Name,
            Instrument_Name,
            Lyric_Text,
            Marker_Text,
            Cue_Point,
            Midi_Channel_Prefix_Assignment,
            End_of_Track,
            Tempo,
            Smpte_Offset,
            Time_Signature,
            Key_Signature,
            Sequencer_Specific_Event,
            Unknown
        }
        public enum MidiFormat
        {
            SingleTrack,
            MultiTrack,
            MultiSong
        }
    }
}
