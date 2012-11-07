using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using DuckieGameState;
using DuckieInput;
using HighScores_WP;

namespace Tripitas {
    class HighScoreDisplay : GameScreen {
        #region Fields

        ContentManager _content;
        SpriteFont _gameFont;

        HighScores _highScores;

        #endregion

        #region Initialization

        public HighScoreDisplay() {
            Name = "HighScoreDisplay";

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            GC.Collect();
        }

        public override void Activate(bool instancePreserved) {
            if (!instancePreserved) {
                if (_content == null) {
                    _content = new ContentManager(ScreenManager.Game.Services, "Content");
                }

                RenderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Vector4, DepthFormat.None);
                _gameFont = ScreenManager.Font;
                _highScores = new HighScores();
                _highScores.InitializeTable("Normal", 5);
                _highScores.LoadScores();
            }
        }

        #endregion

        #region Update n' Draw

        public override void HandleInput(GameTime gameTime, InputState input) {
            if (input == null) {
                throw new ArgumentNullException("input");
            }

            PlayerIndex player;
            if (input.IsNewButtonPress(Buttons.Back, ControllingPlayer, out player)) {
                LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new Background(), new MainMenuScreen());
            }
        }

        public override void Draw(GameTime gameTime) {
            HighScoreTable highScoreTable = _highScores.GetTable("Normal");
            int entryCount = highScoreTable.Entries.Count;
            int yPosition;

            ScreenManager.GraphicsDevice.SetRenderTarget(RenderTarget);
            ScreenManager.GraphicsDevice.Clear(Color.Transparent);

            ScreenManager.SpriteBatch.Begin();
            for (int i = 0; i < entryCount; i++) {
                yPosition = 20 + (20 * i);

                ScreenManager.SpriteBatch.DrawString(_gameFont, highScoreTable.Entries[i].Name + " " + highScoreTable.Entries[i].Score, new Vector2(10, yPosition), Color.White);
            }
            ScreenManager.SpriteBatch.End();
        }

        #endregion

    }
}
