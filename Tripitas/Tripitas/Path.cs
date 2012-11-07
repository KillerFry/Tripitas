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
    public class Path {
        private VertexPositionColor[] _vertices;

        public VertexPositionColor[] Vertices {
            get { return _vertices; }
            set { _vertices = value; }
        }

        private short[] _indices;

        public short[] Indices {
            get { return _indices; }
            set { _indices = value; }
        }

        private List<float> _distance;

        public List<float> Distance {
            get { return _distance; }
            set { _distance = value; }
        }

        public Path() { }

        public Path(VertexPositionColor[] vertices) {
            _vertices = vertices;
            _indices = new short[_vertices.Length];
            for (int i = 0; i < _vertices.Length; i++) {
                _indices[i] = (short)i;
            }
        }

        public Path(List<Vector2> vertices) {
            _vertices = new VertexPositionColor[vertices.Count];
            _indices = new short[vertices.Count];
            _distance = new List<float>();
            for (int i = 0; i < vertices.Count; i++) {
                _vertices[i] = new VertexPositionColor(new Vector3(vertices[i], 0), Color.Black);
                _indices[i] = (short)i;
                _distance.Add(0);
                if (i > 1) {
                    _distance[i - i] = Vector2.Distance(vertices[i - 1], vertices[i]);
                }
            }
        }
    }
}