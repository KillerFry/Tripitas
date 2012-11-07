using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using DuckieGameState;
using DuckieInput;

namespace Tripitas {
    abstract class MenuScreen : GameScreen {
        #region Fields

        List<MenuEntry> _menuEntries = new List<MenuEntry>();
        int _selectedEntry = 0;
        string _menuTitle;

        SpriteFont titleFont;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of menu entries, so drived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries {
            get { return _menuEntries; }
        }

        #endregion

        #region Initialization

        public MenuScreen(string menuTitle) {
            EnabledGestures = GestureType.Tap;
            
            _menuTitle = menuTitle;
            Name = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void Activate(bool instancePreserved) {
            RenderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Vector4, DepthFormat.None); 
            //base.Activate(instancePreserved);
            titleFont = ScreenManager.Game.Content.Load<SpriteFont>("MainMenu/titleFont");
            foreach (MenuEntry menuEntry in _menuEntries)
            {
                menuEntry.Font = ScreenManager.Game.Content.Load<SpriteFont>("MainMenu/menuEntryFont");
                menuEntry.Padding = 00;
            }
        }

        #endregion

        #region Handle Input

        public override void HandleInput(GameTime gameTime, InputState input) {
            PlayerIndex playerIndex;

            if (input.IsNewKeyPress(Keys.Up, ControllingPlayer, out playerIndex) ||
                input.IsNewButtonPress(Buttons.DPadUp, ControllingPlayer, out playerIndex) ||
                input.IsNewButtonPress(Buttons.LeftThumbstickUp, ControllingPlayer, out playerIndex)) {
                _selectedEntry--;

                if (_selectedEntry < 0) {
                    _selectedEntry = _menuEntries.Count - 1;
                }
            }

            if (input.IsNewKeyPress(Keys.Down, ControllingPlayer, out playerIndex) ||
                input.IsNewButtonPress(Buttons.DPadDown, ControllingPlayer, out playerIndex) ||
                input.IsNewButtonPress(Buttons.LeftThumbstickDown, ControllingPlayer, out playerIndex))
            {
                _selectedEntry++;

                if (_selectedEntry >= _menuEntries.Count) {
                    _selectedEntry = 0;
                }
            }

            if (input.IsNewKeyPress(Keys.Space, ControllingPlayer, out playerIndex) ||
                input.IsNewKeyPress(Keys.Enter, ControllingPlayer, out playerIndex) ||
                input.IsNewButtonPress(Buttons.A, ControllingPlayer, out playerIndex) ||
                input.IsNewButtonPress(Buttons.Start, ControllingPlayer, out playerIndex)) {
                OnSelectEntry(_selectedEntry, playerIndex);
                }
            else if (input.IsNewKeyPress(Keys.Escape, ControllingPlayer, out playerIndex) ||
                 input.IsNewButtonPress(Buttons.B, ControllingPlayer, out playerIndex) ||
                 input.IsNewButtonPress(Buttons.Back, ControllingPlayer, out playerIndex))
            {
                OnCancel(playerIndex);
            }

            if (input.IsNewButtonPress(Buttons.Back, ControllingPlayer, out playerIndex)) {
                ScreenManager.Game.Exit();
            }

            foreach (GestureSample gesture in input.Gestures) {
                if (gesture.GestureType == GestureType.Tap) {
                    Point tapLocation = new Point((int)gesture.Position.X, (int)gesture.Position.Y);
                    for (int i = 0; i < _menuEntries.Count; i++) {
                        MenuEntry menuEntry = _menuEntries[i];
                        
                        if (menuEntry.GetMenuEntryHitBounds().Contains(tapLocation)) {
                            OnSelectEntry(i, PlayerIndex.One);
                        }
                    }
                }
            }

            base.HandleInput(gameTime, input);
        }

        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex) {
            _menuEntries[entryIndex].OnSelectEntry(playerIndex);
        }

        /// <summary>
        /// Handler for then the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel(PlayerIndex playerIndex) {
            ExitScreen();
        }

        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event hendler.
        /// </summary>
        protected void OnCancel(object sender, PlayerIndexEventArgs e) {
            OnCancel(e.PlayerIndex);
        }

        #endregion

        #region Update n' Draw

        protected virtual void UpdateMenuEntryLocation() {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Start at Y = 175; each X value is generated per entry/
            Vector2 position = new Vector2(0.0f, 250.0f);

            for (int i = 0; i < _menuEntries.Count; i++) {
                MenuEntry menuEntry = _menuEntries[i];

                // Each entry is to be centered horizontally
                position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2 - menuEntry.GetWidth() / 2;

                if (ScreenState == ScreenState.TransitionOn) {
                    position.X -= transitionOffset * 256;
                } else {
                    position.X += transitionOffset * 512;
                }

                // Set the entry's position
                menuEntry.Position = position;

                // Move down for the next entry the size of this entry.
                position.Y += menuEntry.GetHeight() + 25;
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            for (int i = 0; i < _menuEntries.Count; i++) {
                bool isSelected = IsActive && (i == _selectedEntry);

                _menuEntries[i].Update(this, isSelected, gameTime);
            }
        }

        public override void Draw(GameTime gameTime) {
            UpdateMenuEntryLocation();

            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            
            graphics.SetRenderTarget(RenderTarget);
            ScreenManager.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin();
            
            // Draw each menu entry in turn.
            for (int i = 0; i < _menuEntries.Count; i++) {
                MenuEntry menuEntry = MenuEntries[i];

                bool isSelected = IsActive && (i == _selectedEntry);

                menuEntry.Draw(ScreenManager.GraphicsDevice, spriteBatch, menuEntry.Font, Color.Black, isSelected, gameTime);
            }

            // Make the menu slide into place furing transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end.
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen.
            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 120);
            Vector2 titleOrigin = titleFont.MeasureString(_menuTitle) / 2;
            Color titleColor = new Color(0, 0, 0) * TransitionAlpha;
            float titleScale = 1.0f;

            //titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(titleFont, _menuTitle, titlePosition, titleColor, 0, titleOrigin, titleScale, SpriteEffects.None, 0);
            
            spriteBatch.End();
        }

        #endregion
    }
}
