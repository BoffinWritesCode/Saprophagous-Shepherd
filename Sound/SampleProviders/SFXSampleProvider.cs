using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;

namespace MiniJam61Egypt.Sound
{
    public class SFXSampleProvider : ISampleProvider
    {
        private readonly SoundBankCached.CachedSoundData _sound;
        private long _pos;

        public WaveFormat WaveFormat { get => _sound.format; }

        public SFXSampleProvider(SoundBankCached.CachedSoundData sound)
        {
            _sound = sound;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            var availableSamples = _sound.data.Length - _pos;
            var samplesToCopy = Math.Min(availableSamples, count);
            Array.Copy(_sound.data, _pos, buffer, offset, samplesToCopy);
            _pos += samplesToCopy;
            return (int)samplesToCopy;
        }
    }
}
