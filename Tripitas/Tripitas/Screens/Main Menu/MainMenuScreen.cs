using Microsoft.Xna;
using Microsoft.Xna.Framework.Graphics;
using DuckieGameState;
using DuckieInput;

namespace Tripitas {
    class MainMenuScreen : MenuScreen {
        #region Initialization

        public MainMenuScreen()
            : base("Cat's Guts") {
            MenuEntry playGameMenuEntry = new MenuEntry("Play Game");
            //MenuEntry highScoreCaptureMenuEntry = new MenuEntry("Highs Score Capture");
            MenuEntry highScoreMenuEntry = new MenuEntry("High Scores");

            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            //highScoreCaptureMenuEntry.Selected += HighScoreCaptureMenuEntrySelected;
            highScoreMenuEntry.Selected += HighScoreMenuEntrySelected;

            MenuEntries.Add(playGameMenuEntry);
            //MenuEntries.Add(highScoreCaptureMenuEntry);
            MenuEntries.Add(highScoreMenuEntry);
        }

        #endregion

        #region Handle Input

        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e) {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayBackground(), new GameplayScreen(), new GameplayUI());
        }

        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e) {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }

        void HighScoreMenuEntrySelected(object sender, PlayerIndexEventArgs e) {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new HighScoreDisplay());
        }

        #endregion
    }
}
