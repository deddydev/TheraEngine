﻿using System;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Audio
{
    [TFileDef("Audio Parameters", "Describes how a raw audio file should sound within the engine.")]
    [TFileExt("aup")]
    public class AudioParameters : TFileObject
    {
        /// <summary>
        /// Determines if Position, Direction and Velocity are relative to the listener or the world.
        /// </summary>
        public UsableValue<bool> ListenerRelative { get; set; } = new UsableValue<bool>(false, false, false);
        /// <summary>
        /// Determines if the source plays again upon ending.
        /// </summary>
        public UsableValue<bool> Loop { get; set; } = new UsableValue<bool>(false, false, false);
        /// <summary>
        /// (EFX Extension) If this Source property is set to True, this Source’s direct-path
        /// is automatically filtered according to the orientation of the source relative
        /// to the listener and the setting of the Source property Sourcef.ConeOuterGainHF.
        /// Type: bool Range [False, True] Default: True
        /// </summary>
        public UsableValue<bool> EfxDirectFilterGainHighFrequencyAuto { get; set; } = new UsableValue<bool>(true, true, false);
        /// <summary>
        /// (EFX Extension) If this Source property is set to True, the intensity of this
        /// Source’s reflected sound is automatically attenuated according to source-listener
        /// distance and source directivity (as determined by the cone parameters). If it
        /// is False, the reflected sound is not attenuated according to distance and directivity.
        /// Type: bool Range [False, True] Default: True
        /// </summary>
        public UsableValue<bool> EfxAuxiliarySendFilterGainAuto { get; set; } = new UsableValue<bool>(true, true, false);
        /// <summary>
        /// (EFX Extension) If this Source property is AL_TRUE (its default value), the intensity
        /// of this Source’s reflected sound at high frequencies will be automatically attenuated
        /// according to the high-frequency source directivity as set by the Sourcef.ConeOuterGainHF
        /// property. If this property is AL_FALSE, the Source’s reflected sound is not filtered
        /// at all according to the Source’s directivity. Type: bool Range [False, True]
        /// Default: True
        /// </summary>
        public UsableValue<bool> EfxAuxiliarySendFilterGainHighFrequencyAuto { get; set; } = new UsableValue<bool>(true, true, false);
        /// <summary>
        /// Directional Source, inner cone angle, in degrees. Range: [0-360] Default: 360
        /// </summary>
        public UsableValue<float> ConeInnerAngle { get; set; } = new UsableValue<float>(360.0f, 360.0f, false);
        /// <summary>
        /// Directional Source, outer cone angle, in degrees. Range: [0-360] Default: 360
        /// </summary>
        public UsableValue<float> ConeOuterAngle { get; set; } = new UsableValue<float>(360.0f, 360.0f, false);
        /// <summary>
        /// Specify the pitch to be applied, either at Source, or on mixer results, at Listener.
        /// Range: [0.5f - 2.0f] Default: 1.0f
        /// </summary>
        public UsableValue<float> Pitch { get; set; } = new UsableValue<float>(1.0f, 1.0f, false);
        /// <summary>
        /// Indicate the gain (volume amplification) applied. Type: float. Range: [0.0f - ? ] 
        /// A value of 1.0 means un-attenuated/unchanged. Each division by 2 equals an
        /// attenuation of -6dB. Each multiplicaton with 2 equals an amplification of +6dB.
        /// A value of 0.0f is meaningless with respect to a logarithmic scale; it is interpreted
        /// as zero volume - the channel is effectively disabled.
        /// </summary>
        public UsableValue<float> Gain { get; set; } = new UsableValue<float>(1.0f, 1.0f, false);
        /// <summary>
        /// Indicate minimum Source attenuation. Type: float Range: [0.0f - 1.0f] (Logarithmic)
        /// </summary>
        public UsableValue<float> MinGain { get; set; } = new UsableValue<float>(0.0f, 0.0f, false);
        /// <summary>
        /// Indicate maximum Source attenuation. Type: float Range: [0.0f - 1.0f] (Logarithmic)
        /// </summary>
        public UsableValue<float> MaxGain { get; set; } = new UsableValue<float>(1.0f, 1.0f, false);
        /// <summary>
        /// Source specific reference distance. Type: float Range: [0.0f - float.PositiveInfinity]
        /// At 0.0f, no distance attenuation occurs. Type: float Default: 1.0f.
        /// </summary>
        public UsableValue<float> ReferenceDistance { get; set; } = new UsableValue<float>(1.0f, 1.0f, false);
        /// <summary>
        /// Source specific rolloff factor. Type: float Range: [0.0f - float.PositiveInfinity]
        /// </summary>
        public UsableValue<float> RolloffFactor { get; set; } = new UsableValue<float>(1.0f, 1.0f, false);
        /// <summary>
        /// Directional Source, outer cone gain. Default: 0.0f Range: [0.0f - 1.0] (Logarithmic)
        /// </summary>
        public UsableValue<float> ConeOuterGain { get; set; } = new UsableValue<float>(0.0f, 0.0f, false);
        /// <summary>
        /// Indicate distance above which Sources are not attenuated using the inverse clamped distance model.
        /// Default: float.PositiveInfinity Type: float Range: [0.0f - float.PositiveInfinity]
        /// </summary>
        public UsableValue<float> MaxDistance { get; set; } = new UsableValue<float>(float.MaxValue, float.MaxValue, false);
        /// <summary>
        /// The playback position, expressed in seconds.
        /// </summary>
        public UsableValue<float> PlaybackOffsetSeconds { get; set; } = new UsableValue<float>(0.0f, 0.0f, false);
        /// <summary>
        /// (EFX Extension) This property is a multiplier on the amount of Air Absorption
        /// applied to the Source. The AL_AIR_ABSORPTION_FACTOR is multiplied by an internal
        /// Air Absorption Gain HF value of 0.994 (-0.05dB) per meter which represents normal
        /// atmospheric humidity and temperature. Range [0.0f .. 10.0f] Default: 0.0f
        /// </summary>
        public UsableValue<float> EfxAirAbsorptionFactor { get; set; } = new UsableValue<float>(0.0f, 0.0f, false);
        /// <summary>
        /// (EFX Extension) This property is defined the same way as the Reverb Room Rolloff
        /// property: it is one of two methods available in the Effect Extension to attenuate
        /// the reflected sound (early reflections and reverberation) according to source-listener
        /// distance. Range [0.0f .. 10.0f] Default: 0.0f
        /// </summary>
        public UsableValue<float> EfxRoomRolloffFactor { get; set; } = new UsableValue<float>(0.0f, 0.0f, false);
        /// <summary>
        /// (EFX Extension) A directed Source points in a specified direction. The Source
        /// sounds at full volume when the listener is directly in front of the source; it
        /// is attenuated as the listener circles the Source away from the front. Range [0.0f .. 1.0f] Default: 1.0f
        /// </summary>
        public UsableValue<float> EfxConeOuterGainHighFrequency { get; set; } = new UsableValue<float>(1.0f, 1.0f, false);
        /// <summary>
        /// The position of the audio in world or listener relative space.
        /// </summary>
        public UsableValue<Vec3> Position { get; set; } = new UsableValue<Vec3>(Vec3.Zero, Vec3.Zero, false);
        /// <summary>
        /// The direction the sound is facing.
        /// </summary>
        public UsableValue<Vec3> Direction { get; set; } = new UsableValue<Vec3>(Vec3.Zero, Vec3.Zero, false);
        /// <summary>
        /// How fast the the sound is moving.
        /// </summary>
        public UsableValue<Vec3> Velocity { get; set; } = new UsableValue<Vec3>(Vec3.Zero, Vec3.Zero, false);
    }
    [Serializable]
    public class UsableValue<T> where T : struct
    {
        public UsableValue(T value, T defaultValue, bool use = false)
        {
            DefaultValue = defaultValue;
            _value = value;
            UseOverrideValue = use;
        }

        private T _value;

        public bool UseOverrideValue { get; set; }
        public T Value => UseOverrideValue ? _value : DefaultValue;
        public T DefaultValue { get; }
        public T OverrideValue
        {
            get => _value;
            set
            {
                _value = value;
                UseOverrideValue = !_value.Equals(DefaultValue);
            }
        }

        public static implicit operator UsableValue<T>(T value) => new UsableValue<T>(value, value);
        public static implicit operator UsableValue<T>(bool use) => new UsableValue<T>(default, default, use);
        public static implicit operator T(UsableValue<T> value)
        {
            if (!value.UseOverrideValue)
                throw new Exception("Value not set.");
            return value.OverrideValue;
        }
    }
}