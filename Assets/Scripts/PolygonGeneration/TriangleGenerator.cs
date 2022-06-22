using SneakySquirrelLabs.TerracedTerrainGenerator.Utils;
using UnityEngine;

namespace SneakySquirrelLabs.TerracedTerrainGenerator.PolygonGeneration
{
    /// <summary>
    /// Generates an equilateral triangle mesh.
    /// </summary>
    internal class TriangleGenerator : PolygonGenerator
    {
        #region Setup

        /// <summary>
        /// Creates a triangle generator
        /// </summary>
        /// <param name="size">The size of the generated triangle (the distance between the center and
        /// its vertices).</param>
        internal TriangleGenerator(float size) : base(size) { }

        #endregion

        #region Internal

        internal override Mesh Generate(bool calculateNormals)
        {
            var mesh = new Mesh
            {
                name = "Terraced Terrain Mesh"
            };

            var vertices = CreateVertices(Radius);
            mesh.SetVertices(vertices);
            var triangles = new[] {0, 1, 2};
            mesh.SetTriangles(triangles, 0, false, 0);
            if (calculateNormals)
                mesh.RecalculateNormals();
            
            return mesh;

            static Vector3[] CreateVertices(float radius)
            {
                var vertices = new Vector3[3];
                vertices[0] = new Vector3(radius, 0f, 0f);
                vertices[1] = vertices[0].Rotate(-120f);
                vertices[2] = vertices[1].Rotate(-120f);
                return vertices;
            }
        }

        #endregion
    }
}