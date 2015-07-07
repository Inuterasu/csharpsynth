StreamSynthesizer.cs

Implementation of ShortMessage(byte Command, byte Data1, byte Data2) so that also other devices and sequencers can use standard midi communication protocol

MidiSequencer.cs

Alex, your sequencer is really nice. Good work. But we need to separate sequencer from synth in order to make it more flexible and usable for others outputs. So sequencer should call a delegate ShortMessage(Command, Data1, Data2) so that developers can decide where these midi messages goes(either  into synth.ShortMessage() or to other destinations. In that way we can stick to a standard and not making that tight bondage as you have it now, but loose one.

loadstream(MidiFileStream) -> MidiEvent should have MidiShortMessageStruct, it can be stored there for further use in sending ShortMessages out of the sequencer.

struct MidiShortMessage{
> public byte Command;
> public byte Data1;
> public byte Data1;
}

Alex I hope that you see the point, that all(I mean all short messages) the parsing of MidiEvents that you do in load stream and in ProccesMidiEvent should be done on the side of Synthesizer in ShortMessage() function and not in the sequencer. Only that way can also other midi message generators and devices really use your synth class, otherwise it is just for your sequencer and for NoteOn and NoteOff sending. Sure the helper function in synth class like mentioned NoteOn, NoteOff can be there to facilitate developers their work, but when it comes to a real standard, right now we are not able easily receiving midi message from other devices or sequencers.

When I think of it. synth.ShortMessage() should not even use MidiEvent class, and instead try to parse everything from bytes in order to avoid GC to do its work or at lease use only value types in the midi message parsing process. Do you see the point?

NAudio

They have already written SF2 loader. There is likely no need for us to rewrite it again, so I would recommend to use theirs if the license will allow us. And the energy we save invest into better sounds synthesis and performance improvements.

Any other ideas?