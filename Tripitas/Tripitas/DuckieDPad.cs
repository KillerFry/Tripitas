using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Tripitas
{
    class DuckieDPad : DrawableGameComponent
    {
        SpriteBatch _spriteBatch;

        Texture2D _upArrow;
        Texture2D _rightArrow;
        Texture2D _downArrow;
        Texture2D _leftArrow;

        Vector2 _centerPosition;
        Vector2 _upArrowPosition;
        Vector2 _rightArrowPosition;
        Vector2 _downArrowPosition;
        Vector2 _leftArrowPosition;

        public Vector2 Position
        {
            get { return _centerPosition; }
            set
            {
                _centerPosition = value;
                _upArrowPosition = new Vector2(
                    _centerPosition.X - (_upArrow.Bounds.Width / 2),
                    _centerPosition.Y - (_upArrow.Bounds.Height * 1.5f));
                _rightArrowPosition = new Vector2(
                    _centerPosition.X + (_rightArrow.Bounds.Width / 2),
                    _centerPosition.Y + (_rightArrow.Bounds.Height / 2));
                _downArrowPosition = new Vector2(
                    _centerPosition.X - (_downArrow.Bounds.Width / 2),
                    _centerPosition.Y + (_downArrow.Bounds.Height / 2));
                _leftArrowPosition = new Vector2(
                    _centerPosition.X - (_leftArrow.Bounds.Width * 1.5f),
                    _centerPosition.Y + (_leftArrow.Bounds.Height / 2));
            }
        }


        public DuckieDPad(Game game) :
            base(game) { }

        public DuckieDPad(Game game, Vector2 position) :
            this(game)
        {
            _centerPosition = position;
        }

        public override void Initialize()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _upArrow = Game.Content.Load<Texture2D>("UpArrow");
            _rightArrow = Game.Content.Load<Texture2D>("RightArrow");
            _downArrow = Game.Content.Load<Texture2D>("DownArrow");
            _leftArrow = Game.Content.Load<Texture2D>("LeftArrow");

            _upArrowPosition = new Vector2(
                _centerPosition.X - (_upArrow.Bounds.Width / 2),
                _centerPosition.Y - (_upArrow.Bounds.Height * 1.5f));
            _rightArrowPosition = new Vector2(
                _centerPosition.X + (_rightArrow.Bounds.Width / 2),
                _centerPosition.Y - (_rightArrow.Bounds.Height / 2));
            _downArrowPosition = new Vector2(
                _centerPosition.X - (_downArrow.Bounds.Width / 2),
                _centerPosition.Y + (_downArrow.Bounds.Height / 2));
            _leftArrowPosition = new Vector2(
                _centerPosition.X - (_leftArrow.Bounds.Width * 1.5f),
                _centerPosition.Y - (_leftArrow.Bounds.Height / 2));
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(_upArrow, _upArrowPosition, null, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            _spriteBatch.Draw(_rightArrow, _rightArrowPosition, null, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            _spriteBatch.Draw(_downArrow, _downArrowPosition, null, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            _spriteBatch.Draw(_leftArrow, _leftArrowPosition, null, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
            _spriteBatch.End();
        }
    }
}
