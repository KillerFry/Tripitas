using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using DuckieGameState;

namespace Tripitas {
    class GameplayBackground : GameScreen {
        ContentManager _content;
        Texture2D _backgroundTexture;
        Rectangle _fullscreen;

        public GameplayBackground() {
            Name = "GameplayBackground";

            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void Activate(bool instancePreserved) {
            if (!instancePreserved) {
                if (_content == null) {
                    _content = new ContentManager(ScreenManager.Game.Services, "Content");
                }

                RenderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Vector4, DepthFormat.None);
                _backgroundTexture = _content.Load<Texture2D>("Gameplay/paperBackground");
                _fullscreen = new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);
            }
        }

        public override void Unload() {
            _content.Unload();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {
            base.Update(gameTime, otherScreenHasFocus, false);
        }

        public override void Draw(GameTime gameTime) {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            ScreenManager.GraphicsDevice.SetRenderTarget(RenderTarget);
            ScreenManager.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();
            spriteBatch.Draw(_backgroundTexture, _fullscreen, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            spriteBatch.End();
        }
    }
}
