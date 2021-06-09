using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace MiniJam61Egypt.Sound
{
    public class NAudioSFXManager : ISFXManager
    {
        private MixingSampleProvider _mixer;
        private SoundBankCached _soundBank;
        private WaveOut _output;

        public float Volume { get; set; }

        public NAudioSFXManager(SoundBankCached soundBank, int sampleRate = 44100, int channels = 2, int latencyMilliseconds = 50)
        {
            Volume = 1f;

            _soundBank = soundBank;

            _mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels));
            _mixer.ReadFully = true;

            _output = new WaveOut();
            _output.DesiredLatency = latencyMilliseconds;
            _output.Init(_mixer);
            _output.Play();
        }

        public void PlaySound(string name, float volume = 1f, float pan = 0f)
        {
            _mixer.AddMixerInput(CreatePitchPanVolume(new SFXSampleProvider(_soundBank[name]), volume, pan));
        }

        public void PlaySound(string name, float volume = 1f, float pan = 0f, float pitch = 1f)
        {
            _mixer.AddMixerInput(new SmbPitchShiftingSampleProvider(CreatePitchPanVolume(new SFXSampleProvider(_soundBank[name]), volume, pan), 512, 4, pitch));
        }

        private ISampleProvider CreatePitchPanVolume(ISampleProvider source, float volume, float pan)
        {
            return new PanVolumeSampleProvider(source, volume * Volume, pan);
        }

        public void Dispose()
        {
            _output.Dispose();
        }
    }
}
