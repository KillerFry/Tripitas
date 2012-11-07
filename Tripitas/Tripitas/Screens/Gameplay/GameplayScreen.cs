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
using DuckieParticles;
using HighScores_WP;

namespace Tripitas {
    class GameplayScreen : GameScreen {
        #region Fields

        ContentManager _content;
        SpriteFont _gameFont;

        List<Tripa> _tripas = new List<Tripa>();
        bool _touchedTripa;
        Tripa _activeTripa;

        List<Vector2> _touchedPositions = new List<Vector2>();
        List<Vector2> _actualTouchedPositions = new List<Vector2>();
        Dictionary<string, Path> _donePairs = new Dictionary<string, Path>();

        VertexPositionColor[] _vertices;
        VertexPositionColor[] _loneVertex = new VertexPositionColor[1];

        short[] _indices;
        short[] _loneIndex = new short[1];

        BasicEffect _effect;

        Texture2D _marquee;
        float _movement = 0.0f;

        Vector2 _screenOrigin = new Vector2(0, 0);

        Vector3 _cameraPosition;
        Vector3 _target;

        Matrix _viewMatrix;
        Matrix _projectionMatrix;

        List<TouchLocation> _touchLocations = new List<TouchLocation>();
        TouchLocation touchLocation;

        public TouchLocation TouchLocation {
            get { return touchLocation; }
        }

        int _touchId;

        public List<TouchLocation> TouchLocations {
            get { return _touchLocations; }
        }

        public bool IsTouched {
            get { return _touchedTripa; }
        }

        Tripa trip;

        public int Score
        {
            get;
            protected set;
        }

        HighScores highScores;
        bool captureDone = false;

        #endregion

        #region Initialization

        public GameplayScreen() {
            Name = "GameplayScreen";

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _vertices = _loneVertex;
            _indices = _loneIndex;

            GC.Collect();
        }

        public override void Activate(bool instancePreserved) {
            if (!instancePreserved) {
                if (_content == null) {
                    _content = new ContentManager(ScreenManager.Game.Services, "Content");
                }

                RenderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Vector4, DepthFormat.None);
                _marquee = _content.Load<Texture2D>("square");
                _gameFont = _content.Load<SpriteFont>("Gameplay/tripaFont");

                Score = 0;

                highScores = new HighScores();
                highScores.InitializeTable("Normal", 20);
                highScores.LoadScores();

                _cameraPosition = new Vector3(0.0f, 0.0f, 1.0f);
                _target = Vector3.Forward;
                _viewMatrix = Matrix.CreateLookAt(_cameraPosition, _target, Vector3.Up);
                _projectionMatrix = Matrix.CreateOrthographicOffCenter(0, (float)ScreenManager.GraphicsDevice.Viewport.Width, (float)ScreenManager.GraphicsDevice.Viewport.Height, 0, 0.1f, 1000.0f);

                _effect = new BasicEffect(ScreenManager.GraphicsDevice);

                _effect.World = Matrix.Identity;
                _effect.View = _viewMatrix;
                _effect.Projection = _projectionMatrix;

                _effect.EnableDefaultLighting();
                _effect.DiffuseColor = Color.Black.ToVector3();

                if (Microsoft.Phone.Shell.PhoneApplicationService.Current.StartupMode == Microsoft.Phone.Shell.StartupMode.Launch) {
                    InitializeTripas();

                    foreach (Tripa tripa in _tripas) {
                        tripa.SetMarqueeTexture(_marquee);
                        tripa.SetFont(_gameFont);
                        tripa.SetSpriteBatch(ScreenManager.SpriteBatch);
                        tripa.SetGraphicsDevice(ScreenManager.GraphicsDevice);
                        tripa.LoadContent();
                    }

                    Thread.Sleep(1000);
                }
            }

            if (Microsoft.Phone.Shell.PhoneApplicationService.Current.State.ContainsKey("Tripas")) {
                _tripas = (List<Tripa>)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["Tripas"];
                for (int i = 0; i < _tripas.Count - 1; i += 2) {
                    _tripas[i].MatchingPair = _tripas[i + 1];
                    _tripas[i].SetMarqueeTexture(_marquee);
                    _tripas[i].SetFont(_gameFont);
                    _tripas[i].SetSpriteBatch(ScreenManager.SpriteBatch);
                    _tripas[i].SetGraphicsDevice(ScreenManager.GraphicsDevice);
                    _tripas[i + 1].MatchingPair = _tripas[i];
                    _tripas[i + 1].SetMarqueeTexture(_marquee);
                    _tripas[i + 1].SetFont(_gameFont);
                    _tripas[i + 1].SetSpriteBatch(ScreenManager.SpriteBatch);
                    _tripas[i + 1].SetGraphicsDevice(ScreenManager.GraphicsDevice);
                }
                _donePairs = (Dictionary<string, Path>)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["DonePairs"];
            }

