using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Tripitas {
    public static class RandomGenerator {
        public static Random _randomGenerator = new Random();

        public static int GetRandomNumber(int minValue, int maxValue) {
            return _randomGenerator.Next(minValue, maxValue);
        }
    }

    public class Tripa {
        #region Member fields

        private Texture2D _marquee;

        private SpriteFont _font;

        public Vector2 _position;

        public Vector2 Position {
            get { return _position; }
            set {
                _position = value;
                //_area = new BoundingSphere(new Vector3(_position, 0), 25.0f);
            }
        }

        public string _screenValue;

        public string ScreenValue {
            get { return _screenValue; }
            set { _screenValue = value; }
        }

        public float _scale;

        public float Scale {
            get { return _scale; }
            set { _scale = value; }
        }

        public Vector2 _pressStartSize;

        public Vector2 PressStartSize {
            get { return _pressStartSize; }
            set { _pressStartSize = value; }
        }

        public Tripa _matchingPair;

        public Tripa MatchingPair {
            get { return _matchingPair; }
            set { _matchingPair = value; }
        }

        public Vector2 _origin;

        public Vector2 Origin {
            get { return _origin; }
            set { _origin = value; }
        }

        private SpriteBatch _spriteBatch;

        private GraphicsDevice _graphicsDevice;

        public bool _done;

        public bool Done {
            get { return _done; }
            set { _done = value; }
        }

        public BoundingSphere _area;

        public BoundingSphere Area {
            get { return _area; }
            set { _area = value; }
        }

        public VertexPositionNormalTexture[] _marqueeVertices;

        public VertexPositionNormalTexture[] MarqueeVertices {
            get { return _marqueeVertices; }
            set { _marqueeVertices = value; }
        }

        public short[] _marqueeIndices;

        public short[] MarqueeIndices {
            get { return _marqueeIndices; }
            set { _marqueeIndices = value; }
        }

        #endregion

        public Tripa() {
            _marqueeVertices = new VertexPositionNormalTexture[4];
            _marqueeIndices = new short[] { 0, 1, 2, 0, 2, 3 };
        }

        public Tripa(string screenValue, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch) {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
            _screenValue = screenValue;
            _position = new Vector2(RandomGenerator.GetRandomNumber(80, 770), RandomGenerator.GetRandomNumber(80, 440));
            _scale = 1.0f;
            _area = new BoundingSphere(new Vector3(_position, 0), 25.0f);
            _marqueeVertices = new VertexPositionNormalTexture[4];
            _marqueeIndices = new short[] { 0, 1, 2, 0, 2, 3 };
        }

        public void LoadContent() {
            _pressStartSize = _font.MeasureString(_screenValue);
            _origin = new Vector2(_pressStartSize.X / 2, _pressStartSize.Y / 2 - 8);
        }

        public void SetMarqueeTexture(Texture2D texture) {
            _marquee = texture;
            _marqueeVertices[0] = new VertexPositionNormalTexture(new Vector3(_position.X - _marquee.Bounds.Width / 2, _position.Y - _marquee.Bounds.Height / 2, 0), new Vector3(0, 0, 1), new Vector2(0, 0));
            _marqueeVertices[1] = new VertexPositionNormalTexture(new Vector3(_position.X + _marquee.Bounds.Width / 2, _position.Y - _marquee.Bounds.Height / 2, 0), new Vector3(0, 0, 1), new Vector2(1, 0));
            _marqueeVertices[2] = new VertexPositionNormalTexture(new Vector3(_position.X + _marquee.Bounds.Width / 2, _position.Y + _marquee.Bounds.Height / 2, 0), new Vector3(0, 0, 1), new Vector2(1, 1));
            _marqueeVertices[3] = new VertexPositionNormalTexture(new Vector3(_position.X - _marquee.Bounds.Width / 2, _position.Y + _marquee.Bounds.Height / 2, 0), new Vector3(0, 0, 1), new Vector2(0, 1));
        }

        public void SetFont(SpriteFont font) {
            _font = font;
        }

        public void SetGraphicsDevice(GraphicsDevice graphicsDevice) {
            _graphicsDevice = graphicsDevice;
        }

        public void SetSpriteBatch(SpriteBatch spriteBatch) {
            _spriteBatch = spriteBatch;
        }

        public void Draw(GameTime gameTime, BasicEffect effect) {
            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, _screenValue, _position, Color.Black, 0.0f, _origin, _scale, SpriteEffects.None, 0);
            _spriteBatch.End();

            DrawPressStartMarqueeTexture(effect);
            //BoundingSphereRenderer.Render(_area, GraphicsDevice, _effect.View, _effect.Projection, Color.Black);
        }

        public bool TouchedPair(Vector2 touchPosition) {
            if (_area.Contains(new Vector3(touchPosition, 0)) == ContainmentType.Contains) {
                _done = true;
                _matchingPair.Done = true;
                return true;
            }
            return false;
        }

        public void DrawPressStartMarqueeTexture(BasicEffect _effect) {
            _effect.LightingEnabled = true;
            _effect.TextureEnabled = true;
            _effect.Texture = _marquee;

            foreach (EffectPass pass in _effect.CurrentTechnique.Passes) {
                pass.Apply();
                _graphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, _marqueeVertices, 0, _marqueeVertices.Length, _marqueeIndices, 0, 2);
            }

            _effect.TextureEnabled = false;
            _effect.LightingEnabled = false;
        }
    }
}