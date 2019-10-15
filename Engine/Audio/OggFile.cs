using System;
using System.IO;

namespace TheraEngine.Audio
{
    public static class OggFile
    {
        public static byte[] ReadSamples(
            string filePath,
            out int channels,
            out int bitsPerSample,
            out int sampleRate)
        {
            if (!File.Exists(filePath))
            {
                channels = 0;
                bitsPerSample = 0;
                sampleRate = 0;

                return null;
            }

            using (var vorbis = new NVorbis.VorbisReader(filePath))
            {
                // get the channels & sample rate
                channels = vorbis.Channels;
                sampleRate = vorbis.SampleRate;
                bitsPerSample = 32;

                // OPTIONALLY: get a TimeSpan indicating the total length of the Vorbis stream
                var totalTime = vorbis.TotalTime;

                // create a buffer for reading samples
                var readBuffer = new float[channels * sampleRate / 5];  // 200ms

                // get the initial position (obviously the start)
                var position = TimeSpan.Zero;

                // go grab samples
                int cnt;
                while ((cnt = vorbis.ReadSamples(readBuffer, 0, readBuffer.Length)) > 0)
                {
                    // do stuff with the buffer
                    // samples are interleaved (chan0, chan1, chan0, chan1, etc.)
                    // sample value range is -0.99999994f to 0.99999994f unless vorbis.ClipSamples == false

                    // OPTIONALLY: get the position we just read through to...
                    position = vorbis.DecodedTime;
                }

                return null;
            }
        }
    }
}
