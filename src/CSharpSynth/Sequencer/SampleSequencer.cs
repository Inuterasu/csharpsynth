/*    ______   __ __     _____             __  __  
 *   / ____/__/ // /_   / ___/__  ______  / /_/ /_ 
 *  / /    /_  _  __/   \__ \/ / / / __ \/ __/ __ \
 * / /___ /_  _  __/   ___/ / /_/ / / / / /_/ / / /
 * \____/  /_//_/     /____/\__, /_/ /_/\__/_/ /_/ 
 *                         /____/                  
 * Midi Sequencer 
 * 
 * Supports Accurate: (Sample Precision)  
 *  
 */
using CSharpSynth.Midi;
using CSharpSynth.Sequencer;
using System;
using CSharpSynth;
using CSharpSynth.Synthesis;

namespace CSharpSynth.Sequencer
{
    public class SampleSequencer
    {
        //--Variables
        private MidiFile _MidiFile;
        private bool[] blockList;
        private bool playing = false;
        private bool looping = false;
        private int sampleTime;
        private int eventIndex;
        private double BPM;
        private int sampleRate;
        private MidiSequencerEvent eventQueue;
        //--Public Properties
        public bool isPlaying
        {
            get { return playing; }
        }
        public double BeatsPerMinute
        {
            get { return BPM; }
        }
        public int CurrentSampleTime
        {
            get { return sampleTime; }
        }
        public int EndSampleTime
        {
            get { return (int)_MidiFile.Tracks[0].TotalTime; }
        }
        public TimeSpan EndTime
        {
            get { return new TimeSpan(0, 0, (int)SynthHelper.getTimeFromSample(sampleRate, (int)_MidiFile.Tracks[0].TotalTime)); }
        }
        public TimeSpan Time
        {
            get { return new TimeSpan(0, 0, (int)SynthHelper.getTimeFromSample(sampleRate, sampleTime)); }
        }
        public bool Looping
        {
            get { return looping; }
            set { looping = value; }
        }
        //--Public Methods
        public SampleSequencer(int sampleRate)
        {
            this.sampleRate = sampleRate;
            blockList = new bool[16];
            eventQueue = new MidiSequencerEvent(this);
        }
        public bool LoadMidi(MidiFile midi, bool UnloadUnusedInstruments)
        {
            if (playing == true)
                return false;
            _MidiFile = midi;
            if (_MidiFile.SequencerReady == false)
            {
                try
                {
                    BPM = 120.0;
                    //Combine all tracks into 1 track that is organized from lowest to highest abs time
                    _MidiFile.CombineTracks();
                    //Convert delta time to sample time
                    eventIndex = 0;              
                    double lastSample = 0;
                    double currentSample = 0;
                    for (int x = 0; x < _MidiFile.Tracks[0].MidiEvents.Length; x++)
                    {
                        currentSample = lastSample + DeltaTimetoSamples(_MidiFile.Tracks[0].MidiEvents[x].deltaTime);
                        _MidiFile.Tracks[0].MidiEvents[x].deltaTime = (uint)currentSample;
                        lastSample = currentSample;
                        //Update tempo
                        if (_MidiFile.Tracks[0].MidiEvents[x].midiMetaEvent == MidiHelper.MidiMetaEvent.Tempo)
                        {
                            BPM = MidiHelper.MicroSecondsPerMinute / System.Convert.ToDouble(_MidiFile.Tracks[0].MidiEvents[x].Parameters[0]);
                        }
                    }
                    //Set total time to proper value
                    _MidiFile.Tracks[0].TotalTime = _MidiFile.Tracks[0].MidiEvents[_MidiFile.Tracks[0].MidiEvents.Length - 1].deltaTime;
                    //reset tempo
                    BPM = 120;
                    //mark midi as ready for sequencing
                    _MidiFile.SequencerReady = true;
                }
                catch (Exception ex)
                {
                    PlatformDebug.error("Error Loading Midi:\n" + ex.Message);
                    return false;
                }
            }
            Array.Clear(blockList, 0, blockList.Length);
            return true;
        }
        public bool LoadMidi(string file, bool UnloadUnusedInstruments)
        {
            if (playing == true)
                return false;
            MidiFile mf = null;
            try
            {
                mf = new MidiFile(file);
            }
            catch (Exception ex)
            {
                PlatformDebug.error("Error Loading Midi:\n" + ex.Message);
                return false;
            }
            return LoadMidi(mf, UnloadUnusedInstruments);
        }
        public void Play()
        {
            if (playing == true)
                return;
            //set bpm
            BPM = 120.0;
            //Let the synth know that the sequencer is ready.
            eventIndex = 0;
            playing = true;
        }
        public void Stop()
        {
            playing = false;
            sampleTime = 0;
        }
        public bool isChannelMuted(int channel)
        {
            return blockList[channel];
        }
        public void MuteChannel(int channel)
        {
            blockList[channel] = true;
        }
        public void UnMuteChannel(int channel)
        {
            blockList[channel] = false;
        }
        public void MuteAllChannels()
        {
            for (int x = 0; x < blockList.Length; x++)
                blockList[x] = true;
        }
        public void UnMuteAllChannels()
        {
            for (int x = 0; x < blockList.Length; x++)
                blockList[x] = false;
        }
        public MidiSequencerEvent ProcessFrame(StreamSynthesizer synth)
        {
            if (!playing)
                return null;
            eventQueue.Events.Clear();
            //stop or loop
            if (sampleTime >= (int)_MidiFile.Tracks[0].TotalTime)
            {
                sampleTime = 0;
                if (looping == true)
                {
                    //Clear the current programs for the channels.
                    synth.resetPrograms();
                    //Clear vol, pan, and tune
                    synth.resetSynthControls();
                    //set bpm
                    BPM = 120;
                    //Let the synth know that the sequencer is ready.
                    eventIndex = 0;
                }
                else
                {
                    playing = false;
                    synth.NoteOffAll(true);
                    return eventQueue;
                }
            }
            while (eventIndex < _MidiFile.Tracks[0].EventCount && _MidiFile.Tracks[0].MidiEvents[eventIndex].deltaTime < (sampleTime + synth.SamplesPerBuffer))
            {
                eventQueue.Events.Add(_MidiFile.Tracks[0].MidiEvents[eventIndex]);
                eventIndex++;
            }
            return eventQueue;
        }
        public void IncrementSampleCounter(int amount)
        {
            sampleTime = sampleTime + amount;
        }
        public void SetTime(TimeSpan time, StreamSynthesizer synth)
        {
            int _stime = SynthHelper.getSampleFromTime(sampleRate, (float)time.TotalSeconds);
            if (_stime > sampleTime)
            {
                SilentProcess(_stime - sampleTime, synth);
            }
            else if (_stime < sampleTime)
            {//we have to restart the midi to make sure we get the right temp, instrument, etc
                synth.Stop();
                sampleTime = 0;
                synth.resetPrograms();
                synth.resetSynthControls();
                BPM = 120;
                eventIndex = 0;
                SilentProcess(_stime, synth);
            }
        }
        //--Private Methods
        private double DeltaTimetoSamples(double DeltaTime)
        {
            return sampleRate * (DeltaTime * (60.0 / (BPM * _MidiFile.MidiHeader.DeltaTiming)));
        }
        private void SilentProcess(int amount, StreamSynthesizer synth)
        {
            while (eventIndex < _MidiFile.Tracks[0].EventCount && _MidiFile.Tracks[0].MidiEvents[eventIndex].deltaTime < (sampleTime + amount))
            {
                if (_MidiFile.Tracks[0].MidiEvents[eventIndex].midiChannelEvent != MidiHelper.MidiChannelEvent.Note_On)
                    synth.ProcessSampleMessage(_MidiFile.Tracks[0].MidiEvents[eventIndex]);
                eventIndex++;
            }
            sampleTime = sampleTime + amount;
        }
    }
}