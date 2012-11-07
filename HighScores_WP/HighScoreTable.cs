using System;
using System.Collections.Generic;
using System.Text;

namespace HighScores_WP {
    public class HighScoreTable {
        private List<HighScoreEntry> _scoreEntries;
        private int _tableSize;

        public string Description { get; set; }

        internal HighScoreTable(int tableSize, string tableDescription) {
            Description = tableDescription;
            _tableSize = tableSize;

            Clear();
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<HighScoreEntry> Entries {
            get {
                return _scoreEntries.AsReadOnly();
            }
        }

        public void Clear() {
            _scoreEntries = new List<HighScoreEntry>();

            for (int i = 0; i < _tableSize; i++) {
                _scoreEntries.Add(new HighScoreEntry());
            }
        }

        public HighScoreEntry AddEntry(string name, int score) {
            return AddEntry(name, score, DateTime.Now);
        }

        internal HighScoreEntry AddEntry(string name, int score, DateTime date) {
            HighScoreEntry entry = new HighScoreEntry();
            entry.Name = name;
            entry.Score = score;
            entry.Date = date;

            _scoreEntries.Add(entry);

            _scoreEntries.Sort(new HighScoreEntry());

            if (_scoreEntries.Count > _tableSize) {
                _scoreEntries.RemoveAt(_tableSize);
            }

            if (_scoreEntries.Contains(entry)) {
                return entry;
            }

            return null;
        }

        public bool ScoreQualifies(int score) {
            return (score > _scoreEntries[_tableSize - 1].Score);
        }
    }
}
