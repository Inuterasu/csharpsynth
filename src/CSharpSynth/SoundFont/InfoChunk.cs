namespace NAudio.SoundFont
{
    using System;

    public class InfoChunk
    {
        private string author;
        private string bankName;
        private string comments;
        private string copyright;
        private string creationDate;
        private string dataROM;
        private string targetProduct;
        private string tools;
        private SFVersion verROM;
        private SFVersion verSoundFont;
        private string waveTableSoundEngine;

        internal InfoChunk(RiffChunk chunk)
        {
            RiffChunk chunk2;
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            if (chunk.ReadChunkID() != "INFO")
            {
                throw new ApplicationException("Not an INFO chunk");
            }
            while ((chunk2 = chunk.GetNextSubChunk()) != null)
            {
                switch (chunk2.ChunkID)
                {
                    case "ifil":
                        flag = true;
                        this.verSoundFont = (SFVersion) chunk2.GetDataAsStructure(new SFVersionBuilder());
                        break;

                    case "isng":
                        flag2 = true;
                        this.waveTableSoundEngine = chunk2.GetDataAsString();
                        break;

                    case "INAM":
                        flag3 = true;
                        this.bankName = chunk2.GetDataAsString();
                        break;

                    case "irom":
                        this.dataROM = chunk2.GetDataAsString();
                        break;

                    case "iver":
                        this.verROM = (SFVersion) chunk2.GetDataAsStructure(new SFVersionBuilder());
                        break;

                    case "ICRD":
                        this.creationDate = chunk2.GetDataAsString();
                        break;

                    case "IENG":
                        this.author = chunk2.GetDataAsString();
                        break;

                    case "IPRD":
                        this.targetProduct = chunk2.GetDataAsString();
                        break;

                    case "ICOP":
                        this.copyright = chunk2.GetDataAsString();
                        break;

                    case "ICMT":
                        this.comments = chunk2.GetDataAsString();
                        break;

                    case "ISFT":
                        this.tools = chunk2.GetDataAsString();
                        break;

                    default:
                        throw new ApplicationException(string.Format("Unknown chunk type {0}", chunk2.ChunkID));
                }
            }
            if (!flag)
            {
                throw new ApplicationException("Missing SoundFont version information");
            }
            if (!flag2)
            {
                throw new ApplicationException("Missing wavetable sound engine information");
            }
            if (!flag3)
            {
                throw new ApplicationException("Missing SoundFont name information");
            }
        }

        public override string ToString()
        {
            return string.Format("Bank Name: {0}\r\nAuthor: {1}\r\nCopyright: {2}\r\nCreation Date: {3}\r\nTools: {4}\r\nComments: {5}\r\nSound Engine: {6}\r\nSoundFont Version: {7}\r\nTarget Product: {8}\r\nData ROM: {9}\r\nROM Version: {10}", new object[] { this.BankName, this.Author, this.Copyright, this.CreationDate, this.Tools, "TODO-fix comments", this.WaveTableSoundEngine, this.SoundFontVersion, this.TargetProduct, this.DataROM, this.ROMVersion });
        }

        public string Author
        {
            get
            {
                return this.author;
            }
            set
            {
                this.author = value;
            }
        }

        public string BankName
        {
            get
            {
                return this.bankName;
            }
            set
            {
                this.bankName = value;
            }
        }

        public string Comments
        {
            get
            {
                return this.comments;
            }
            set
            {
                this.comments = value;
            }
        }

        public string Copyright
        {
            get
            {
                return this.copyright;
            }
            set
            {
                this.copyright = value;
            }
        }

        public string CreationDate
        {
            get
            {
                return this.creationDate;
            }
            set
            {
                this.creationDate = value;
            }
        }

        public string DataROM
        {
            get
            {
                return this.dataROM;
            }
            set
            {
                this.dataROM = value;
            }
        }

        public SFVersion ROMVersion
        {
            get
            {
                return this.verROM;
            }
            set
            {
                this.verROM = value;
            }
        }

        public SFVersion SoundFontVersion
        {
            get
            {
                return this.verSoundFont;
            }
        }

        public string TargetProduct
        {
            get
            {
                return this.targetProduct;
            }
            set
            {
                this.targetProduct = value;
            }
        }

        public string Tools
        {
            get
            {
                return this.tools;
            }
            set
            {
                this.tools = value;
            }
        }

        public string WaveTableSoundEngine
        {
            get
            {
                return this.waveTableSoundEngine;
            }
            set
            {
                this.waveTableSoundEngine = value;
            }
        }
    }
}

