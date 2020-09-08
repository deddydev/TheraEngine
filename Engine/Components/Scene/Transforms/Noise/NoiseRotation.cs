using System;
using TheraEngine.ComponentModel;
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

        private readonly Rotator _rotation = new Rotator();
        private readonly FastNoise _noise = new FastNoise();

        protected override void OnSpawned()
        {
            _noise.SetFrequency(_noiseFrequency);
            RegisterTick(ETickGroup.DuringPhysics, ETickOrder.Logic, NoiseTick, EInputPauseType.TickAlways);
            base.OnSpawned();
        }
        protected override void OnDespawned()
        {
            UnregisterTick(ETickGroup.DuringPhysics, ETickOrder.Logic, NoiseTick, EInputPauseType.TickAlways);
            base.OnDespawned();
        }
        protected virtual void NoiseTick(float delta)
        {
            _time += delta;
            _rotation.SetRotations(
                MaxPitch * ShakeIntensity * _noise.GetPerlin(21.0f, _time),
                MaxYaw * ShakeIntensity * _noise.GetPerlin(20.0f, _time),
                MaxRoll * ShakeIntensity * _noise.GetPerlin(22.0f, _time));
            RecalcLocalTransform();
        }
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = _rotation.GetMatrix();
            inverseLocalTransform = _rotation.GetInverseMatrix();
        }
    }
}
