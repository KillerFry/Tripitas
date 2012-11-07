using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DuckieParticles {
    public enum DuckieParticleState {
        Alive,
        Dead
    }

    public class DuckieParticle {
        #region Properties

        public string ParticleType {
            get;
            set;
        }

        public Texture2D Texture {
            get;
            set;
        }

        public Vector2 Position {
            get;
            set;
        }

        public TimeSpan LifeTime {
            get;
            set;
        }

        public DuckieParticleState State {
            get;
            internal set;
        }

        public DuckieParticleCluster ParticleCluster {
            get;
            set;
        }

        public DuckieParticleManager ParticleManager {
            get;
            set;
        }

        #endregion

        #region Initialization

        public virtual void LoadContent() { }

        public virtual void UnloadCOntent() { }

        #endregion

        #region Update n' Draw

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(GameTime gameTime) { }
        
        #endregion

    }
}
