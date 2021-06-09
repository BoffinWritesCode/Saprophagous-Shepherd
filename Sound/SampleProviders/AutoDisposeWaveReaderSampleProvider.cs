using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;
using NAudio.Vorbis;

namespace MiniJam61Egypt.Sound
{
    public class AutoDisposeWaveReaderSampleProvider<T> : ISampleProvider where T : WaveStream, ISampleProvider
    {
        private readonly T reader;
        private bool isDisposed;

        public AutoDisposeWaveReaderSampleProvider(T reader)
        {
            this.reader = reader;
            this.WaveFormat = reader.WaveFormat;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            if (isDisposed)
            {
                return 0;
            }

            int read = reader.Read(buffer, offset, count);
            if (read == 0)
            {
                reader.Dispose();
                isDisposed = true;
            }

            return read;
        }

        public WaveFormat WaveFormat { get; private set; }
    }
}
