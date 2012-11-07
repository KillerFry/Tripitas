using System;
using DuckieGameState;


namespace Tripitas {
    public class ScreenFactory : IScreenFactory {
        public GameScreen CreateScreen(Type screenType) {
            if (screenType == typeof(GameplayBackground)) {
                return new GameplayBackground();
            }
            
            if (screenType == typeof(GameplayScreen)) {
                return new GameplayScreen();
            }
            
            return Activator.CreateInstance(screenType) as GameScreen;
        }
    }
}
