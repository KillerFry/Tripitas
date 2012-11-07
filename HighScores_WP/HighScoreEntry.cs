using System;
using System.Collections.Generic;

namespace HighScores_WP {
    public class HighScoreEntry : IComparer<HighScoreEntry> {
        public string Name { get; set; }
        public int Score { get; set; }
        public DateTime Date { get; set; }

        internal HighScoreEntry() {
            Name = string.Empty;
            Score = 0;
            Date = DateTime.MinValue;
        }

        public int Compare(HighScoreEntry x, HighScoreEntry y) {
            if (x.Score < y.Score) {
                return 1;
            } else if (x.Score > y.Score) {
                return -1;
            } else {
                if (x.Date < y.Date) {
                    return -1;
                } else if (x.Date < y.Date) {
                    return 1;
                } else {
                    return 0;
                }
            }
        }
    }
}
