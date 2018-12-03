using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Input.Devices;
using TheraEngine.ThirdParty;

namespace TheraEngine.Components.Scene.Transforms
{
    [TFileDef("Rotational Noise Component")]
    public class NoiseRotationComponent : SceneComponent
    {
        public NoiseRotationComponent() { }
        public NoiseRotationComponent(float maxPitch, float maxYaw, float maxRoll, float noiseFreq, float currentShakeIntensity)
        {
            MaxPitch = maxPitch;
            MaxYaw = maxYaw;
            MaxRoll = maxRoll;
            NoiseFrequency = noiseFreq;
            ShakeIntensity = currentShakeIntensity;
        }

        public float ShakeIntensity { get; set; } = 0.0f;
        public float MaxYaw { get; set; } = 30.0f;
        public float MaxPitch { get; set; } = 30.0f;
        public float MaxRoll { get; set; } = 20.0f;
        public float NoiseFrequency
        {
            get => _noiseFrequency;
            set
            {
                _noiseFrequency = value;
                _noise.SetFrequency(_noiseFrequency);
            }
        }
        private float _time = 0.0f;
        private float _noiseFrequency = 500.0f;

        private Rotator _rotation = new Rotator();
        private FastNoise _noise = new FastNoise();

        public override void OnSpawned()
        {
            _noise.SetFrequency(_noiseFrequency);
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, NoiseTick, EInputPauseType.TickAlways);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, NoiseTick, EInputPauseType.TickAlways);
            base.OnDespawned();
        }
        protected virtual void NoiseTick(float delta)
        {
            _time += delta;
            _rotation.Yaw = MaxYaw * ShakeIntensity * _noise.GetPerlin(20.0f, _time);
            _rotation.Pitch = MaxPitch * ShakeIntensity * _noise.GetPerlin(21.0f, _time);
            _rotation.Roll = MaxRoll * ShakeIntensity * _noise.GetPerlin(22.0f, _time);
            RecalcLocalTransform();
        }
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = _rotation.GetMatrix();
            inverseLocalTransform = _rotation.GetInverseMatrix();
        }
    }
}
