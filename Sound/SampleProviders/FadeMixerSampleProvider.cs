using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NAudio.Utils;
using NAudio.Wave;
using NAudio.Vorbis;
using NAudio.Wave.SampleProviders;

namespace MiniJam61Egypt.Sound
{
    public class FadeMixerSampleProvider : ISampleProvider
    {
        public float Volume { get; set; }

        private Dictionary<string, CustomFadeSampleProvider> _providers;
        private string _previousTrack;
        private string _currentTrack;

        public FadeMixerSampleProvider(WaveFormat format)
        {
            WaveFormat = format;
            _providers = new Dictionary<string, CustomFadeSampleProvider>();
            Volume = 1f;
        }

        public WaveFormat WaveFormat { get; private set; }

        public void Add(string name, SoundBankStreamed.StreamedSoundData track, MusicManager.TrackData data)
        {
            using (FileStream stream = File.Open(track.File, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                stream.Seek(track.Position, SeekOrigin.Begin);
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    byte[] buffer = reader.ReadBytes(track.Length);

                    double start = data == null ? 0 : data.LoopStart;
                    double end = data == null ? 0 : data.LoopEnd;

                    _providers[name] = new CustomFadeSampleProvider(new WaveToSampleProvider(new LoopStream(new VorbisWaveReader(new MemoryStream(buffer)), start, end)));
                }
            }
        }

        public void SetCurrent(string name, double fadeTime)
        {
            if (name == _currentTrack) return;

            if (!string.IsNullOrEmpty(_currentTrack))
            {
                _providers[_currentTrack].BeginFadeOut(fadeTime);
            }
            _previousTrack = _currentTrack;
            _currentTrack = name;
            _providers[_currentTrack].BeginFadeIn(fadeTime);
        }

        public int Read(float[] buffer, int offset, int sampleCount)
        {
            if (string.IsNullOrEmpty(_currentTrack))
            {
                for (int i = offset; i < offset + sampleCount; i++)
                {
                    buffer[i] = 0f;
                }
                return sampleCount;
            }


            int read = 0;
            while (read < sampleCount)
            {
                int needed = sampleCount - read;
                CustomFadeSampleProvider provider = _providers[_currentTrack];
                int samplesRead = provider.Read(buffer, offset, needed);
                read += samplesRead;
                if (!string.IsNullOrEmpty(_previousTrack))
                {
                    CustomFadeSampleProvider prev = _providers[_previousTrack];
                    if (!prev.IsSilent)
                    {
                        float[] samples = new float[samplesRead];
                        prev.Read(samples, 0, samplesRead);
                        for (int i = offset; i < offset + samplesRead; i++)
                        {
                            buffer[i] += samples[i - offset];
                        }
                    }
                }
                /*
                if (samplesRead == 0 || provider.IsSilent)
                {
                    if (_next == null)
                    {
                        for (int i = offset + read; i < offset + sampleCount; i++)
                        {
                            buffer[i] = 0f;
                        }
                        read = sampleCount;
                        break;
                    }
                    SetCurrent(_mainCurrent2, _next, _nextData, _fadeInTime);
                }
                */

            }

            for (int i = offset; i < offset + sampleCount; i++)
            {
                buffer[i] *= Volume;
            }

            foreach(KeyValuePair<string, CustomFadeSampleProvider> pair in _providers)
            {
                //float seconds = sampleCount / (float)WaveFormat.SampleRate;
                //TimeSpan time = TimeSpan.FromSeconds(seconds);
                if (pair.Value.IsSilent)
                {
                    pair.Value.Skip(sampleCount);
                }
            }

            return read;
        }
    }
}
