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
    public class TwoTrackMixerSampleProvider : ISampleProvider
    {
        private CustomFadeSampleProvider _current;
        private SoundBankStreamed.StreamedSoundData _next;
        private MusicManager.TrackData _nextData;
        private double _fadeInTime;

        public float Volume { get; set; }

        public TwoTrackMixerSampleProvider(WaveFormat format)
        {
            WaveFormat = format;

            Volume = 1f;
        }

        public WaveFormat WaveFormat { get; private set; }

        public void SetTrack(SoundBankStreamed.StreamedSoundData track, MusicManager.TrackData data, double fadeInMilliseconds, double fadeOutMillseconds)
        {
            if (_current == null)
            {
                SetCurrent(track, data, fadeInMilliseconds);
            }
            else
            {
                _next = track;
                if (fadeOutMillseconds == 0)
                {
                    SetCurrent(track, data, fadeInMilliseconds);
                    return;
                }
                _current.BeginFadeOut(fadeOutMillseconds);
                _fadeInTime = fadeInMilliseconds;
            }
        }

        public void ForceTrack(ISampleProvider provider)
        {
            _current = new CustomFadeSampleProvider(provider);
        }

        private void SetCurrent(SoundBankStreamed.StreamedSoundData track, MusicManager.TrackData data, double fadeInMilliseconds, int offset = 0)
        {
            using (FileStream stream = File.Open(track.File, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                stream.Seek(track.Position + offset, SeekOrigin.Begin);
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    byte[] buffer = reader.ReadBytes(track.Length);

                    double start = data == null ? 0 : data.LoopStart;
                    double end = data == null ? 0 : data.LoopEnd;

                    _current = new CustomFadeSampleProvider(new WaveToSampleProvider(new LoopStream(new VorbisWaveReader(new MemoryStream(buffer)), start, end)));
                    _current.BeginFadeIn(fadeInMilliseconds);
                }
            }
        }

        public int Read(float[] buffer, int offset, int sampleCount)
        {
            if (_current == null)
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
                int samplesRead = _current.Read(buffer, offset, needed);
                read += samplesRead;
                if (samplesRead == 0 || _current.IsSilent)
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
                    SetCurrent(_next, _nextData, _fadeInTime);
                }
            }

            for (int i = offset; i < offset + sampleCount; i++)
            {
                buffer[i] *= Volume;
            }

            return read;
        }
    }
}
