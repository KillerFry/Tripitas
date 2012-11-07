using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using DuckieGameState;
using DuckieInput;

namespace Tripitas {
    class GameplayPause : GameScreen {
        #region Fields

        SpriteFont _font;
        SpriteFont _optionsFont;
        string _message;

        const int _hPad = 32;
        const int _vPad = 16;
        const int _menuEntryPadding = 10;

        List<MenuEntry> _menuEntries;
        int _selectedEntry = 0;

        Rectangle _backgroundRectangle;
        Texture2D _backgroundTexture;

        #endregion

        #region Initialization
        public GameplayPause(string message) {
            Name = "GameplayPause";

            EnabledGestures = GestureType.Tap;

            _message = message;

            MenuEntry returnToMainMenu = new MenuEntry("Menu");
            MenuEntry restartGame = new MenuEntry("Restart");
            MenuEntry resumeGame = new MenuEntry("Resume");

            returnToMainMenu.Selected += ReturnToMainMenuEntrySelected;
            restartGame.Selected += RestartGameSelected;
            resumeGame.Selected += ResumeGameSelected;

            _menuEntries = new List<MenuEntry>();
            _menuEntries.Add(returnToMainMenu);
            _menuEntries.Add(restartGame);
            _menuEntries.Add(resumeGame);

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void Activate(bool preservedInstance) {
            if (!preservedInstance)
            {
                ContentManager content = ScreenManager.Game.Content;

                RenderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Vector4, DepthFormat.None);
                _font = content.Load<SpriteFont>("gamePauseTitleFont");
                _optionsFont = content.Load<SpriteFont>("gamePauseOptionsFont");
                _backgroundTexture = content.Load<Texture2D>("postIt");
                _backgroundRectangle = new Rectangle(135, 70, 554, 380);
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
        void ReturnToMainMenuEntrySelected(object sender, PlayerIndexEventArgs e) {
            ScreenManager.RemoveScreens();
            ScreenManager.AddScreen(new Background(), ControllingPlayer);
            ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);
        }

        void RestartGameSelected(object sender, PlayerIndexEventArgs e) {
            ExitScreen();
            foreach (GameScreen screen in ScreenManager.GetScreens()) {
                if (screen is GameplayScreen) {
                    ((GameplayScreen)screen).ResetGameScreen(true);
                }
            }
            GC.Collect();
        }

        void ResumeGameSelected(object sender, PlayerIndexEventArgs e) {
            ExitScreen();
        }

        #endregion

        #region Handle Input

        public override void HandleInput(GameTime gameTime, InputState input) {
            foreach (GestureSample gesture in input.Gestures) {
                if (gesture.GestureType == GestureType.Tap) {
                    Point tapLocation = new Point((int)gesture.Position.X, (int)gesture.Position.Y);
                    foreach (MenuEntry menuEntry in _menuEntries) {
                        if (menuEntry.GetMenuEntryHitBounds().Contains(tapLocation)) {
                            menuEntry.OnSelectEntry(PlayerIndex.One);
                        }
                    }
                }
            }
        }
        #endregion

        #region Update n' Draw

        public override void Draw(GameTime gameTime) {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            ScreenManager.GraphicsDevice.SetRenderTarget(RenderTarget);
            ScreenManager.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin();
            spriteBatch.Draw(_backgroundTexture, Vector2.Zero, null, Color.White * TransitionAlpha, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(_font, _message, new Vector2(
                _backgroundTexture.Width / 2 - _font.MeasureString(_message).X / 2,
                _backgroundTexture.Height / 7), Color.Black * TransitionAlpha);
            
            for (int i = 0; i < _menuEntries.Count; i++) {
                MenuEntry menuEntry = _menuEntries[i];

                menuEntry.Position = new Vector2(
                    _backgroundRectangle.Left - (menuEntry.GetWidth() / 2) + (_backgroundRectangle.Width / _menuEntries.Count / 2) + ((_backgroundRectangle.Width / _menuEntries.Count) * i),
                    _backgroundRectangle.Bottom - (menuEntry.GetHeight() * 2));

                menuEntry.Draw(ScreenManager.GraphicsDevice, spriteBatch, _optionsFont, Color.Black, false, gameTime);
            }
            
            spriteBatch.End();
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
