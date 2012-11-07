using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.GamerServices;
using DuckieGameState;
using HighScores_WP;

namespace Tripitas {
    public class HighScoreCapture : GameScreen {

        #region Fields

        ContentManager _content;
        SpriteFont _gameFont;

        HighScores _highScores;
        bool _captureDone = false;

        int _score;

        #endregion

        #region Initialization

        public HighScoreCapture() {
            Name = "HighScoreEntry";

            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.0);

            GC.Collect();
        }

        public override void Activate(bool instancePreserved) {
            if (!instancePreserved) {
                if (_content == null) {
                    _content = new ContentManager(ScreenManager.Game.Services, "Content");
                }

                RenderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Vector4, DepthFormat.None);
                _gameFont = _content.Load<SpriteFont>("leFont");
                _highScores = new HighScores();
                _highScores.InitializeTable("Normal", 20);
                _highScores.LoadScores();
            }
        }

        #endregion

        #region Update n' Draw

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {
            if (!Guide.IsVisible && !_captureDone) {
                Guide.BeginShowKeyboardInput((PlayerIndex)ControllingPlayer.Value, "A new high score!", "Enter your name", "", InputCallback, null);
            }
        }

        public override void Draw(GameTime gameTime) {
            ScreenManager.GraphicsDevice.SetRenderTarget(RenderTarget);
            ScreenManager.GraphicsDevice.Clear(Color.Transparent);


        }

        #endregion

        void InputCallback(IAsyncResult result) {
            _captureDone = true;
            string sipContent = Guide.EndShowKeyboardInput(result);
            HighScoreEntry newEntry = null;

            if (sipContent != null) {
                newEntry = _highScores.GetTable("Normal").AddEntry(sipContent, _score);
                _highScores.SaveScores();
            }

            ExitScreen();

            //LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new Background(), new MainMenuScreen());
        }
    }
}