            ScreenManager.Game.ResetElapsedTime();
        }

        public override void Deactivate() {
            foreach (Tripa tripa in _tripas) {
                tripa.MatchingPair = null;
            }
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["Tripas"] = _tripas;
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["DonePairs"] = _donePairs;
            
            base.Deactivate();
        }

        public override void Unload() {
            _content.Unload();

            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("Tripas");
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("DonePairs");
        }

        #endregion

        #region Update n' Draw

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            PlayerIndex player;
            if (input.IsNewButtonPress(Buttons.Back, ControllingPlayer, out player))
            {
                ScreenManager.AddScreen(new GameplayPause("Pause!"), ControllingPlayer);
            }

            bool screenDone = false;
            foreach (Tripa tripa in _tripas)
            {
                screenDone = tripa.Done;
                if (!screenDone)
                {
                    break;
                }
            }

            if (screenDone)
            {
                IsCoverable = true;
                ScreenManager.AddScreen(new GameplaySuccess("Success!"), ControllingPlayer);
                return;
            }

            TouchCollection touchState = input.TouchState;

            if (touchState.Count == 0)
            {
                return;
            }

            foreach (TouchLocation touch in touchState)
            {
                if (touch.Position.X > 100.0f && touch.Position.Y > 100.0f)
                {
                    _touchId = touch.Id;
                }
            }

            touchState.FindById(_touchId, out touchLocation);

            if (touchLocation.State == TouchLocationState.Released)
            {
                _activeTripa = null;
                _touchedTripa = false;
                _touchId = 0;
                _touchedPositions.Clear();
                _actualTouchedPositions.Clear();
                _vertices = _loneVertex;
                _indices = _loneIndex;
                return;
            }

            if (touchLocation.State == TouchLocationState.Pressed)
            {
                foreach (Tripa tripa in _tripas)
                {
                    if (tripa.Area.Contains(new Vector3(touchLocation.Position, 0)) == ContainmentType.Contains)
                    {
                        if (tripa.Done)
                        {
                            break; // GAME OVER!!!
                        }

                        _activeTripa = tripa;
                        _touchedTripa = true;
                        _touchedPositions.Add(touchLocation.Position);
                        _actualTouchedPositions.Add(touchLocation.Position);
                        break;
                    }
                }
            }

