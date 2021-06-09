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
    public class SoundBankCached
    {
        private Dictionary<string, CachedSoundData> _cacheDict;

        public SoundBankCached(string file)
        {
            _cacheDict = new Dictionary<string, CachedSoundData>();

            using (FileStream stream = File.Open(file, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        string name = reader.ReadString();
                        int length = reader.ReadInt32();
                        byte[] data = reader.ReadBytes(length);

                        CacheSound(name, data);
                    }
                }
            }
        }

        private void CacheSound(string name, byte[] data)
        {
            _cacheDict[name] = new CachedSoundData(data);
        }

        public bool ContainsKey(string name) => _cacheDict.ContainsKey(name);

        public CachedSoundData this[string name]
        {
            get
            {
                if (_cacheDict.ContainsKey(name))
                {
                    return _cacheDict[name];
                }
                throw new KeyNotFoundException("Sound not found in Sound Bank: " + name);
            }
        }

        public class CachedSoundData
        {
            public float[] data;
            public WaveFormat format;

            public CachedSoundData(byte[] data)
            {
                using (MemoryStream stream = new MemoryStream(data))
                {
                    using (VorbisWaveReader reader = new VorbisWaveReader(stream))
                    {
                        this.data = new float[reader.Length];
                        this.format = reader.WaveFormat;
                        reader.Read(this.data, 0, (int)reader.Length);
                    }
                }
            }
        }
    }
}
