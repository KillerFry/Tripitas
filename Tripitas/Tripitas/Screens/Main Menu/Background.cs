using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using DuckieGameState;

namespace Tripitas {
    class Background : GameScreen {
        ContentManager _content;
        Texture2D _backgroundTexture;

        public Background() {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            Name = "MenuBackground";
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (_content == null)
                {
                    _content = new ContentManager(ScreenManager.Game.Services, "Content");
                }
                _backgroundTexture = _content.Load<Texture2D>("MainMenu/menuBackground");
                RenderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Vector4, DepthFormat.None);
            }
        }

        public override void Unload()
        {
            _content.Unload();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {
            base.Update(gameTime, otherScreenHasFocus, false);
        }

        public override void Draw(GameTime gameTime) {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            
            ScreenManager.GraphicsDevice.SetRenderTarget(RenderTarget);
            ScreenManager.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();
            spriteBatch.Draw(_backgroundTexture, fullscreen, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            spriteBatch.End();
        }
    }
}
