using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using DuckieGameState;
using DuckieParticles;

namespace Tripitas
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game {
        #region Member Fields

        GraphicsDeviceManager _graphics;
        ScreenManager ScreenManager;
        DuckieParticleManager _particleManager;
        ScreenFactory _screenFactory;

        #endregion

        #region Initialization

        public Game1()
        {
            Content.RootDirectory = "Content";
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = true;
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            InitializeLandscapeGraphics();

            _screenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), _screenFactory);

            ScreenManager = new ScreenManager(this);
            _particleManager = new DuckieParticleManager(this);

            Components.Add(ScreenManager);
            Components.Add(_particleManager);

            Microsoft.Phone.Shell.PhoneApplicationService.Current.Launching +=
                new EventHandler<Microsoft.Phone.Shell.LaunchingEventArgs>(GameLaunching);
            Microsoft.Phone.Shell.PhoneApplicationService.Current.Activated +=
                new EventHandler<Microsoft.Phone.Shell.ActivatedEventArgs>(GameActivated);
            Microsoft.Phone.Shell.PhoneApplicationService.Current.Deactivated +=
                new EventHandler<Microsoft.Phone.Shell.DeactivatedEventArgs>(GameDeactivated);
        }

        private void InitializeLandscapeGraphics() {
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 480;
        }

        #endregion

        #region Update n' Draw

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Transparent);
            
            base.Draw(gameTime);
        }

        #endregion

        private void AddInitialScreens() {
            ScreenManager.AddScreen(new Background(), null);
            ScreenManager.AddScreen(new MainMenuScreen(), null);
        }

        void GameLaunching(object sender, Microsoft.Phone.Shell.LaunchingEventArgs e) {
            AddInitialScreens();
        }

        void GameActivated(object sender, Microsoft.Phone.Shell.ActivatedEventArgs e) {
            // Try to deserialize the screen manager
            if (!ScreenManager.Activate(e.IsApplicationInstancePreserved)) {
                // If the screen manager fails to deserialize, add the initial screens
                AddInitialScreens();
            }
        }

        void GameDeactivated(object sender, Microsoft.Phone.Shell.DeactivatedEventArgs e) {
            // Serialize the screen manager when the game deactived
            ScreenManager.Deactivate();
        }
    }
}