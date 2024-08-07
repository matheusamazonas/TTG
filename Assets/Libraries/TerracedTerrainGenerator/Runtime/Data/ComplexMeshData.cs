using System;
using Unity.Collections;
using UnityEngine;

namespace LazySquirrelLabs.TerracedTerrainGenerator.Data
{
	/// <summary>
	/// Represents all the data necessary to build a complex mesh - a mesh with many sub meshes.
	/// </summary>
	internal class ComplexMeshData : MeshData
	{
		#region Fields

		/// <summary>
		/// All mesh data's vertices (key) and their indices (value). This is kept as a dictionary to ease
		/// finding existing vertices, reducing the number of duplicates.
		/// </summary>
		private NativeParallelHashMap<Vector3, int> _vertices;

		#endregion

		#region Properties

		/// <summary>
		/// All the vertices in the mesh data.
		/// </summary>
		internal NativeParallelHashMap<Vector3, int> Vertices => _vertices;

		#endregion

		#region Setup

		/// <summary>
		/// Creates mesh data for a complex mesh, allocating space for the provided vertex and indices amounts.
		/// </summary>
		/// <param name="vertexCount">The initial amount of mesh vertices.</param>
		/// <param name="indicesCount">The initial amount of mesh (triangle) indices.</param>
		/// <param name="subMeshes">The number of sub meshes.</param>
		/// <param name="allocator">The allocation strategy used when creating vertex and index buffers.</param>
		/// <exception cref="ArgumentException">Thrown whenever an invalid number of sub meshes is provided.</exception>
		internal ComplexMeshData(int vertexCount, int indicesCount, int subMeshes, Allocator allocator)
		{
			if (subMeshes < 1)
			{
				throw new ArgumentException("Mesh data must contain at least 1 sub mesh");
			}

			_vertices = new NativeParallelHashMap<Vector3, int>(vertexCount, allocator);
			IndicesPerSubMesh = new NativeArray<NativeList<int>>(subMeshes, allocator);
			// Estimate the number of indices per sub mesh
			var indicesPerSubMesh = indicesCount / subMeshes;

			for (var i = 0; i < subMeshes; i++)
			{
				IndicesPerSubMesh[i] = new NativeList<int>(indicesPerSubMesh, allocator);
			}
		}

		#endregion

		#region Public

		public override void Dispose()
		{
			base.Dispose();
			_vertices.Dispose();
		}

		#endregion

		#region Internal

		/// <inheritdoc cref="MeshData.AddTriangleAt"/>
		internal void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3, int subMesh, ref int index)
		{
			AddTriangleAt(v1, v2, v3, subMesh, ref index);
		}

		/// <inheritdoc cref="MeshData.AddQuadrilateralAt"/>
		internal void AddQuadrilateral(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, int subMesh, ref int index)
		{
			AddQuadrilateralAt(v1, v2, v3, v4, subMesh, ref index);
		}

		/// <summary>
		/// Fetches the mesh (triangle) indices for a given <paramref name="subMesh"/> index.
		/// </summary>
		/// <param name="subMesh">The index of the sub mesh to fetch the indices from.</param>
		/// <returns></returns>
		internal NativeList<int> GetIndices(int subMesh) => IndicesPerSubMesh[subMesh];

		#endregion

		#region Protected

		protected override int AddVertex(Vector3 vertex, ref int index)
		{
			if (_vertices.TryAdd(vertex, index))
			{
				_vertices[vertex] = index;
				var newIndex = index;
				index++;
				return newIndex;
			}

			return _vertices[vertex];
		}

		#endregion
	}
}