using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DuckieGameState;
using DuckieInput;

namespace Tripitas {
    class MessageBoxScreen : GameScreen {
        #region Fields

        string _message;
        Texture2D _gradientTexture;

        #endregion

        #region Events

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        #endregion

        #region Initialization

        public MessageBoxScreen(string message)
            : this(message, true) { }

        public MessageBoxScreen(string message, bool includeUsageText) {
            const string usageText = "\nA button, Space, Enter = ok" +
                                    "\nB button, Esc = cancel";

            Name = "MessageBox";

            if (includeUsageText) {
                _message = message + usageText;
            } else {
                _message = message;
            }

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }

        public override void Activate(bool instancePreserved) {
            if (!instancePreserved)
            {
                ContentManager content = ScreenManager.Game.Content;

                _gradientTexture = content.Load<Texture2D>("gradient");
            }
        }

        #endregion

        #region Handle Input

        public override void HandleInput(GameTime gameTime, InputState input) {
            PlayerIndex playerIndex;

            if (input.IsNewKeyPress(Keys.Space, ControllingPlayer, out playerIndex) ||
                input.IsNewKeyPress(Keys.Enter, ControllingPlayer, out playerIndex) ||
                input.IsNewButtonPress(Buttons.A, ControllingPlayer, out playerIndex) ||
                input.IsNewButtonPress(Buttons.Start, ControllingPlayer, out playerIndex)) {
                if (Accepted != null) {
                    Accepted(this, new PlayerIndexEventArgs(playerIndex));
                }

                ExitScreen();
            }
            else if (input.IsNewKeyPress(Keys.Escape, ControllingPlayer, out playerIndex) ||
                 input.IsNewButtonPress(Buttons.B, ControllingPlayer, out playerIndex) ||
                 input.IsNewButtonPress(Buttons.Back, ControllingPlayer, out playerIndex)) {
                if (Cancelled != null) {
                    Cancelled(this, new PlayerIndexEventArgs(playerIndex));
                }

                ExitScreen();
            }
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime) {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(_message);
            Vector2 textPosition = (viewportSize - textSize) / 2;

            const int hPad = 32;
            const int vPad = 16;

            Rectangle backgroundRectangle = new Rectangle(
                (int)textPosition.X - hPad,
                (int)textPosition.Y - vPad,
                (int)textSize.X + hPad * 2,
                (int)textSize.Y + vPad * 2);

            Color color = Color.White * TransitionAlpha;

            spriteBatch.Begin();
            spriteBatch.Draw(_gradientTexture, backgroundRectangle, color);
            spriteBatch.DrawString(font, _message, textPosition, color);
            spriteBatch.End();                              
        }

        #endregion
    }
}
