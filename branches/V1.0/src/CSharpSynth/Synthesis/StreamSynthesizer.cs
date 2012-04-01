using System;
using System.Collections.Generic;
using CSharpSynth.Banks;
using CSharpSynth.Effects;

namespace CSharpSynth.Synthesis
{
    public partial class StreamSynthesizer
    {
		
        #region Private Variable
		
		private InstrumentBank bank;
        private float[,] sampleBuffer;
        private int rawBufferLength;
        private Voice[] voicePool;
        private LinkedList<Voice> activeVoices;
        private Stack<Voice> freeVoices;
        private Dictionary<NoteRegistryKey, List<Voice>> keyRegistry;
        private int[] instruments_;
        private float[] panPositions_;
        private float[] volPositions_;
        private double[] tunePositions_;
        private double[] vibraPositions_;
        private double[] pitchWheelSemitoneRange_;
        private byte[] RPC; //register coarse
        private byte[] RPF; //register fine
        private List<BasicAudioEffect> effects;
				
        readonly private int audioChannels = 1;
        readonly private int sampleRate = 44100;
        readonly private int samplesperBuffer = 2000;
		
		//total number of voices available
        private int polyphony = 40; 
		//how many of the same note can be playing at once
        private int maxnotepoly = 2; 
        //Tweakable Parameters, anytime via properties
        private float MainVolume = 1.0f; //Not too high or will cause clipping
		
		#endregion
		
		#region Public Properties
		
        public int BufferSize
        {
            get { return rawBufferLength; }
        }
        public byte[] RegisteredParameterCoarse
        {
            get { return RPC; }
        }
        public byte[] RegisteredParameterFine
        {
            get { return RPF; }
        }
        public double[] PitchWheelPositions
        {
            get { return pitchWheelSemitoneRange_; }
        }
        public double[] VibratoPositions
        {
            get { return vibraPositions_; }
        }
        public float[] PanPositions
        {
            get { return panPositions_; }
        }
        public float[] VolPositions
        {
            get { return volPositions_; }
        }
        public double[] TunePositions
        {
            get { return tunePositions_; }
        }
        public int MaxPolyPerNote
        {
            get { return maxnotepoly; }
            set { maxnotepoly = value; }
        }
        public float MasterVolume
        {
            get { return MainVolume; }
            set { MainVolume = SynthHelper.Clamp(value, 0.0f, 1.0f); }
        }
        public int SampleRate
        {
            get { return sampleRate; }
        }
		public int SamplesPerBuffer
        {
            get { return samplesperBuffer; }
        }
        public int Channels
        {
            get { return audioChannels; }
        }
        public InstrumentBank SoundBank
        {
            get { return bank; }
        }
		
		#endregion
		
		#region Public Methods
		
		/* stick with samples per buffer, if you want ms use the helper method to convert to samples*/
		public StreamSynthesizer(int sampleRate, int audioChannels, int samplesPerBuffer, int maxpoly)
        {
			this.sampleRate = sampleRate;
            this.audioChannels = audioChannels;
            this.samplesperBuffer = samplesPerBuffer;
            this.polyphony = maxpoly;
            
			/* readonly variables can be set only in constructor, so the check has to be done here */
            if (sampleRate < 8000 || sampleRate > 48000)
            {
                sampleRate = 44100;
                this.samplesperBuffer = (sampleRate / 1000) * 50;
                DBG.error("-----> Invalid Sample Rate! Changed to---->" + sampleRate);
                DBG.error("-----> Invalid Buffer Size! Changed to---->" + 50 + "ms");
            }
            if (polyphony < 1 || polyphony > 500)
            {
                polyphony = 40;
                DBG.error("-----> Invalid Max Poly! Changed to---->" + polyphony);
            }
            if (maxnotepoly < 1 || maxnotepoly > polyphony)
            {
                maxnotepoly = 2;
                DBG.error("-----> Invalid Max Note Poly! Changed to---->" + maxnotepoly);
            }
            if (samplesperBuffer < 100 || samplesperBuffer > 500000)
            {
                this.samplesperBuffer = (int)((sampleRate / 1000.0) * 50.0);
                DBG.error("-----> Invalid Buffer Size! Changed to---->" + 50 + "ms");
            }
            if (audioChannels < 1 || audioChannels > 2)
            {
                audioChannels = 1;
                DBG.error("-----> Invalid Audio Channels! Changed to---->" + audioChannels);
            }
						
			setupSynth();
		}
				
