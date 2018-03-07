using System;
using System.Collections.Generic;
using TheraEngine.Core.Memory;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Rendering.Particles
{
    public unsafe class ParticleManager
    {
        public ParticleManager() : this(1000000) { }
        public ParticleManager(int maxParticles) { MaxParticles = maxParticles; }

        private int _lastUsedParticle = -1;
        private DataSource _particles;
        private Particle* _data;
        private List<PrimitiveManager> _meshes = new List<PrimitiveManager>();
        
        public int MaxParticles
        {
            get => _particles.Length;
            set
            {
                DataSource newSource = DataSource.Allocate(value * sizeof(Particle));
                if (_particles != null)
                {
                    Memory.Move(newSource.Address, _particles.Address, (uint)_particles.Length);
                    _particles?.Dispose();
                }
                _particles = newSource;
                if (_particles != null)
                    _data = (Particle*)_particles.Address;
                else
                    _data = null;
            }
        }
        
        public int RegisterMesh(PrimitiveManager mesh)
        {
            for (int i = 0; i < _meshes.Count; ++i)
                if (_meshes[i] == null)
                {
                    _meshes[i] = mesh;
                    return i;
                }

            _meshes.Add(mesh);
            return _meshes.Count - 1;
        }

        public int SpawnParticle(Particle particle)
        {
            if (particle.Life <= 0.0f)
                return -1;

            int index = FindUnusedParticle();
            _data[index] = particle;
            return index;
        }
        
        public int FindUnusedParticle()
        {
            int leastLifeIndex = -1;
            float leastLife = float.MaxValue;

            for (int i = _lastUsedParticle + 1; i < MaxParticles; ++i)
            {
                float life = _data[i].Life;
                if (life < leastLife)
                {
                    leastLife = life;
                    leastLifeIndex = i;
                }
                if (life < 0)
                {
                    _lastUsedParticle = i;
                    return i;
                }
            }

            for (int i = 0; i <= _lastUsedParticle; ++i)
            {
                float life = _data[i].Life;
                if (life < leastLife)
                {
                    leastLife = life;
                    leastLifeIndex = i;
                }
                if (life < 0)
                {
                    _lastUsedParticle = i;
                    return i;
                }
            }

            //All particles are taken, override the one with the least life left
            return leastLifeIndex;
        }
    }
}
