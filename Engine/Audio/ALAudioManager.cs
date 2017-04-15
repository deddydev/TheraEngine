using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System;
using System.Diagnostics;
using System.Threading;

namespace CustomEngine.Audio
{
    public class ALAudioManager : AbstractAudioManager
    {
        private AudioContext _context;
        public ALAudioManager()
        {
            _context = new AudioContext();
        }
        ~ALAudioManager()
        {
            _context.Dispose();
        }
        private static ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }
        }
        public override void Pause(SoundDataBase sound)
        {

        }

        private void ApplyParameters(int source, AudioParameters param)
        {
            AL.Source(source, ALSource3f.Position, param.Position._value);
        }

        public override void Play(SoundDataBase sound, AudioParameters param)
        {
            sound.BufferId = AL.GenBuffer();
            sound.SourceId = AL.GenSource();
            int state;
            
            byte[] data = sound.WaveFile.SoundData;
            AL.BufferData(sound.BufferId, 
                GetSoundFormat(sound.WaveFile.Channels, sound.WaveFile.BitsPerSample),
                data, data.Length, sound.WaveFile.SampleRate);

            AL.Source(sound.SourceId, ALSourcei.Buffer, sound.BufferId);
            ApplyParameters(sound.SourceId, param);
            AL.SourcePlay(sound.SourceId);
            
            do
            {
                Thread.Sleep(250);
                AL.GetSource(sound.SourceId, ALGetSourcei.SourceState, out state);
            }
            while ((ALSourceState)state == ALSourceState.Playing);
            
        }

        public override void Stop(SoundDataBase sound)
        {
            AL.SourceStop(sound.SourceId);
            AL.DeleteSource(sound.SourceId);
            AL.DeleteBuffer(sound.BufferId);
        }
    }
}