        public bool LoadBank(string filename)
        {
            try
            {
                BankManager.addBank(new InstrumentBank(sampleRate, filename));
                SwitchBank(BankManager.Count - 1);
            }
            catch (Exception ex)
            {
                DBG.error("Bank load error!\n" + ex.Message + "\n\n" + ex.StackTrace);
                return false;
            }
            return true;
        }
		
        public bool UnloadBank(int index)
        {
            if (index < BankManager.Count)
            {
                if (BankManager.Banks[index] == bank)
                    bank = null;
                BankManager.removeBank(index);
                return true;
            }
            return false;
        }
		
        public bool UnloadBank()
        {
            if (bank != null)
            {
                BankManager.removeBank(bank);
                return true;
            }
            return false;
        }
		
        public void SwitchBank(int index)
        {
            if (index < BankManager.Count)
            {//banks are reloaded if at different sample rate
                this.bank = BankManager.getBank(index);
                if (this.bank.SampleRate != this.sampleRate)
                    bank.reload(this.sampleRate);
            }
        }
		
        public void setPan(int channel, float position)
        {
            if (channel > -1 && channel < panPositions_.Length && position >= -1.00f && position <= 1.00f)
                panPositions_[channel] = position;
        }
		
        public void setVolume(int channel, float position)
        {
            if (channel > -1 && channel < volPositions_.Length && position >= 0.00f && position <= 1.00f)
                volPositions_[channel] = position;
        }
		
        public void setPitchBend(int channel, double semitones)
        {
            if (channel > -1 && channel < tunePositions_.Length && semitones >= -12.00 && semitones <= 12.00)
            {
                tunePositions_[channel] = semitones;
            }
        }
		
        public void resetSynthControls()
        {
            //Reset Pan Positions back to 0.0f
            Array.Clear(panPositions_, 0, panPositions_.Length);
            //Set Tuning Positions back to 0.0
            Array.Clear(tunePositions_, 0, tunePositions_.Length);
            //Reset Vol Positions back to 1.00f
            for (int x = 0; x < volPositions_.Length; x++)
                volPositions_[x] = 1.00f;
            //Reset instruments
            Array.Clear(instruments_, 0, instruments_.Length);
            //Reset vibrato
            Array.Clear(vibraPositions_, 0, vibraPositions_.Length);
            //Reset pitch wheel to 2 semitones
            for (int x = 0; x < volPositions_.Length; x++)
                pitchWheelSemitoneRange_[x] = 2.0;
            //Reset coarse select
            Array.Clear(RPC, 0, RPC.Length);
            //Reset fine select
            Array.Clear(RPF, 0, RPF.Length);
        }
		
        public void Dispose()
        {
            Stop();
            sampleBuffer = null;
            voicePool = null;
            activeVoices.Clear();
            freeVoices.Clear();
            keyRegistry.Clear();
            effects.Clear();
        }
		
        public void Stop()
        {
            NoteOffAll(true);
        }
	      		
		public void GetNext(byte[] buffer)
        {//Call this to process the next part of audio and return it in raw form.
            ClearWorkingBuffer();
            FillWorkingBuffer();
            for (int x = 0; x < effects.Count; x++)
            {
                effects[x].doEffect(sampleBuffer);
            }
            ConvertBuffer(sampleBuffer, buffer);
        }
		
		public void GetNext(float[] buffer)
        {
            ClearWorkingBuffer();
            FillWorkingBuffer();
            for (int x = 0; x < effects.Count; x++)
            {
                effects[x].doEffect(sampleBuffer);
            }
            ConvertBuffer(sampleBuffer, buffer);
        }
		
        public void AddEffect(BasicAudioEffect effect)
        {
            effects.Add(effect);
        }
		
        public void RemoveEffect(int index)
        {
            effects.RemoveAt(index);
        }
		
        public void ClearEffects()
        {
            effects.Clear();
        }
		
		#endregion
        
		#region Private Methods
		
        private Voice getFreeVoice()
        {
            if (freeVoices.Count == 0)
                return null;
            return freeVoices.Pop();
        }
		
