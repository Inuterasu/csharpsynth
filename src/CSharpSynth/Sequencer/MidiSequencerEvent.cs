using System.Collections.Generic;
using CSharpSynth.Midi;

namespace CSharpSynth.Sequencer
{
    public class MidiSequencerEvent
    {
        //--Variables
        public List<MidiEvent> Events; //List of Events
        public SampleSequencer Sequencer;
        //--Public Methods
        public MidiSequencerEvent(SampleSequencer Sequencer)
        {
            Events = new List<MidiEvent>();
            this.Sequencer = Sequencer;
        }
        public MidiSequencerEvent()
        {
            Events = new List<MidiEvent>();
        }
    }
}
