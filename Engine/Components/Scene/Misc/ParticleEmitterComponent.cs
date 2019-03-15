using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Particles
{
    [TFileDef("Particle Emitter Component")]
    public class ParticleEmitterComponent : TRComponent, I3DRenderable, IPreRendered
    {
        public const int MaxParticles = 100000;
        public Particle[] ParticlesContainer = new Particle[MaxParticles];
        public PrimitiveManager ParticleMesh
        {
            get => _rc.Mesh;
            set => _rc.Mesh = value;
        }
        public Vec3 CameraPosition { get; set; }
        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D();
        public bool PreRenderEnabled => IsEmitting;
        private int _lastUsedParticle = 0;

        private bool _isEmitting = true;
        public bool IsEmitting
        {
            get => _isEmitting;
            set
            {
                if (_isEmitting == value)
                    return;
                _isEmitting = value;
                if (IsSpawned)
                {
                    if (_isEmitting)
                        StartEmitting();
                    else
                        StopEmitting();
                }
            }
        }

        public override void OnSpawned()
        {
            PrimitiveData data = PrimitiveData.FromQuads(VertexShaderDesc.PosColor(), VertexQuad.PosZQuad(1, false, 0.0f, true));
            TMaterial mat = TMaterial.CreateUnlitColorMaterialForward();
            ParticleMesh = new PrimitiveManager(data, mat);

            if (_isEmitting)
                StartEmitting();
            else
                StopEmitting();
        }
        public override void OnDespawned()
        {
            if (_isEmitting)
                StopEmitting();
        }
        private void StartEmitting()
        {
            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Update);
        }
        private void StopEmitting()
        {
            UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Update);
        }
        private int FindUnusedParticle()
        {
            for (int i = _lastUsedParticle; i < MaxParticles; i++)
            {
                if (ParticlesContainer[i].Life < 0.0f)
                {
                    _lastUsedParticle = i;
                    return i;
                }
            }
            for (int i = 0; i < _lastUsedParticle; i++)
            {
                if (ParticlesContainer[i].Life < 0.0f)
                {
                    _lastUsedParticle = i;
                    return i;
                }
            }
            return 0; // All particles are taken, override the first one
        }
        private unsafe void Update(float delta)
        {
            int num = 0;
            for (int i = 0; i < MaxParticles; i++)
            {
                Particle p = ParticlesContainer[i];
                if (p.Life > 0.0f)
                {
                    p.Life -= delta;
                    if (p.Life > 0.0f)
                    {
                        p.Velocity += new Vec3(0.0f, -9.81f, 0.0f) * delta * 0.5f;
                        p.Position += p.Velocity * delta;
                        p.Distance = (p.Position - CameraPosition).LengthSquared;

                        var posBuf = ParticleMesh.Data[EBufferType.Position];
                        var colBuf = ParticleMesh.Data[EBufferType.Color];

                        int offset = num << 2;
                        float* posPtr = (float*)posBuf.Address;
                        posPtr[offset + 0] = p.Position.X;
                        posPtr[offset + 1] = p.Position.Y;
                        posPtr[offset + 2] = p.Position.Z;
                        posPtr[offset + 3] = p.Scale;

                        float* colPtr = (float*)colBuf.Address;
                        colPtr[offset + 0] = p.Color.R;
                        colPtr[offset + 1] = p.Color.G;
                        colPtr[offset + 2] = p.Color.B;
                        colPtr[offset + 3] = p.Color.A;
                    }
                    else
                    {
                        // Particles that just died will be put at the end of the buffer in SortParticles();
                        p.Distance = -1.0f;
                    }

                    num++;
                }
            }
            Array.Sort(ParticlesContainer);
        }

        public RenderCommandMesh3D _rc = new RenderCommandMesh3D(ERenderPass.TransparentForward);
        public void AddRenderables(RenderPasses passes, Camera camera) => passes.Add(_rc);
        
        public void PreRenderUpdate(Camera camera) => CameraPosition = camera.LocalPoint;
        public void PreRenderSwap() { }
        public void PreRender(Viewport viewport, Camera camera) { }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Particle
    {
        private float _life;
        private float _scale;
        private float _distance;
        private Vec3 _position;
        private Vec3 _velocity;
        private ColorF4 _color;

        public float Life { get => _life; set => _life = value; }
        public float Scale { get => _scale; set => _scale = value; }
        public float Distance { get => _distance; set => _distance = value; }
        public Vec3 Position { get => _position; set => _position = value; }
        public Vec3 Velocity { get => _velocity; set => _velocity = value; }
        public ColorF4 Color { get => _color; set => _color = value; }
    };
}
