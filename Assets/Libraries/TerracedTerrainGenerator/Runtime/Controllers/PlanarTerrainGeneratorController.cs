using LazySquirrelLabs.TerracedTerrainGenerator.Sculpting;
using UnityEngine;

namespace LazySquirrelLabs.TerracedTerrainGenerator.Controllers
{
	/// <summary>
	/// A controller that helps with generating planar terraced terrains.
	/// </summary>
	public class PlanarTerrainGeneratorController : TerrainGeneratorController
	{
		#region Serialzied fields

		[SerializeField] private ushort _sides = 8;
		[SerializeField] private float _radius = 20;

		#endregion

		#region Protected

		private protected override TerrainGenerator GetGenerator(float maximumHeight, float[] relativeHeights,
		                                                         SculptSettings sculptSettings, ushort depth)
		{
			return new PlanarTerrainGenerator(_sides, _radius, maximumHeight, relativeHeights, sculptSettings, depth);
		}

		#endregion
	}
}