            if (touchLocation.State == TouchLocationState.Moved)
            {
                if (!_touchedPositions.Contains(touchLocation.Position) &&
                    _touchedTripa)
                {
                    if (_activeTripa.Done)
                    {
                        return;
                    }

                    Vector2 point1 = _touchedPositions[_touchedPositions.Count - 1];
                    Vector2 point2 = touchLocation.Position;

                    Vector2 delta = point2 - point1;
                    float distance = delta.LengthSquared();
                    Vector2 direction = delta / distance;
                    Vector2 newPoint = new Vector2();

                    for (float i = 0.05f; i < 1.0f; i = i + 0.05f)
                    {
                        newPoint = point1 + direction * (distance * i);
                        _touchedPositions.Add(newPoint);
                    }

                    foreach (KeyValuePair<string, Path> pair in _donePairs)
                    {
                        Path path = pair.Value;
                        for (int i = 1; i < path.Vertices.Length; i++)
                        {
                            if (_actualTouchedPositions.Count > 1)
                            {
                                if (
                                    Helpers.LineIntersect(
                                        new Line(new Vector2(path.Vertices[i - 1].Position.X, path.Vertices[i - 1].Position.Y), new Vector2(path.Vertices[i].Position.X, path.Vertices[i].Position.Y)),
                                        new Line(_actualTouchedPositions[_actualTouchedPositions.Count - 1], touchLocation.Position)))
                                {
                                    IsCoverable = true;
                                    ScreenManager.AddScreen(new GameplayGameOver("Fail!"), ControllingPlayer);
                                    return;
                                }
                            }
                        }
                    }

                    for (int i = 1; i < _actualTouchedPositions.Count - 1; i++)
                    {
                        if (Helpers.LineIntersect(
                            new Line(_actualTouchedPositions[i - 1], _actualTouchedPositions[i]),
                            new Line(_actualTouchedPositions[_actualTouchedPositions.Count - 1], touchLocation.Position)))
                        {
                            IsCoverable = true;
                            ScreenManager.AddScreen(new GameplayGameOver("Fail!"), ControllingPlayer);
                            return;
                        }
                    }

                    foreach (Tripa otherTripa in _tripas)
                    {
                        if (otherTripa == _activeTripa ||
                            otherTripa == _activeTripa.MatchingPair)
                        {
                            continue;
                        }

                        if (otherTripa.Area.Contains(new Vector3(touchLocation.Position, 0)) == ContainmentType.Contains)
                        {
                            IsCoverable = true;
                            ScreenManager.AddScreen(new GameplayGameOver("Fail!"), ControllingPlayer);
                            return;
                        }

                    }

                    _touchedPositions.Add(touchLocation.Position);
                    _actualTouchedPositions.Add(touchLocation.Position);

                    if (_activeTripa.MatchingPair.TouchedPair(touchLocation.Position))
                    {
                        if (_donePairs.ContainsKey(_activeTripa.ScreenValue)) return;
                        _donePairs.Add(_activeTripa.ScreenValue, new Path(_actualTouchedPositions));
                        Score++;
                        /*foreach (IGameComponent component in ScreenManager.Game.Components) {
                            if (component is DuckieParticleManager) {
                                DuckieParticleManager particleManager = component as DuckieParticleManager;
                                DuckieParticleCluster particleCluster = new DuckieParticleCluster();
                                particleManager.AddParticleCluster(particleCluster);
                                particleManager.AddTexture("particle");
                                particleCluster.AddParticles(10);
                                particleCluster.OnClusterUpdate += new DuckieParticleCluster.DuckieParticleClusterUpdate(ParticleUpdate);
                            }
                        }*/
                    }
                }
            }

            _vertices = new VertexPositionColor[_actualTouchedPositions.Count];
            _indices = new short[_actualTouchedPositions.Count];

