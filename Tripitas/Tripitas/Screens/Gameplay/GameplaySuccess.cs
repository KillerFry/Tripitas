using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.GamerServices;
using DuckieGameState;
using DuckieInput;
using HighScores_WP;

namespace Tripitas
{
    public class GameplaySuccess : GameScreen
    {
        #region Fields

        SpriteFont _font;
        SpriteFont _optionsFont;
        string _message;

        IAsyncResult keyboardResult;

        const int _hPad = 32;
        const int _vPad = 16;
        const int _menuEntryPadding = 10;

        List<MenuEntry> _menuEntries;
        int _selectedEntry = 0;

        Rectangle _backgroundRectangle;
        Texture2D _backgroundTexture;

        HighScores highScores;
        bool captureDone = true;

        #endregion

        #region Initialization
        public GameplaySuccess(string message)
        {
            Name = "GameplaySuccess";

            EnabledGestures = GestureType.Tap;

            _message = message;

            MenuEntry restartGame = new MenuEntry("Restart");
            MenuEntry next = new MenuEntry("Next!");
            MenuEntry enterHighScoreAndExit = new MenuEntry("Enter high score and exit");

            restartGame.Selected += restartSelected;
            next.Selected += nextSelected;
            enterHighScoreAndExit.Selected += EnterHighScoreAndExitSelected;

            _menuEntries = new List<MenuEntry>();
            _menuEntries.Add(restartGame);
            _menuEntries.Add(next);
            _menuEntries.Add(enterHighScoreAndExit);

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void Activate(bool preservedInstance)
        {
            if (!preservedInstance)
            {
                ContentManager content = ScreenManager.Game.Content;

                RenderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Vector4, DepthFormat.None);
                _font = content.Load<SpriteFont>("gamePauseTitleFont");
                _optionsFont = content.Load<SpriteFont>("gamePauseOptionsFont");
                _backgroundTexture = content.Load<Texture2D>("postIt");
                _backgroundRectangle = new Rectangle(135, 70, 554, 380);
                highScores = new HighScores();
                highScores.InitializeTable("Normal", 5);
                highScores.LoadScores();
                foreach (MenuEntry menuEntry in _menuEntries)
                {
                    menuEntry.Font = _optionsFont;
                    menuEntry.Padding = 5;
#if DEBUG
                    menuEntry.DrawHitBox = true;
#endif
                }
            }
        }
        #endregion

        #region Events
        void restartSelected(object sender, PlayerIndexEventArgs e)
        {
            ExitScreen();
            GameplayScreen screen = ScreenManager.GetScreenByName("GameplayScreen") as GameplayScreen;
            screen.ResetGameScreen(true);
            GC.Collect();
        }

        void nextSelected(object sender, PlayerIndexEventArgs e)
        {
            ExitScreen();
            GameplayScreen screen = ScreenManager.GetScreenByName("GameplayScreen") as GameplayScreen;
            screen.ResetGameScreen(false);
            GC.Collect();
        }

        void EnterHighScoreAndExitSelected(object sender, PlayerIndexEventArgs e)
        {
            if (highScores.GetTable("Normal").ScoreQualifies(((GameplayScreen)ScreenManager.GetScreenByName("GameplayScreen")).Score))
            {
                captureDone = false;
            }
        }

        #endregion

        #region Handle Input

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            foreach (GestureSample gesture in input.Gestures)
            {
                if (gesture.GestureType == GestureType.Tap)
                {
                    Point tapLocation = new Point((int)gesture.Position.X, (int)gesture.Position.Y);
                    foreach (MenuEntry menuEntry in _menuEntries)
                    {
                        if (menuEntry.GetMenuEntryHitBounds().Contains(tapLocation))
                        {
                            menuEntry.OnSelectEntry(PlayerIndex.One);
                        }
                    }
                }
            }
        }
        #endregion

        #region Update n' Draw
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            if (keyboardResult == null && !Guide.IsVisible && !captureDone)
            {
                keyboardResult = Guide.BeginShowKeyboardInput((PlayerIndex)ControllingPlayer.Value, "A new high score!", "Enter your name", "", InputCallback, null);
            }
            else if (keyboardResult != null && keyboardResult.IsCompleted)
            {
                Guide.EndShowKeyboardInput(keyboardResult);
                keyboardResult = null;
                ScreenManager.RemoveScreens();
                ScreenManager.AddScreen(new Background(), ControllingPlayer);
                ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            ScreenManager.GraphicsDevice.SetRenderTarget(RenderTarget);
            ScreenManager.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin();
            spriteBatch.Draw(_backgroundTexture, Vector2.Zero, Color.White * TransitionAlpha);
            spriteBatch.DrawString(_font, _message, new Vector2(
                _backgroundTexture.Width / 2 - _font.MeasureString(_message).X / 2,
                _backgroundTexture.Height / 7), Color.Black * TransitionAlpha);

            for (int i = 0; i < _menuEntries.Count; i++)
            {
                MenuEntry menuEntry = _menuEntries[i];

                if (menuEntry.EntryText == "Enter high score and exit")
                {
                    if (!highScores.GetTable("Normal").ScoreQualifies(((GameplayScreen)ScreenManager.GetScreenByName("GameplayScreen")).Score))
                    {
                        continue;
                    }
                    menuEntry.Position = new Vector2(
                        _backgroundRectangle.Left - menuEntry.GetWidth() / 2 + _backgroundRectangle.Width / 2,
                        _backgroundRectangle.Top + (menuEntry.GetHeight() * 2.75f));
                }
                else
                {
                    menuEntry.Position = new Vector2(
                        _backgroundRectangle.Left - (menuEntry.GetWidth() / 2) + (_backgroundRectangle.Width / 2 / 2) + ((_backgroundRectangle.Width / 2) * i),
                        _backgroundRectangle.Bottom - (menuEntry.GetHeight() * 1.25f));
                }
                menuEntry.Draw(ScreenManager.GraphicsDevice, spriteBatch, _optionsFont, Color.Black, false, gameTime);
            }
            spriteBatch.End();
        }
        #endregion

        #region Private Methods
        void InputCallback(IAsyncResult result)
        {
            captureDone = true;
            string sipContent = Guide.EndShowKeyboardInput(result);
            HighScoreEntry newEntry = null;

            if (sipContent != null)
            {
                newEntry = highScores.GetTable("Normal").AddEntry(sipContent, ((GameplayScreen)ScreenManager.GetScreenByName("GameplayScreen")).Score);
                highScores.SaveScores();
            }
        }
        #endregion
    }
}
