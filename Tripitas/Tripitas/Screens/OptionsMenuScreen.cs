using Microsoft.Xna.Framework;
using DuckieGameState;

namespace Tripitas {
    class OptionsMenuScreen : MenuScreen {
        MenuEntry _ungulateMenuEntry;
        MenuEntry _languageMenuEntry;

        public OptionsMenuScreen()
            : base("Options") {
                _ungulateMenuEntry = new MenuEntry(string.Empty);
                _languageMenuEntry = new MenuEntry(string.Empty);

                SetMenuEntryText();

                MenuEntries.Add(_ungulateMenuEntry);
                MenuEntries.Add(_languageMenuEntry);
        }

        void SetMenuEntryText() {
            _ungulateMenuEntry.EntryText = "Preferred ungulate: ";
            _languageMenuEntry.EntryText = "Language: ";
        }
    }
}
