using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace HighScores_WP {
    public class HighScores {
        private Dictionary<string, HighScoreTable> _highScoreTables;

        public string FileName { get; set; }

        public HighScores() {
            FileName = "Scores.dat";
            Clear();
        }

        public void InitializeTable(string tableName, int tableSize) {
            InitializeTable(tableName, tableSize, string.Empty);
        }

        public void InitializeTable(string tableName, int tableSize, string tableDescription) {
            if (!_highScoreTables.ContainsKey(tableName)) {
                _highScoreTables.Add(tableName, new HighScoreTable(tableSize, tableDescription));
            }
        }

        public HighScoreTable GetTable(string tableName) {
            if (_highScoreTables.ContainsKey(tableName)) {
                return _highScoreTables[tableName];
            } else {
                return null;
            }
        }

        public void Clear() {
            if (_highScoreTables == null) {
                _highScoreTables = new Dictionary<string, HighScoreTable>();
            }

            foreach (HighScoreTable table in _highScoreTables.Values) {
                table.Clear();
            }
        }

        public void LoadScores() {
            string fileContent;
            HighScoreTable table;

            try {
                Clear();

                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication()) {
                    if (!store.FileExists(FileName)) {
                        return;
                    }

                    using (StreamReader sr = new StreamReader(store.OpenFile(FileName, FileMode.Open))) {
                        fileContent = sr.ReadToEnd();
                    }
                }

                XDocument xDoc = XDocument.Parse(fileContent);

                var result = from c in xDoc.Root.Descendants("entry")
                             select new {
                                 TableName = c.Parent.Parent.Element("name").Value,
                                 Name = c.Element("name").Value,
                                 Score = c.Element("score").Value,
                                 Date = c.Element("date").Value
                             };

                foreach (var el in result) {
                    table = GetTable(el.TableName);
                    if (table != null) {
                        table.AddEntry(el.Name, int.Parse(el.Score), DateTime.Parse(el.Date));
                    }
                }

            } catch {
                Clear();
            }
        }

        public void SaveScores() {
            StringBuilder sb = new StringBuilder();
            XmlWriter xmlWriter = XmlWriter.Create(sb);
            HighScoreTable table;

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("highscores");

            foreach (string tableName in _highScoreTables.Keys) {
                table = _highScoreTables[tableName];
                xmlWriter.WriteStartElement("table");
                xmlWriter.WriteStartElement("name");
                xmlWriter.WriteString(tableName);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("entries");
                foreach (HighScoreEntry entry in table.Entries) {
                    if (entry.Date != DateTime.MinValue) {
                        xmlWriter.WriteStartElement("entry");

                        xmlWriter.WriteStartElement("score");
                        xmlWriter.WriteString(entry.Score.ToString());
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("name");
                        xmlWriter.WriteString(entry.Name);
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteStartElement("date");
                        xmlWriter.WriteString(entry.Date.ToString("yyyy-MM-ddTHH:mm:ss"));
                        xmlWriter.WriteEndElement();

                        xmlWriter.WriteEndElement();
                    }
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();

            xmlWriter.Close();

            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication()) {
                using (StreamWriter sw = new StreamWriter(store.CreateFile(FileName))) {
                    sw.Write(sb.ToString());
                }
            }
        }
    }
}
