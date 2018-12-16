using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Audio
{
    public class AudioInstance : TObject
    {
        public int ID { get; }
        public bool Valid { get; internal set; }

        internal AudioInstance(int instanceID, AudioParameters parameters)
        {
            ID = instanceID;
            Valid = true;

            if (parameters != null)
            {
                _listenerRelative = parameters.ListenerRelative.Value;
                _loop = parameters.Loop.Value;

                _efxDirectFilterGainHighFrequencyAuto = parameters.ListenerRelative.Value;
                _efxAuxiliarySendFilterGainAuto = parameters.ListenerRelative.Value;
                _efxAuxiliarySendFilterGainHighFrequencyAuto = parameters.ListenerRelative.Value;

                _efxAirAbsorptionFactor = parameters.EfxAirAbsorptionFactor.Value;
                _efxRoomRolloffFactor = parameters.EfxRoomRolloffFactor.Value;
                _efxConeOuterGainHighFrequency = parameters.EfxConeOuterGainHighFrequency.Value;

                _coneInnerAngle = parameters.ConeInnerAngle.Value;
                _coneOuterAngle = parameters.ConeOuterAngle.Value;
                _coneOuterGain = parameters.ConeOuterGain.Value;
                _pitch = parameters.Pitch.Value;
                _gain = parameters.Gain.Value;
                _minGain = parameters.MinGain.Value;
                _maxGain = parameters.MaxGain.Value;

                _referenceDistance = parameters.ReferenceDistance.Value;
                _rolloffFactor = parameters.RolloffFactor.Value;
                _maxDistance = parameters.MaxDistance.Value;

                _playbackOffsetSeconds = parameters.PlaybackOffsetSeconds.Value;

                _position = parameters.Position.Value;
                _direction = parameters.Direction.Value;
                _velocity = parameters.Velocity.Value;
            }
            else
            {
                _listenerRelative = false;
                _loop = false;

                _efxDirectFilterGainHighFrequencyAuto = false;
                _efxAuxiliarySendFilterGainAuto = false;
                _efxAuxiliarySendFilterGainHighFrequencyAuto = false;

                _efxAirAbsorptionFactor = 0.0f;
                _efxRoomRolloffFactor = 0.0f;
                _efxConeOuterGainHighFrequency = 0.0f;

                _coneInnerAngle = 0.0f;
                _coneOuterAngle = 0.0f;
                _coneOuterGain = 0.0f;
                _pitch = 0.0f;
                _gain = 0.0f;
                _minGain = 0.0f;
                _maxGain = 0.0f;

                _referenceDistance = 0.0f;
                _rolloffFactor = 0.0f;
                _maxDistance = 0.0f;

                _playbackOffsetSeconds = 0.0f;

                _position = Vec3.Zero;
                _direction = Vec3.Zero;
                _velocity = Vec3.Zero;
            }
        }

        #region Properties

        public EAudioState State => Engine.Audio.GetState(this);

        /// <summary>
        /// Determines if Position, Direction and Velocity are relative to the listener or the world.
        /// </summary>
        [Description("Determines if Position, Direction and Velocity are relative to the listener or the world.")]
        public bool ListenerRelative
        {
            get => _listenerRelative;
            set
            {
                _listenerRelative = value;
                Engine.Audio.UpdateSourceListenerRelative(this);
            }
        }
        /// <summary>
        /// Determines if the source plays again upon ending.
        /// </summary>
        [Description("Determines if the source plays again upon ending.")]
        public bool Loop
        {
            get => _loop;
            set
            {
                _loop = value;
                Engine.Audio.UpdateSourceLoop(this);
            }
        }
        /// <summary>
        /// (EFX Extension) If this Source property is set to True, this Source’s direct-path
        /// is automatically filtered according to the orientation of the source relative
        /// to the listener and the setting of the Source property Sourcef.ConeOuterGainHF.
        /// Type: bool Range [False, True] Default: True
        /// </summary>
        [Description(
            "(EFX Extension) If this Source property is set to True, this Source’s direct-path " +
            "is automatically filtered according to the orientation of the source relative " +
            "to the listener and the setting of the Source property Sourcef.ConeOuterGainHF. " +
            "Type: bool Range [False, True] Default: True")]
        public bool EfxDirectFilterGainHighFrequencyAuto
        {
            get => _efxDirectFilterGainHighFrequencyAuto;
            set
            {
                _efxDirectFilterGainHighFrequencyAuto = value;
                Engine.Audio.UpdateSourceEfxDirectFilterGainHighFrequencyAuto(this);
            }
        }
        /// <summary>
        /// (EFX Extension) If this Source property is set to True, the intensity of this
        /// Source’s reflected sound is automatically attenuated according to source-listener
        /// distance and source directivity (as determined by the cone parameters). If it
        /// is False, the reflected sound is not attenuated according to distance and directivity.
        /// Type: bool Range [False, True] Default: True
        /// </summary>
        [Description(
            "(EFX Extension) If this Source property is set to True, the intensity of this " +
            "Source’s reflected sound is automatically attenuated according to source-listener " +
            "distance and source directivity (as determined by the cone parameters). If it " +
            "is False, the reflected sound is not attenuated according to distance and directivity. " +
            "Type: bool Range [False, True] Default: True")]
        public bool EfxAuxiliarySendFilterGainAuto
        {
            get => _efxAuxiliarySendFilterGainAuto;
            set
            {
                _efxAuxiliarySendFilterGainAuto = value;
                Engine.Audio.UpdateSourceEfxAuxiliarySendFilterGainAuto(this);
            }
        }
        /// <summary>
        /// (EFX Extension) If this Source property is AL_TRUE (its default value), the intensity
        /// of this Source’s reflected sound at high frequencies will be automatically attenuated
        /// according to the high-frequency source directivity as set by the Sourcef.ConeOuterGainHF
        /// property. If this property is AL_FALSE, the Source’s reflected sound is not filtered
        /// at all according to the Source’s directivity. Type: bool Range [False, True]
        /// Default: True
        /// </summary>
        public bool EfxAuxiliarySendFilterGainHighFrequencyAuto
        {
            get => _efxAuxiliarySendFilterGainHighFrequencyAuto;
            set
            {
                _efxAuxiliarySendFilterGainHighFrequencyAuto = value;
                Engine.Audio.UpdateSourceEfxAuxiliarySendFilterGainHighFrequencyAuto(this);
            }
        }
        /// <summary>
        /// (EFX Extension) This property is a multiplier on the amount of Air Absorption
        /// applied to the Source. The AL_AIR_ABSORPTION_FACTOR is multiplied by an internal
        /// Air Absorption Gain HF value of 0.994 (-0.05dB) per meter which represents normal
        /// atmospheric humidity and temperature. Range [0.0f .. 10.0f] Default: 0.0f
        /// </summary>
        public float EfxAirAbsorptionFactor
        {
            get => _efxAirAbsorptionFactor;
            set
            {
                _efxAirAbsorptionFactor = value;
                Engine.Audio.UpdateSourceEfxAirAbsorptionFactor(this);
            }
        }
        /// <summary>
        /// (EFX Extension) This property is defined the same way as the Reverb Room Rolloff
        /// property: it is one of two methods available in the Effect Extension to attenuate
        /// the reflected sound (early reflections and reverberation) according to source-listener
        /// distance. Range [0.0f .. 10.0f] Default: 0.0f
        /// </summary>
        public float EfxRoomRolloffFactor
        {
            get => _efxRoomRolloffFactor;
            set
            {
                _efxRoomRolloffFactor = value;
                Engine.Audio.UpdateSourceEfxRoomRolloffFactor(this);
            }
        }
        /// <summary>
        /// (EFX Extension) A directed Source points in a specified direction. The Source
        /// sounds at full volume when the listener is directly in front of the source; it
        /// is attenuated as the listener circles the Source away from the front. Range [0.0f .. 1.0f] Default: 1.0f
        /// </summary>
        public float EfxConeOuterGainHighFrequency
        {
            get => _efxConeOuterGainHighFrequency;
            set
            {
                _efxConeOuterGainHighFrequency = value;
                Engine.Audio.UpdateSourceEfxConeOuterGainHighFrequency(this);
            }
        }
        /// <summary>
        /// Directional Source, inner cone angle, in degrees. Range: [0-360] Default: 360
        /// </summary>
        public float ConeInnerAngle
        {
            get => _coneInnerAngle;
            set
            {
                _coneInnerAngle = value;
                Engine.Audio.UpdateSourceConeInnerAngle(this);
            }
        }
        /// <summary>
        /// Directional Source, outer cone angle, in degrees. Range: [0-360] Default: 360
        /// </summary>
        public float ConeOuterAngle
        {
            get => _coneOuterAngle;
            set
            {
                _coneOuterAngle = value;
                Engine.Audio.UpdateSourceConeOuterAngle(this);
            }
        }
        /// <summary>
        /// Specify the pitch to be applied, either at Source, or on mixer results, at Listener.
        /// Range: [0.5f - 2.0f] Default: 1.0f
        /// </summary>
        public float Pitch
        {
            get => _pitch;
            set
            {
                _pitch = value;
                Engine.Audio.UpdateSourcePitch(this);
            }
        }
        /// <summary>
        /// Indicate the gain (volume amplification) applied. Type: float. Range: [0.0f - ? ] 
        /// A value of 1.0 means un-attenuated/unchanged. Each division by 2 equals an
        /// attenuation of -6dB. Each multiplicaton with 2 equals an amplification of +6dB.
        /// A value of 0.0f is meaningless with respect to a logarithmic scale; it is interpreted
        /// as zero volume - the channel is effectively disabled.
        /// </summary>
        public float Gain
        {
            get => _gain;
            set
            {
                _gain = value;
                Engine.Audio.UpdateSourceGain(this);
            }
        }
        /// <summary>
        /// Indicate minimum Source attenuation. Type: float Range: [0.0f - 1.0f] (Logarithmic)
        /// </summary>
        public float MinGain
        {
            get => _minGain;
            set
            {
                _minGain = value;
                Engine.Audio.UpdateSourceMinGain(this);
            }
        }
        /// <summary>
        /// Indicate maximum Source attenuation. Type: float Range: [0.0f - 1.0f] (Logarithmic)
        /// </summary>
        public float MaxGain
        {
            get => _maxGain;
            set
            {
                _maxGain = value;
                Engine.Audio.UpdateSourceMaxGain(this);
            }
        }
        /// <summary>
        /// Source specific reference distance. Type: float Range: [0.0f - float.PositiveInfinity]
        /// At 0.0f, no distance attenuation occurs. Type: float Default: 1.0f.
        /// </summary>
        public float ReferenceDistance
        {
            get => _referenceDistance;
            set
            {
                _referenceDistance = value;
                Engine.Audio.UpdateSourceReferenceDistance(this);
            }
        }
        /// <summary>
        /// Source specific rolloff factor. Type: float Range: [0.0f - float.PositiveInfinity]
        /// </summary>
        public float RolloffFactor
        {
            get => _rolloffFactor;
            set
            {
                _rolloffFactor = value;
                Engine.Audio.UpdateSourceRolloffFactor(this);
            }
        }
        /// <summary>
        /// Directional Source, outer cone gain. Default: 0.0f Range: [0.0f - 1.0] (Logarithmic)
        /// </summary>
        public float ConeOuterGain
        {
            get => _coneOuterGain;
            set
            {
                _coneOuterGain = value;
                Engine.Audio.UpdateSourceConeOuterGain(this);
            }
        }
        /// <summary>
        /// Indicate distance above which Sources are not attenuated using the inverse clamped distance model.
        /// Default: float.PositiveInfinity Type: float Range: [0.0f - float.PositiveInfinity]
        /// </summary>
        public float MaxDistance
        {
            get => _maxDistance;
            set
            {
                _maxDistance = value;
                Engine.Audio.UpdateSourceMaxDistance(this);
            }
        }
        /// <summary>
        /// The playback position, expressed in seconds.
        /// </summary>
        [Description("The playback position, expressed in seconds.")]
        public float PlaybackOffsetSeconds
        {
            get => _playbackOffsetSeconds;
            set
            {
                _playbackOffsetSeconds = value;
                Engine.Audio.UpdateSourcePlaybackOffsetSeconds(this);
            }
        }
        /// <summary>
        /// The position of the audio in world or listener relative space.
        /// </summary>
        [Description("The position of the audio in world or listener relative space.")]
        public Vec3 Position
        {
            get => _position;
            set
            {
                _position = value;
                Engine.Audio.UpdateSourcePosition(this);
            }
        }
        /// <summary>
        /// The direction the sound is facing.
        /// </summary>
        [Description("The direction the sound is facing.")]
        public Vec3 Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                Engine.Audio.UpdateSourceDirection(this);
            }
        }
        /// <summary>
        /// How fast the the sound is moving.
        /// </summary>
        [Description("How fast the the sound is moving.")]
        public Vec3 Velocity
        {
            get => _velocity;
            set
            {
                _velocity = value;
                Engine.Audio.UpdateSourceVelocity(this);
            }
        }

        #endregion

        #region Fields
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
        #endregion

        public void Play()  => Engine.Audio.Play (this);
        public void Pause() => Engine.Audio.Pause(this);
        public void Stop()  => Engine.Audio.Stop (this);

        public void UpdateAllParameters(bool force = false) 
            => Engine.Audio.UpdateSource(this, force);
    }
}
