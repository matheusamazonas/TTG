using System;
using UnityEngine;

namespace SneakySquirrelLabs.TerracedTerrainGenerator.Sculpting
{
	/// <summary>
	/// Settings used during mesh deformation, the step that creates hills and valleys on the terrain.
	/// </summary>
	public readonly struct DeformationSettings
	{
		#region Properties

		/// <summary>
		/// Seed used by the randomizer to get random noise values.
		/// </summary>
		public int Seed { get; }

		/// <summary>
		/// The frequency of deformation (how many elements in a given area).
		/// </summary>
		public float Frequency { get; }

		/// <summary>
		/// The curve used to change the height distribution.
		/// </summary>
		public AnimationCurve HeightDistribution { get; }

		#endregion

		#region Setup

		/// <summary>
		/// <see cref="DeformationSettings"/>'s constructor. Initializes the deformer with a random seed.
		/// </summary>
		/// <param name="frequency">The frequency of deformation (how many elements in a given area).</param>
		/// <param name="heightDistribution">The curve used to change the height distribution. If it's null, the
		/// distribution won't be affected, thus it will be linear.</param>
		public DeformationSettings(float frequency, AnimationCurve heightDistribution)
			: this(GetRandomSeed(), frequency, heightDistribution)
		{

		}

		/// <summary>
		/// <see cref="DeformationSettings"/>'s constructor.
		/// </summary>
		/// <param name="seed">Seed used by the randomizer.</param>
		/// <param name="frequency">The frequency of deformation (how many elements in a given area).</param>
		/// <param name="heightDistribution">The curve used to change the height distribution. If it's null, the
		/// distribution won't be affected, thus it will be linear.</param>
		public DeformationSettings(int seed, float frequency, AnimationCurve heightDistribution)
		{
			if (frequency <= 0)
				throw new ArgumentOutOfRangeException(nameof(frequency));

			Seed = seed;
			Frequency = frequency;
			HeightDistribution = heightDistribution;
		}

		#endregion

		#region Private

		private static int GetRandomSeed()
		{
			var random = new System.Random();
			return random.Next();
		}

		#endregion
	}
}