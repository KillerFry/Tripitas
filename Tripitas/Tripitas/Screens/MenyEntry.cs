using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DuckieGameState;

namespace Tripitas {
    class MenuEntry {
        #region Fields

        /// <summary>
        /// Text for the entry.
        /// </summary>
        string _entryText;

        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        float _selectionFade;

        /// <summary>
        /// The position at which the entry is drawn. This is set by the MenuScreen
        /// each frame in Update.
        /// </summary>
        Vector2 _position;
        
        short[] hitBoxIndices = { 0, 1, 3, 0, 3, 2 };
        VertexPositionColor[] vertices;

        public bool DrawHitBox { get; set; }

        public SpriteFont Font { get; set; }

        public Rectangle HitBox { get; protected set; }
        public int Padding { get; set; }
        #endregion

        #region Properties

        /// <summary>
        /// Menu entry text.
        /// </summary>
        public string EntryText {
            get { return _entryText; }
            set { _entryText = value; }
        }

        /// <summary>
        /// Menu entry position.
        /// </summary>
        public Vector2 Position {
            get { return _position; }
            set { _position = value; }
        }
        #endregion

        #region Events

        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;

        /// <summary>
        /// Method for raising the SelectedEvent.
        /// </summary>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex) {
            if (Selected != null) {
                Selected(this, new PlayerIndexEventArgs(playerIndex));
            }
        }

        #endregion

        #region Initialization

        public MenuEntry(string entryText) {
            _entryText = entryText;
            vertices = new VertexPositionColor[4];
        }

        #endregion

        #region Update n' Draw

        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime) {
            // There is no such thing as a free sandwhich... neither as a selected item
            // on Windows Phone, so we always force isSelected to false;
#if WINDOWS_PHONE
            isSelected = false;
#endif

            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected) {
                _selectionFade = Math.Min(_selectionFade + fadeSpeed, 1);
            } else {
                _selectionFade = Math.Max(_selectionFade - fadeSpeed, 0);
            }
        }

        public virtual void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch, SpriteFont font, Color color, bool isSelected, GameTime gameTime) {
            // There is no spoon... nor a selected item on Windows Phone, so we always
            // force isSelected to be false.
#if WINDOWS_PHONE
            isSelected = false;
#endif

            // Draw the selected entry in yellow, otherwise white.
            color = isSelected ? Color.Yellow : Color.Black;

            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;

            float pulsate = (float)Math.Sin(time * 6) + 1;

            float scale = 1;// + pulsate * 0.05f * _selectionFade;

            // Draw text, centered on the middle of each line
            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            spriteBatch.DrawString(font, _entryText, _position, color, 0, origin, scale, SpriteEffects.None, 0);

            if (DrawHitBox)
            {
                HitBox = GetMenuEntryHitBounds();
                vertices[0] = new VertexPositionColor(new Vector3(HitBox.Left, HitBox.Top, 0), Color.Black);
                vertices[1] = new VertexPositionColor(new Vector3(HitBox.Right, HitBox.Top, 0), Color.Black);
                vertices[2] = new VertexPositionColor(new Vector3(HitBox.Left, HitBox.Bottom, 0), Color.Black);
                vertices[3] = new VertexPositionColor(new Vector3(HitBox.Right, HitBox.Bottom, 0), Color.Black);

                BasicEffect effect = new BasicEffect(graphics);

                effect.World = Matrix.Identity;
                effect.View = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Forward, Vector3.Up);
                effect.Projection = Matrix.CreateOrthographicOffCenter(0, (float)graphics.Viewport.Width, (float)graphics.Viewport.Height, 0, 0.1f, 1000.0f);

                //effect.EnableDefaultLighting();
                effect.DiffuseColor = Color.Black.ToVector3();

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    short[] i = { 0, 1, 3, 2, 0 };
                    graphics.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices, 0, vertices.Length, i, 0, 4);
                }
            }
        }

        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight() {
            return Font.LineSpacing;
        }

        public virtual int GetWidth() {
            return (int)Font.MeasureString(_entryText).X;
        }

        #endregion

        public Rectangle GetMenuEntryHitBounds()
        {
            return new Rectangle(
                (int)Position.X - Padding,
                (int)Position.Y - GetHeight() / 2 - Padding,
                GetWidth() + Padding,
                GetHeight() + Padding);
        }
    }

    class PlayerIndexEventArgs : EventArgs {

        /// <summary>
        /// Player who triggered the event.
        /// </summary>
        public PlayerIndex PlayerIndex {
            get { return _playerIndex; }
        }

        PlayerIndex _playerIndex;

        public PlayerIndexEventArgs(PlayerIndex playerIndex) {
            _playerIndex = playerIndex;
        }

    }
}