            for (int i = 0; i < _actualTouchedPositions.Count; i++)
            {
                _vertices[i] = new VertexPositionColor(new Vector3(_actualTouchedPositions[i], 0), Color.Black);
                _indices[i] = (short)i;
            }
        }

        public void ParticleUpdate(DuckieParticle particle) {
            particle.Position += new Vector2(RandomGenerator.GetRandomNumber(-1, 5), RandomGenerator.GetRandomNumber(-1, 6));
        }

        public override void Draw(GameTime gameTime) {

            ScreenManager.GraphicsDevice.SetRenderTarget(RenderTarget);
            ScreenManager.GraphicsDevice.Clear(Color.Transparent);

            ScreenManager.GraphicsDevice.BlendState = BlendState.Opaque;
            ScreenManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (Tripa tripa in _tripas) {
                tripa.Draw(gameTime, _effect);
            }

            if (_vertices.Length > 1) {
                foreach (EffectPass pass in _effect.CurrentTechnique.Passes) {
                    pass.Apply();
                    ScreenManager.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, _vertices, 0, _vertices.Length, _indices, 0, _indices.Length - 1);
                }
            }

            if (_donePairs.Count > 0) {
                foreach (KeyValuePair<string, Path> pair in _donePairs) {
                    foreach (EffectPass pass in _effect.CurrentTechnique.Passes) {
                        pass.Apply();
                        ScreenManager.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, pair.Value.Vertices, 0, pair.Value.Vertices.Length, pair.Value.Indices, 0, pair.Value.Indices.Length - 1);
                    }
                }
            }
            //trip.Draw(gameTime, _effect);
            

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0) {
                ScreenManager.FadeBackBufferToBlack(1f - TransitionAlpha);
            }
        }

        #endregion

        #region Private Methods

        private void InitializeTripas() {
            Tripa tripa;
            Tripa tripaPair;
            for (int i = 0; i < 7; i++) {
                tripa = new Tripa(i.ToString(), ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch);
                tripaPair = new Tripa(i.ToString(), ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch);
                CheckRelocateTripaPosition(tripa);
                _tripas.Add(tripa);
                CheckRelocateTripaPosition(tripaPair);
                _tripas.Add(tripaPair);
                tripa.MatchingPair = tripaPair;
                tripaPair.MatchingPair = tripa;
            }
            tripa = null;
            tripaPair = null;
        }

        private void CheckRelocateTripaPosition(Tripa tripa) {
            bool reassign = true;
            while (reassign) {
                reassign = false;
                foreach (Tripa previousTripa in _tripas) {
                    while (previousTripa.Area.Contains(tripa.Area) == ContainmentType.Intersects) {
                        tripa.Position = new Vector2(RandomGenerator.GetRandomNumber(95, 770), RandomGenerator.GetRandomNumber(95, 440));
                        tripa.Area = new BoundingSphere(new Vector3(tripa.Position, 0), 25.0f);
                        reassign = true;
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public void ResetGameScreen(bool resetScore) {
            _touchId = 0;
            _activeTripa = null;
            _touchedTripa = false;
            _tripas.Clear();
            _donePairs.Clear();
            _touchedPositions.Clear();
            _actualTouchedPositions.Clear();
            _vertices = _loneVertex;
            _indices = _loneIndex;
            if (resetScore)
            {
                Score = 0;
            }

            InitializeTripas();

            foreach (Tripa tripa in _tripas) {
                tripa.SetMarqueeTexture(_marquee);
                tripa.SetFont(_gameFont);
                tripa.SetSpriteBatch(ScreenManager.SpriteBatch);
                tripa.SetGraphicsDevice(ScreenManager.GraphicsDevice);
                tripa.LoadContent();
            }

            IsCoverable = false;
        }
        
        #endregion
    }

    static class Helpers {
        public static bool LineIntersect(Line l1, Line l2) {
            bool result = false;
            float x;
            float y;

            if (l1.Slope != l2.Slope) {
                x = (l2.YIntercept - l1.YIntercept) / (l1.Slope - l2.Slope);
                if (Math.Abs(l1.Slope) < Math.Abs(l2.Slope)) {
                    y = l1.Slope * x + l1.YIntercept;
                } else {
                    y = l2.Slope * x + l2.YIntercept;
                }

                if ((l1.StartPosition.X - x) * (x - l1.EndPosition.X) >= 0 &&
                    (l1.StartPosition.Y - y) * (y - l1.EndPosition.Y) >= 0 &&
                    (l2.StartPosition.X - x) * (x - l2.EndPosition.X) >= 0 &&
                    (l2.StartPosition.Y - y) * (y - l2.EndPosition.Y) >= 0) {
                    result = true;
                }
            } else {
                if (l1.YIntercept == l2.YIntercept) {
                    result = Overlap(l1.StartPosition.X, l1.StartPosition.Y, l2);
                    if (!result) {
                        result = Overlap(l1.EndPosition.X, l1.EndPosition.Y, l2);
                    }
                    if (!result) {
                        result = Overlap(l2.StartPosition.X, l2.StartPosition.Y, l1);
                    }
                    if (!result) {
                        result = Overlap(l2.EndPosition.X, l2.StartPosition.Y, l1);
                    }
                }
            }

            return result;
        }

        private static bool Overlap(float x, float y, Line line) {
            if (x >= Math.Min(line.StartPosition.X, line.EndPosition.X) &&
                x <= Math.Max(line.StartPosition.X, line.EndPosition.X) &&
                y >= Math.Min(line.StartPosition.Y, line.EndPosition.Y) &&
                y <= Math.Min(line.StartPosition.Y, line.EndPosition.Y)) {
                return true;
            }
            return false;
        }
    }

    public struct Line {
        private Vector2 _startPosition;

        public Vector2 StartPosition {
            get { return _startPosition; }
            set { _startPosition = value; }
        }

        private Vector2 _endPosition;

        public Vector2 EndPosition {
            get { return _endPosition; }
            set { _endPosition = value; }
        }

        private float _distance;

        public float Distance {
            get { return _distance; }
            set { _distance = value; }
        }

        private float _slope;

        public float Slope {
            get { return _slope; }
            set { _slope = value; }
        }

        private float _yIntercept;

        public float YIntercept {
            get { return _yIntercept; }
        }

        public Line(Vector2 startPosition, Vector2 endPosition) {
            _startPosition = startPosition;
            _endPosition = endPosition;
            _distance = Vector2.Distance(_startPosition, _endPosition);
            if (startPosition.X != endPosition.X) {
                _slope = (_endPosition.Y - _startPosition.Y) / (_endPosition.X - _startPosition.X);
            } else {
                _slope = 10000;
            }
            _yIntercept = _startPosition.Y - _slope * _startPosition.X;
        }
    }
}
