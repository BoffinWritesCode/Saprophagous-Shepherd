using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Vorbis;

namespace MiniJam61Egypt.Sound
{
    public class SoundBankStreamed
    {
        Dictionary<string, StreamedSoundData> _streamDict;

        public SoundBankStreamed(string file)
        {
            _streamDict = new Dictionary<string, StreamedSoundData>();

            using (FileStream stream = File.Open(file, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        string name = reader.ReadString();
                        int length = reader.ReadInt32();

                        AddSound(name, file, length, reader.BaseStream.Position);

                        reader.ReadBytes(length);
                    }
                }
            }
        }

        private void AddSound(string name, string file, int len, long pos)
        {
            _streamDict[name] = new StreamedSoundData(file, len, pos);
        }

        public bool ContainsKey(string name) => _streamDict.ContainsKey(name);

        public StreamedSoundData this[string name]
        {
            get
            {
                if (_streamDict.ContainsKey(name))
                {
                    return _streamDict[name];
                }
                throw new KeyNotFoundException("Sound not found in Sound Bank: " + name);
            }
        }

        public class StreamedSoundData
        {
            public string File;
            public int Length;
            public long Position;

            public StreamedSoundData(string file, int len, long pos)
            {
                File = file;
                Length = len;
                Position = pos;
            }
        }
    }
}