        private Voice getUsedVoice(NoteRegistryKey r)
        {
            List<Voice> voicelist;
            Voice voice;
            if (keyRegistry.TryGetValue(r, out voicelist))
            {
                if (voicelist.Count > 0)
                {
                    voicelist[0].StopImmediately();
                    voice = voicelist[0];
                    voicelist.RemoveAt(0);
                    activeVoices.Remove(voice);
                    return voice;
                }
            }
            return null;
        }
		
        private void ConvertBuffer(float[,] from, byte[] to)
        {
            int bytesPerSample = 2; //assume 16 bit audio
            int channels = from.GetLength(0);
            int bufferSize = from.GetLength(1);

            for (int i = 0; i < bufferSize; i++)
            {
                for (int c = 0; c < channels; c++)
                {
                    // Apply master volume
                    float floatSample = from[c, i] * MainVolume;

                    // Clamp the value to the [-1.0..1.0] range
                    floatSample = SynthHelper.Clamp(floatSample, -1.0f, 1.0f);

                    // Convert it to the 16 bit [short.MinValue..short.MaxValue] range
                    short shortSample = (short)(floatSample * short.MaxValue);

                    // Calculate the right index based on the PCM format of interleaved samples per channel [L-R-L-R]
                    int index = i * channels * bytesPerSample + c * bytesPerSample;

                    // Store the 16 bit sample as two consecutive 8 bit values in the buffer with regard to endian-ness
                    if (!BitConverter.IsLittleEndian)
                    {
                        to[index] = (byte)(shortSample >> 8);
                        to[index + 1] = (byte)shortSample;
                    }
                    else
                    {
                        to[index] = (byte)shortSample;
                        to[index + 1] = (byte)(shortSample >> 8);
                    }
                }
            }
        }
		
		private void ConvertBuffer(float[,] from, float[] to)
        {
            int channels = from.GetLength(0);
            int bufferSize = from.GetLength(1);
            // Make sure the buffer sizes are correct
            System.Diagnostics.Debug.Assert(to.Length == from.Length, "Buffer sizes are mismatched.");
            						
			int k = 0;
			for (int i = 0; i < bufferSize; i++)
            {
                for (int c = 0; c < channels; c++)
                {
                    // Apply master volume
                    float floatSample = from[c, i] * MainVolume;

                    //Clamp the value to the [-1.0..1.0] range
                    to[k] = SynthHelper.Clamp(floatSample, -1.0f, 1.0f);

					k++;
                }
            }
        }
		
        private void FillWorkingBuffer()
        {
            // Call process on all active voices
            LinkedListNode<Voice> node;
            LinkedListNode<Voice> delnode;
            node = activeVoices.First;
            ProcessServerMessages();
			while (node != null)
            {
                //Process buffer with no interrupt for events
				node.Value.Process(sampleBuffer, 0, samplesperBuffer);
                if (node.Value.isInUse == false)
                {
                    delnode = node;
                    node = node.Next;
                    freeVoices.Push(delnode.Value);
                    activeVoices.Remove(delnode);
                }
                else
                {
                    node = node.Next;
                }
            }
        }
		
        private void ClearWorkingBuffer()
        {
            Array.Clear(sampleBuffer, 0, audioChannels * samplesperBuffer);
        }
		
        private void setupSynth()
        {
            //initialize variables
            sampleBuffer = new float[audioChannels, samplesperBuffer];
            rawBufferLength = audioChannels * samplesperBuffer * 2; //Assuming 16 bit data
            // Create voice structures
            voicePool = new Voice[polyphony];
            for (int i = 0; i < polyphony; ++i)
                voicePool[i] = new Voice(this);
            freeVoices = new Stack<Voice>(voicePool);
            activeVoices = new LinkedList<Voice>();
            keyRegistry = new Dictionary<NoteRegistryKey, List<Voice>>();
            //Setup Channel Data
            panPositions_ = new float[16];
            volPositions_ = new float[16];
            tunePositions_ = new double[16];
            vibraPositions_ = new double[16];
            instruments_ = new int[16];
            pitchWheelSemitoneRange_ = new double[16];
            RPC = new byte[16];
            RPF = new byte[16];
            resetSynthControls(); //set controls to default values
            //create effect list
            effects = new List<BasicAudioEffect>();
        }
		
		#endregion
		
    }
}
