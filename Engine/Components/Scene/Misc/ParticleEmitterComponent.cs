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
        private int _maxParticles = 100000;
        public Particle[] _particles;
        private bool _isEmitting = true;
        private int _lastUsedParticle = 0;
        private float _elapsed = 0.0f;

        public int ActiveInstances => ParticleMesh.Instances;
        public PrimitiveManager ParticleMesh
        {
            get => _rc.Mesh;
            set => _rc.Mesh = value;
        }
        public Vec3 CameraPosition { get; set; }
        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D();

        public int NumPerSpawn { get; set; } = 2;
        public float SecPerSpawn { get; set; } = 0.2f;
        public float NewParticleLifeSeconds { get; set; } = 2.0f;

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
        public int MaxParticles
        {
            get => _maxParticles;
            set
            {
                _maxParticles = value;
                if (IsSpawned)
                {
                    _particles.Resize(_maxParticles);
                    var instBufs = ParticleMesh.Data.GetAllBuffersOfType(EBufferType.Other);
                    //instBufs[0].Resize(_maxParticles);
                    //instBufs[1].Resize(_maxParticles);
                }
            }
        }

        public override void OnSpawned()
        {
            base.OnSpawned();

            _particles = new Particle[MaxParticles];

            PrimitiveData data = PrimitiveData.FromQuads(VertexShaderDesc.JustPositions(), VertexQuad.PosZQuad(1, false, 0.0f, true));

            Vec4[] positions = new Vec4[MaxParticles];
            ColorF4[] colors = new ColorF4[MaxParticles];
            var posBuf = data.AddBuffer(positions, new VertexAttribInfo(EBufferType.Other, 0), false, false, true, 1);
            var colBuf = data.AddBuffer(colors, new VertexAttribInfo(EBufferType.Other, 1), false, false, true, 1);
            posBuf.Location = 1;
            colBuf.Location = 2;
            
            GLSLScript vert = Engine.Files.LoadEngineShader("ParticleInstance.vs", EGLSLType.Vertex);
            GLSLScript frag = Engine.Files.LoadEngineShader("ParticleInstance.fs", EGLSLType.Fragment);
            RenderingParameters rp = new RenderingParameters(true);
            TMaterial mat = new TMaterial("ParticleMaterial", rp, vert, frag);
            ParticleMesh = new PrimitiveManager(data, mat);
            
            if (_isEmitting)
                StartEmitting();
            else
                StopEmitting();
        }
        public override void OnDespawned()
        {
            base.OnDespawned();

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
        protected virtual int FindUnusedParticleIndex()
        {
            for (int i = _lastUsedParticle; i < MaxParticles; i++)
                if (_particles[i].Life <= 0.0f)
                    return _lastUsedParticle = i;
            
            for (int i = 0; i < _lastUsedParticle; i++)
                if (_particles[i].Life <= 0.0f)
                    return _lastUsedParticle = i;
            
            return _lastUsedParticle = 0; // All particles are taken, override the first one
        }
        private readonly Random Random = new Random();
        protected virtual void InitializeParticle(ref Particle particle)
        {
            particle.Position = WorldPoint;
            Vec3 rand = new Vec3((float)Random.NextDouble(), (float)Random.NextDouble(), (float)Random.NextDouble());
            particle.Color = new ColorF4(rand.X, rand.Y, rand.Z, 1.0f);
            particle.Life = NewParticleLifeSeconds;
            particle.Velocity = WorldMatrix.UpVec * 10.0f + rand * 10.0f;
            particle.Scale = (float)Random.NextDouble();
        }

        private unsafe void Update(float delta)
        {
            _elapsed += delta;

            while (SecPerSpawn > 0.001f && _elapsed >= SecPerSpawn)
            {
                _elapsed -= SecPerSpawn;
                for (int i = 0; i < NumPerSpawn; ++i)
                {
                    int unusedParticle = FindUnusedParticleIndex();
                    InitializeParticle(ref _particles[unusedParticle]);
                }
            }
            
            int instanceCount = 0;
            for (int i = 0; i < MaxParticles; i++)
            {
                Particle p = _particles[i];
                if (p.Life > 0.0f)
                {
                    p.Life -= delta;

                    p.Velocity += new Vec3(0.0f, -9.81f, 0.0f) * delta * 0.5f;
                    p.Position += p.Velocity * delta;
                    p.Distance = (p.Position - CameraPosition).LengthSquared;
                    
                    var instBufs = ParticleMesh.Data.GetAllBuffersOfType(EBufferType.Other);
                    
                    Vec4* posPtr = (Vec4*)instBufs[0].Address;
                    posPtr[instanceCount] = new Vec4(p.Position, p.Scale);

                    ColorF4* colPtr = (ColorF4*)instBufs[1].Address;
                    colPtr[instanceCount] = p.Color;

                    ++instanceCount;
                }
                else
                {
                    // Particles that just died will be put at the end of the buffer in SortParticles();
                    p.Distance = -1.0f;
                }
                _particles[i] = p;
            }

            ParticleMesh.Instances = instanceCount;
            Array.Sort(_particles);
        }

        public RenderCommandMesh3D _rc = new RenderCommandMesh3D(ERenderPass.TransparentForward);
        public void AddRenderables(RenderPasses passes, Camera camera) => passes.Add(_rc);

        [Browsable(false)]
        public bool PreRenderEnabled => IsEmitting;
        public void PreRenderUpdate(Camera camera) => CameraPosition = camera.LocalPoint;
        public void PreRenderSwap() { }
        public void PreRender(Viewport viewport, Camera camera) { }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Particle : IComparable<Particle>
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

        public int CompareTo(Particle other) => (int)(other.Distance - Distance);
    };
}
