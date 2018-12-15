using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Audio
{
    public struct AudioInstance
    {
        public int ID { get; }
        internal AudioInstance(int instanceID, AudioParameters parameters)
        {
            ID = instanceID;
            _listenerRelative = parameters.ListenerRelative.Value;
            _listenerRelative = parameters.ListenerRelative.Value;
            _listenerRelative = parameters.ListenerRelative.Value;
            _listenerRelative = parameters.ListenerRelative.Value;
            _listenerRelative = parameters.ListenerRelative.Value;
        }

        /// <summary>
        /// Determines if Position, Direction and Velocity are relative to the listener or the world.
        /// </summary>
        public bool ListenerRelative
        {
            get => _listenerRelative;
            set
            {
                _listenerRelative = value;
                Engine.Audio.UpdateInstanceSourceListenerRelative(this, value);
            }
        }
        /// <summary>
        /// Determines if the source plays again upon ending.
        /// </summary>
        public bool Loop { get; set; }
        /// <summary>
        /// (EFX Extension) If this Source property is set to True, this Source’s direct-path
        /// is automatically filtered according to the orientation of the source relative
        /// to the listener and the setting of the Source property Sourcef.ConeOuterGainHF.
        /// Type: bool Range [False, True] Default: True
        /// </summary>
        public bool EfxDirectFilterGainHighFrequencyAuto { get; set; }
        /// <summary>
        /// (EFX Extension) If this Source property is set to True, the intensity of this
        /// Source’s reflected sound is automatically attenuated according to source-listener
        /// distance and source directivity (as determined by the cone parameters). If it
        /// is False, the reflected sound is not attenuated according to distance and directivity.
        /// Type: bool Range [False, True] Default: True
        /// </summary>
        public bool EfxAuxiliarySendFilterGainAuto { get; set; }
        /// <summary>
        /// (EFX Extension) If this Source property is AL_TRUE (its default value), the intensity
        /// of this Source’s reflected sound at high frequencies will be automatically attenuated
        /// according to the high-frequency source directivity as set by the Sourcef.ConeOuterGainHF
        /// property. If this property is AL_FALSE, the Source’s reflected sound is not filtered
        /// at all according to the Source’s directivity. Type: bool Range [False, True]
        /// Default: True
        /// </summary>
        public bool EfxAuxiliarySendFilterGainHighFrequencyAuto { get; set; }
        /// <summary>
        /// Directional Source, inner cone angle, in degrees. Range: [0-360] Default: 360
        /// </summary>
        public float ConeInnerAngle { get; set; }
        /// <summary>
        /// Directional Source, outer cone angle, in degrees. Range: [0-360] Default: 360
        /// </summary>
        public float ConeOuterAngle { get; set; }
        /// <summary>
        /// Specify the pitch to be applied, either at Source, or on mixer results, at Listener.
        /// Range: [0.5f - 2.0f] Default: 1.0f
        /// </summary>
        public float Pitch { get; set; }
        /// <summary>
        /// Indicate the gain (volume amplification) applied. Type: float. Range: [0.0f - ? ] 
        /// A value of 1.0 means un-attenuated/unchanged. Each division by 2 equals an
        /// attenuation of -6dB. Each multiplicaton with 2 equals an amplification of +6dB.
        /// A value of 0.0f is meaningless with respect to a logarithmic scale; it is interpreted
        /// as zero volume - the channel is effectively disabled.
        /// </summary>
        public float Gain { get; set; }
        /// <summary>
        /// Indicate minimum Source attenuation. Type: float Range: [0.0f - 1.0f] (Logarithmic)
        /// </summary>
        public float MinGain { get; set; }
        /// <summary>
        /// Indicate maximum Source attenuation. Type: float Range: [0.0f - 1.0f] (Logarithmic)
        /// </summary>
        public float MaxGain { get; set; }
        /// <summary>
        /// Source specific reference distance. Type: float Range: [0.0f - float.PositiveInfinity]
        /// At 0.0f, no distance attenuation occurs. Type: float Default: 1.0f.
        /// </summary>
        public float ReferenceDistance { get; set; }
        /// <summary>
        /// Source specific rolloff factor. Type: float Range: [0.0f - float.PositiveInfinity]
        /// </summary>
        public float RolloffFactor { get; set; }
        /// <summary>
        /// Directional Source, outer cone gain. Default: 0.0f Range: [0.0f - 1.0] (Logarithmic)
        /// </summary>
        public float ConeOuterGain { get; set; }
        /// <summary>
        /// Indicate distance above which Sources are not attenuated using the inverse clamped distance model.
        /// Default: float.PositiveInfinity Type: float Range: [0.0f - float.PositiveInfinity]
        /// </summary>
        public float MaxDistance { get; set; }
        /// <summary>
        /// The playback position, expressed in seconds.
        /// </summary>
        public float PlaybackOffsetSeconds { get; set; }
        /// <summary>
        /// (EFX Extension) This property is a multiplier on the amount of Air Absorption
        /// applied to the Source. The AL_AIR_ABSORPTION_FACTOR is multiplied by an internal
        /// Air Absorption Gain HF value of 0.994 (-0.05dB) per meter which represents normal
        /// atmospheric humidity and temperature. Range [0.0f .. 10.0f] Default: 0.0f
        /// </summary>
        public float EfxAirAbsorptionFactor { get; set; }
        /// <summary>
        /// (EFX Extension) This property is defined the same way as the Reverb Room Rolloff
        /// property: it is one of two methods available in the Effect Extension to attenuate
        /// the reflected sound (early reflections and reverberation) according to source-listener
        /// distance. Range [0.0f .. 10.0f] Default: 0.0f
        /// </summary>
        public float EfxRoomRolloffFactor { get; set; }
        /// <summary>
        /// (EFX Extension) A directed Source points in a specified direction. The Source
        /// sounds at full volume when the listener is directly in front of the source; it
        /// is attenuated as the listener circles the Source away from the front. Range [0.0f .. 1.0f] Default: 1.0f
        /// </summary>
        public float EfxConeOuterGainHighFrequency { get; set; }
        /// <summary>
        /// The position of the audio in world or listener relative space.
        /// </summary>
        public Vec3 Position { get; set; }
        /// <summary>
        /// The direction the sound is facing.
        /// </summary>
        public Vec3 Direction { get; set; }
        /// <summary>
        /// How fast the the sound is moving.
        /// </summary>
        public Vec3 Velocity { get; set; }

        public EAudioState State
        {
            get => Engine.Audio.GetState(this);
            set => Engine.Audio.sta
        }

        public bool _listenerRelative;
        public bool _loop;

        public bool _efxDirectFilterGainHighFrequencyAuto;
        public bool _efxAuxiliarySendFilterGainAuto;
        public bool _efxAuxiliarySendFilterGainHighFrequencyAuto;
        public float _efxAirAbsorptionFactor;
        public float _efxRoomRolloffFactor;
        public float _efxConeOuterGainHighFrequency;

        public float _coneInnerAngle;
        public float _coneOuterAngle;
        public float _pitch;
        public float _gain;
        public float _minGain;
        public float _maxGain;
        public float _referenceDistance;
        public float _rolloffFactor;
        public float _coneOuterGain;
        public float _maxDistance;
        public float _playbackOffsetSeconds;

        public Vec3 _position;
        public Vec3 _direction;
        public Vec3 _velocity;
    }
    /// <summary>
    /// Stores audio samples and information for playing them.
    /// </summary>
    [TFileDef("Audio File")]
    [TFileExt("aud", new string[] { "wav"/*, "mp3", "ogg", "flac"*/ }, null)]
    public class AudioFile : TFileObject
    {
        [TSerialize(IsElementString = true)]
        public byte[] Samples { get; private set; }
        [TSerialize(IsAttribute = true)]
        public int Channels { get; private set; }
        [TSerialize(IsAttribute = true)]
        public int BitsPerSample { get; private set; }
        [TSerialize(IsAttribute = true)]
        public int SampleRate { get; private set; }

        [Browsable(false)]
        public HashSet<AudioInstance> Instances { get; } = new HashSet<AudioInstance>();
        [Browsable(false)]
        public int BufferId { get; internal set; }

        public override void ManualRead3rdParty(string filePath)
        {
            string ext = filePath.GetExtensionLowercase();
            switch (ext)
            {
                case "wav":

                    Samples = WaveFile.ReadSamples(filePath,
                        out int channels, out int bps, out int sampleRate);

                    Channels = channels;
                    BitsPerSample = bps;
                    SampleRate = sampleRate;

                    break;
            }
        }
    }
}
