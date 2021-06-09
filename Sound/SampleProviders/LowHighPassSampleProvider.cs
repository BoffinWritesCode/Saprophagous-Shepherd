using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Dsp;
using NAudio.Wave;

namespace MiniJam61Egypt.Sound
{
    public class LowHighShelfSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider sourceProvider;

        private BiQuadFilter LowPassFilter;
        private BiQuadFilter HighPassFilter;

        public bool Enabled { get; set; }

        private float _lowCutoff;
        public float LowShelfCutoff
        {
            get => _lowCutoff;
            set
            {
                _lowCutoff = value;
                UpdateFilters();
            }
        }

        private float _highCutoff;
        public float HighShelfCutoff
        {
            get => _highCutoff;
            set
            {
                _highCutoff = value;
                UpdateFilters();
            }
        }

        private float _lowGain;
        public float LowShelfGain
        {
            get => _lowGain;
            set
            {
                _lowGain = value;
                UpdateFilters();
            }
        }

        private float _highGain;
        public float HighShelfGain
        {
            get => _highGain;
            set
            {
                _highGain = value;
                UpdateFilters();
            }
        }

        private void UpdateFilters()
        {
            LowPassFilter = BiQuadFilter.LowShelf(sourceProvider.WaveFormat.SampleRate, LowShelfCutoff, 0.5f, LowShelfGain);
            HighPassFilter = BiQuadFilter.HighShelf(sourceProvider.WaveFormat.SampleRate, HighShelfCutoff, 0.5f, HighShelfGain);
        }

        public LowHighShelfSampleProvider(ISampleProvider sourceProvider)
        {
            this.sourceProvider = sourceProvider;

            _lowCutoff = 0f;
            LowPassFilter = BiQuadFilter.LowShelf(sourceProvider.WaveFormat.SampleRate, LowShelfCutoff, 0.5f, LowShelfGain);

            _highCutoff = 22000f;
            HighPassFilter = BiQuadFilter.HighShelf(sourceProvider.WaveFormat.SampleRate, HighShelfCutoff, 0.5f, HighShelfGain);

            Enabled = true;
        }

        public WaveFormat WaveFormat => sourceProvider.WaveFormat;

        private float _prevLow;
        private float _prevHigh;
        private float _prevLowGain;
        private float _prevHighGain;

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = sourceProvider.Read(buffer, offset, count);

            if (!Enabled) return samplesRead;

            if (_highCutoff == _prevHigh && _lowCutoff == _prevLow && _highGain == _prevHighGain && _lowGain == _prevLowGain)
            {
                for (int n = 0; n < samplesRead; n++)
                {
                    buffer[offset + n] = LowPassFilter.Transform(HighPassFilter.Transform(buffer[offset + n]));
                }
            }
            else
            {
                for (int n = 0; n < samplesRead; n++)
                {
                    float progress = n / (float)samplesRead;

                    float _tempLow = Lerp(_prevLow, _lowCutoff, progress);
                    float _tempHigh = Lerp(_prevHigh, _highCutoff, progress);
                    float _tempLowGain = Lerp(_prevLow, _lowGain, progress);
                    float _tempHighGain = Lerp(_prevHighGain, _highGain, progress);

                    BiQuadFilter low = BiQuadFilter.LowShelf(sourceProvider.WaveFormat.SampleRate, _tempLow, 0.5f, _tempLowGain);
                    BiQuadFilter high = BiQuadFilter.HighShelf(sourceProvider.WaveFormat.SampleRate, _tempHigh, 0.5f, _tempHighGain);

                    buffer[offset + n] = low.Transform(high.Transform(buffer[offset + n]));
                }
            }

            _prevHigh = _highCutoff;
            _prevLow = _lowCutoff;
            _prevHighGain = _highGain;
            _prevLowGain = _lowGain;

            return samplesRead;
        }

        private float Lerp(float a, float b, float c)
        {
            return a + (b - a) * c;
        }
    }
}
