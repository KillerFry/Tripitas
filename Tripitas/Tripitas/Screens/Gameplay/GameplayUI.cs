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

namespace Tripitas {
    class GameplayUI : GameScreen {
        #region Fields

        ContentManager _content;

        Texture2D _untouched;
        Texture2D _touched;
        Vector2 _touchIndicatorPosition;

        Texture2D _magnifierView;
        Texture2D _zoomViewIcon;
        Vector2 _zoomViewIconPosition;

        Texture2D _pause;
        Vector2 _pausePosition;

        RenderTarget2D _zoomRenderTarget;
        Vector2 _touchPosition;

        bool _isPress;
        bool _isTouched;

        int _touchId;

        SpriteFont scoreFont;

        #endregion

        #region Initialization

        public GameplayUI() {
            Name = "GameplayUI";

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _zoomViewIconPosition = new Vector2(10.0f, 100.0f);
            
            GC.Collect();
        }

        public override void Activate(bool instancePreserved) {
            if (!instancePreserved) {
                if (_content == null) {
                    _content = new ContentManager(ScreenManager.Game.Services, "Content");
                }

                RenderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Vector4, DepthFormat.None);
                _pause = _content.Load<Texture2D>("Gameplay/pause");
                _untouched = _content.Load<Texture2D>("Gameplay/untouched");
                _touched = _content.Load<Texture2D>("Gameplay/touched");
                _zoomViewIcon = _content.Load<Texture2D>("Gameplay/zoom");
                _magnifierView = _content.Load<Texture2D>("magZoomEffect");
                scoreFont = _content.Load<SpriteFont>("Gameplay/scoreFont");
                _zoomRenderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, 150, 150, false, SurfaceFormat.Color, DepthFormat.None);

                _pausePosition = new Vector2(5.0f, 415.0f);
                _touchIndicatorPosition = new Vector2(740.0f, 4.0f);
            }
        }

        public override void Unload() {
            _content.Unload();
        }

        #endregion

        #region Update n' Draw

        public override void HandleInput(GameTime gameTime, InputState input) {
            if (input == null) {
                throw new ArgumentNullException("input");
            }

            TouchCollection touchState = input.TouchState;

            if (touchState.Count == 0) {
                return;
            }

            foreach (TouchLocation touch in touchState) {
                if ((touch.Position.X > 100.0f && touch.Position.Y < 100.0f) ||
                    touch.Position.X < 100.0f) {
                        _touchId = touch.Id;
                }
            }

            TouchLocation touchLocation;
            touchState.FindById(_touchId, out touchLocation);

            if (touchLocation.State == TouchLocationState.Released) {
                _isPress = false;
                _touchId = 0;
                return;
            }

            if (touchLocation.State == TouchLocationState.Pressed) {
                _touchPosition = touchLocation.Position;
                if (touchLocation.Position.X > _zoomViewIconPosition.X &&
                    touchLocation.Position.X < _zoomViewIconPosition.X + 40 &&
                    touchLocation.Position.Y > _zoomViewIconPosition.Y &&
                    touchLocation.Position.Y < _zoomViewIconPosition.Y + 40) {
                        _isPress = true;
                }
            }

            if (touchLocation.State == TouchLocationState.Pressed) {
                _touchPosition = touchLocation.Position;
                if (touchLocation.Position.X > _pausePosition.X &&
                    touchLocation.Position.X < _pausePosition.X + _pause.Width &&
                    touchLocation.Position.Y > _pausePosition.Y &&
                    touchLocation.Position.Y < _pausePosition.Y + _pause.Height) {
                    ScreenManager.AddScreen(new GameplayPause("Pause!"), ControllingPlayer);
                }
            }
        }

        public override void Draw(GameTime gameTime) {

            Vector2 position = Vector2.Zero;
            int offsetX = 0;
            int offsetY = 0;
            int offsetXPosition = 19;
            int offsetYPosition = 19;
            _isTouched = false;

            GameplayScreen screen = (GameplayScreen)ScreenManager.GetScreenByName("GameplayScreen");

            _isTouched = screen.IsTouched;

            if (_isPress && _isTouched) {
                ScreenManager.GraphicsDevice.SetRenderTarget(_zoomRenderTarget);
                ScreenManager.GraphicsDevice.Clear(Color.Transparent);

                ScreenManager.SpriteBatch.Begin();
                position = screen.TouchLocation.Position;

                if (position.X < 150) {
                    offsetX = 150;
                    offsetXPosition = 55;
                }
                if (position.Y < 150) {
                    offsetY = 150;
                    offsetYPosition = 55;
                }

                ScreenManager.SpriteBatch.Draw(ScreenManager.GetScreenByName("GameplayBackground").RenderTarget, new Vector2(offsetXPosition, offsetYPosition),
                    new Rectangle((int)position.X - 30, (int)position.Y - 30, 80, 80),
                    Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
                ScreenManager.SpriteBatch.Draw(ScreenManager.GetScreenByName("GameplayScreen").RenderTarget, new Vector2(offsetXPosition, offsetYPosition),
                    new Rectangle((int)position.X - 30, (int)position.Y - 30, 80, 80),
                    Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
                SpriteEffects spriteEffect = SpriteEffects.None;
                if (offsetX > 0) {
                    spriteEffect |=SpriteEffects.FlipHorizontally;
                }
                if (offsetY > 0) {
                    spriteEffect |= SpriteEffects.FlipVertically;
                }
                ScreenManager.SpriteBatch.Draw(_magnifierView, Vector2.Zero,
                    null,
                    Color.White, 0.0f, Vector2.Zero, 1.0f, spriteEffect, 0);
                ScreenManager.SpriteBatch.End();
            }

            ScreenManager.GraphicsDevice.SetRenderTarget(RenderTarget);
            ScreenManager.GraphicsDevice.Clear(Color.Transparent);

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.DrawString(scoreFont, screen.Score.ToString(), new Vector2(25, 5), Color.Black);
            ScreenManager.SpriteBatch.Draw(_pause, _pausePosition, null, Color.White, 0.0f, Vector2.Zero, 0.75f, SpriteEffects.None, 1 );
            ScreenManager.SpriteBatch.Draw((_isTouched ? _touched : _untouched), _touchIndicatorPosition, Color.White);
            ScreenManager.SpriteBatch.Draw(_zoomViewIcon, _zoomViewIconPosition, null, Color.White, 0.0f, Vector2.Zero, 0.60f, SpriteEffects.None, 1);
            if (_isPress) {
                ScreenManager.SpriteBatch.Draw(_zoomRenderTarget, new Vector2(position.X - 150 + offsetX, position.Y - 150 + offsetY), Color.White);
            }
            ScreenManager.SpriteBatch.End();
        }

        #endregion
    }
}
