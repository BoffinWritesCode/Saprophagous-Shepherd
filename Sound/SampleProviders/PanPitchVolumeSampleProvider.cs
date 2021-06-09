using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Utils;

namespace MiniJam61Egypt.Sound
{
    public class PanVolumeSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider _source;
        private float[] _privateBuffer;

        private float _pan;
        private float _left;
        private float _right;

        public WaveFormat WaveFormat => _source.WaveFormat;

        public float Pan
        {
            get => _pan;
            set
            {
                _pan = value;
                GetMults(out _left, out _right);
            }
        }

        public float Volume { get; set; }

        public PanVolumeSampleProvider(ISampleProvider source, float volume = 1f, float initialPan = 0f)
        {
            _source = source;

            Pan = initialPan;
            Volume = volume;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            switch(WaveFormat.Channels)
            {
                default:
                case 1:
                    return ReadMono(buffer, offset, count);
                case 2:
                    return ReadStereo(buffer, offset, count);
            }
        }

        /// <summary>
        /// Opposite channel decays linearly as the balance changes.
        /// </summary>
        private void GetMults(out float left, out float right)
        {
            left = Pan <= 0 ? 1f : (1 - Pan) * 0.5f;
            right = Pan >= 0 ? 1f : (Pan + 1) * 0.5f;
        }

        private int ReadMono(float[] buffer, int offset, int sampleCount)
        {
            int sourceSamplesRequired = sampleCount / 2;

            _privateBuffer = BufferHelpers.Ensure(_privateBuffer, sourceSamplesRequired);
            int sourceSamplesRead = _source.Read(_privateBuffer, 0, sourceSamplesRequired);

            int outIndex = offset;
            for (int n = 0; n < sourceSamplesRead; n++)
            {
                buffer[outIndex++] = _left * _privateBuffer[n] * Volume;
                buffer[outIndex++] = _right * _privateBuffer[n] * Volume;
            }

            return sourceSamplesRead * 2;
        }

        private int ReadStereo(float[] buffer, int offset, int sampleCount)
        {
            _privateBuffer = BufferHelpers.Ensure(_privateBuffer, sampleCount);
            int sourceSamplesRead = _source.Read(_privateBuffer, 0, sampleCount);
            int outIndex = offset;
            bool useLeft = true;
            for (int n = 0; n < sourceSamplesRead; n++)
            {
                float multiplier = useLeft ? _left : _right;
                useLeft = !useLeft;
                buffer[outIndex++] = multiplier * _privateBuffer[n] * Volume;
            }

            return sourceSamplesRead;
        }
    }
}
