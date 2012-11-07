using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DuckieParticles {
    public class DuckieParticleCluster {
        #region Fields
        
        internal DuckieParticleManager ParticleManager {
            get;
            set;
        }

        List<DuckieParticle> _particles;
        Dictionary<string, Texture2D> _textures;

        #endregion

        #region Initialization

        public DuckieParticleCluster() {
            _particles = new List<DuckieParticle>();
            _textures = new Dictionary<string, Texture2D>();
        }

        public virtual void LoadContent() { }

        public virtual void UnloadContent() { }

        #endregion

        #region Update n' Draw

        public delegate void DuckieParticleClusterUpdate(DuckieParticle particle);
        public event DuckieParticleClusterUpdate OnClusterUpdate;

        public virtual void Update(GameTime gameTime) {
            foreach (DuckieParticle particle in _particles) {
                particle.Update(gameTime);
                OnClusterUpdate(particle);
            }
        }

        public virtual void Draw(GameTime gameTime) {
            foreach (DuckieParticle particle in _particles) {
                particle.Draw(gameTime);
            }
        }

        #endregion

        #region Public Methods

        public virtual void AddParticles(uint numberOfParticles) {
            if (_particles == null) {
                _particles = new List<DuckieParticle>();
            }
            for (uint i = 0; i < numberOfParticles; i++) {
                _particles.Add(new DuckieParticle() { ParticleCluster = this } );
            }
        }

        #endregion
    }
}
