using System;
using System.ComponentModel;
using TheraEngine.Input.Devices;

namespace TheraEngine.Worlds.Actors.Components.Scene.Transforms
{
    [FileDef("3D Screen Shake Component")]
    public class ScreenShake3DComponent : NoiseRotationComponent
    {
        private float _trauma;
        private float _maxTrauma;
        private float _traumaDecrement;
        
        /// <summary>
        /// How much the amount of screenshake should decrease every second.
        /// </summary>
        public float TraumaDecrementPerSecond
        {
            get => _traumaDecrement;
            set => _traumaDecrement = value;
        }
        /// <summary>
        /// The maximum amount of screenshake allowed.
        /// </summary>
        public float MaxTrauma
        {
            get => _maxTrauma;
            set => _maxTrauma = value;
        }
        /// <summary>
        /// Add to this value to increase the amount of screenshake.
        /// </summary>
        public float Trauma
        {
            get => _trauma;
            set
            {
                _trauma = value.Clamp(0.0f, MaxTrauma);
                float normalized = _trauma / MaxTrauma;
                ShakeIntensity = normalized * normalized;
            }
        }
        protected override void NoiseTick(float delta)
        {
            Trauma -= TraumaDecrementPerSecond * delta;
            base.NoiseTick(delta);
        }
    }
}
