using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DuckieGameState;

namespace Tripitas
{
    class LoadingScreen : GameScreen
    {
        #region Fields

        bool _loadingIsSlow;
        bool _otherScreensAreGone;

        GameScreen[] _screensToLoad;

        #endregion

        #region Initialization

        private LoadingScreen(ScreenManager screenManager, bool loadingIsSlow, GameScreen[] screensToLoad)
        {
            _loadingIsSlow = loadingIsSlow;
            _screensToLoad = screensToLoad;

            Name = "LoadingScreen";

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
        }

        public static void Load(ScreenManager screenManager, bool loadingIsSlow, PlayerIndex? controllingPlayer, params GameScreen[] screensToLoad)
        {
            foreach (GameScreen screen in screenManager.GetScreens())
            {
                screen.ExitScreen();
            }

            LoadingScreen loadingScreen = new LoadingScreen(screenManager, loadingIsSlow, screensToLoad);

            screenManager.AddScreen(loadingScreen, controllingPlayer);
        }

        public override void Activate(bool instancePreserved)
        {
            RenderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Vector4, DepthFormat.None);
        }

        #endregion

        #region Update n' Draw

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (_otherScreensAreGone)
            {
                ScreenManager.RemoveScreen(this);

                foreach (GameScreen screen in _screensToLoad)
                {
                    if (screen != null)
                    {
                        ScreenManager.AddScreen(screen, ControllingPlayer);
                    }
                }

                ScreenManager.Game.ResetElapsedTime();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if ((ScreenState == ScreenState.Active) &&
                (ScreenManager.GetScreens().Length == 1))
            {
                _otherScreensAreGone = true;
            }

            if (_loadingIsSlow)
            {
                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
                SpriteFont font = ScreenManager.Font;

                const string message = "Loading...";

                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
                Vector2 textSize = font.MeasureString(message);
                Vector2 textPosition = (viewportSize - textSize) / 2;

                Color color = Color.White * TransitionAlpha;

                ScreenManager.GraphicsDevice.SetRenderTarget(RenderTarget);
                ScreenManager.GraphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin();
                spriteBatch.DrawString(font, message, textPosition, color);
                spriteBatch.End();
            }
        }

        #endregion
    }
}
