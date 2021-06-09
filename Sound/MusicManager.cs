using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using NAudio;
using NAudio.Vorbis;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace MiniJam61Egypt.Sound
{
    public class MusicManager : IDisposable
    {
        private FadeMixerSampleProvider _mixer;
        private LowHighShelfSampleProvider _mainProvider;
        private WaveOut _output;
        private Dictionary<string, TrackData> _trackData;
        private SoundBankStreamed _soundBank;

        public float Volume { get => _mixer.Volume; set => _mixer.Volume = value; }
        public bool EQEnabled { get => _mainProvider.Enabled; set => _mainProvider.Enabled = value; }
        public float LowShelfCutoff { get => _mainProvider.LowShelfCutoff; set => _mainProvider.LowShelfCutoff = value; }
        public float HighShelfCutoff { get => _mainProvider.HighShelfCutoff; set => _mainProvider.HighShelfCutoff = value; }
        public float LowShelfGain { get => _mainProvider.LowShelfGain; set => _mainProvider.LowShelfGain = value; }
        public float HighShelfGain { get => _mainProvider.HighShelfGain; set => _mainProvider.HighShelfGain = value; }

        public MusicManager(SoundBankStreamed soundBank, WaveFormat format = null, int latencyMilliseconds = 200)
        {
            _soundBank = soundBank;

            //Default wave format is 32-bit float 44.1Khz stereo
            _mixer = new FadeMixerSampleProvider(format == null ? WaveFormat.CreateIeeeFloatWaveFormat(44100, 2) : format);
            _mainProvider = new LowHighShelfSampleProvider(_mixer);

            _trackData = new Dictionary<string, TrackData>();

            _output = new WaveOut();
            _output.DesiredLatency = latencyMilliseconds;
            _output.Init(_mainProvider);
            _output.Play();
        }

        /// <summary>
        /// Assign a track by name and file.
        /// </summary>
        /// <param name="name">The internal name of the track.</param>
        /// <param name="file">The file location of the track.</param>
        /// <param name="loopStart">Where the stream should loop back to when the track hits the loop end.</param>
        /// <param name="loopEnd">Where the stream should end (leave as 0 for the end of the track)</param>
        public void CreateTrackData(string name, double loopStart = 0.0, double loopEnd = 0.0)
        {
            if (_soundBank.ContainsKey(name))
            {
                _trackData[name] = new TrackData(loopStart, loopEnd);
                return;
            }
            throw new KeyNotFoundException($"Key {name} not found in sound bank. Doesn't exist?");
        }

        public void AddTrack(string track)
        {
            TrackData data;
            if (_trackData.TryGetValue(track, out data))
            {
                _mixer.Add(track, _soundBank[track], data);
            }
            else
            {
                _mixer.Add(track, _soundBank[track], new TrackData(0, 0));
            }
        }

        public void Play(string track, double fadeTime)
        {
            _mixer.SetCurrent(track, fadeTime);
        }

        public void Dispose()
        {
            _output.Stop();
            _output.Dispose();
            _output = null;
        }

        public class TrackData
        {
            public double LoopStart;
            public double LoopEnd;
            public TrackData(double s, double e)
            {
                LoopStart = s;
                LoopEnd = e;
            }
        }
    }
}
