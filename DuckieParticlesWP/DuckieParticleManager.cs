using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DuckieParticles {
    public class DuckieParticleManager : DrawableGameComponent {
        #region Fields

        internal ContentManager Content {
            get;
            set;
        }

        public SpriteBatch SpriteBatch {
            get;
            protected set;
        }

        List<DuckieParticle> _particles;
        List<DuckieParticleCluster> _particleClusters;
        Dictionary<string, Texture2D> _textures;

        public Dictionary<string, Texture2D> Textures {
            get { return _textures; }
        }

        #endregion

        #region Initialization

        public DuckieParticleManager(Game game)
            : base(game) {
                _particles = new List<DuckieParticle>();
                _particleClusters = new List<DuckieParticleCluster>();
                _textures = new Dictionary<string, Texture2D>();
        }

        protected override void LoadContent() {
            Content = Game.Content;

            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent() {
            foreach (DuckieParticle particle in _particles) {
                particle.UnloadCOntent();
            }
        }

        #endregion

        #region Update n' Draw

        public override void Update(GameTime gameTime) {
            foreach (DuckieParticle particle in _particles) {
                particle.Update(gameTime);
            }

            foreach (DuckieParticleCluster cluster in _particleClusters) {
                cluster.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime) {
            foreach (DuckieParticle particle in _particles) {
                particle.Draw(gameTime);
            }

            foreach (DuckieParticleCluster cluster in _particleClusters) {
                cluster.Draw(gameTime);
            }
        }

        #endregion

        #region Public Methods

        public void AddTexture(string textureName) {
            if (!_textures.ContainsKey(textureName)) {
                _textures.Add(textureName, Content.Load<Texture2D>(textureName));
            }
        }

        public void AddParticleCluster(DuckieParticleCluster particleCluster) {
            _particleClusters.Add(particleCluster);
            particleCluster.ParticleManager = this;
        }

        #endregion
    }
}
