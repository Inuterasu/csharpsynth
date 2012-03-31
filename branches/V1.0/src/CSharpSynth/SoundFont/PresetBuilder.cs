namespace NAudio.SoundFont
{
    using System;
    using System.IO;
    using System.Text;

    internal class PresetBuilder : StructureBuilder
    {
        private Preset lastPreset = null;

        public void LoadZones(Zone[] presetZones)
        {
            for (int i = 0; i < (base.data.Count - 1); i++)
            {
                Preset preset = (Preset) base.data[i];
                preset.Zones = new Zone[(preset.endPresetZoneIndex - preset.startPresetZoneIndex) + 1];
                Array.Copy(presetZones, preset.startPresetZoneIndex, preset.Zones, 0, preset.Zones.Length);
            }
            base.data.RemoveAt(base.data.Count - 1);
        }

        public override object Read(BinaryReader br)
        {
            Preset preset = new Preset();
            string str = Encoding.ASCII.GetString(br.ReadBytes(20));
            if (str.IndexOf('\0') >= 0)
            {
                str = str.Substring(0, str.IndexOf('\0'));
            }
            preset.Name = str;
            preset.PatchNumber = br.ReadUInt16();
            preset.Bank = br.ReadUInt16();
            preset.startPresetZoneIndex = br.ReadUInt16();
            preset.library = br.ReadUInt32();
            preset.genre = br.ReadUInt32();
            preset.morphology = br.ReadUInt32();
            if (this.lastPreset != null)
            {
                this.lastPreset.endPresetZoneIndex = (ushort) (preset.startPresetZoneIndex - 1);
            }
            base.data.Add(preset);
            this.lastPreset = preset;
            return preset;
        }

        public override void Write(BinaryWriter bw, object o)
        {
            Preset preset = (Preset) o;
        }

        public override int Length
        {
            get
            {
                return 0x26;
            }
        }

        public Preset[] Presets
        {
            get
            {
                return (Preset[]) base.data.ToArray(typeof(Preset));
            }
        }
    }
}

