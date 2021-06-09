using System;
using System.Collections.Generic;
using System.Text;

using NAudio.Wave;

namespace MiniJam61Egypt.Sound
{
    public class LoopStream : WaveStream
    {
        private WaveStream _source;
        private long _loopStart;
        private long _loopEnd;

        public bool DoLooping { get; set; }

        /// <summary>
        /// Creates a new Loop stream
        /// </summary>
        /// <param name="sourceStream">The stream to read from. Note: the Read method of this stream should return 0 when it reaches the end</param>
        /// <param name="loopStart">The time (in seconds) at which to loop back to when the end is reached.</param>
        /// <param name="loopEnd">The time (in seconds) at which to loop back when reached. Leave as 0 for end of the source.</param>
        public LoopStream(WaveStream sourceStream, double loopStart = 0, double loopEnd = 0)
        {
            this._source = sourceStream;
            DoLooping = true;

            int sRate = sourceStream.WaveFormat.SampleRate;
            int chans = sourceStream.WaveFormat.Channels;
            int size = sourceStream.WaveFormat.BitsPerSample / 8;

            long mult = sRate * chans * size;
            _loopStart = (long)(loopStart * mult);
            _loopEnd = (long)(loopEnd * mult);
        }

        public override WaveFormat WaveFormat
        {
            get { return _source.WaveFormat; }
        }

        public override long Length
        {
            get { return _source.Length; }
        }

        public override long Position
        {
            get { return _source.Position; }
            set { _source.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int toRead = count - totalBytesRead;
                bool resetLoop = false;

                //Only read enough bytes to reach the end of the loop if necessary.
                if (_loopEnd != 0 && DoLooping && _source.Position + toRead >= _loopEnd)
                {
                    resetLoop = true;
                    toRead = (int)(_loopEnd - _source.Position);
                }

                int bytesRead = _source.Read(buffer, offset + totalBytesRead, toRead);
                if (bytesRead == 0 || resetLoop)
                {
                    if (_source.Position == 0)
                    {
                        break;
                    }
                    _source.Position = _loopStart;
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }
    }
}
