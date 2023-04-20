using System;
using System.Threading;
using System.Threading.Tasks;
using SneakySquirrelLabs.TerracedTerrainGenerator.Settings;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SneakySquirrelLabs.TerracedTerrainGenerator.Samples.Randomizer
{
	internal class Randomizer : MonoBehaviour
	{
		#region Serialized fields

		[SerializeField] private MeshFilter _meshFilter;

		#endregion
		
		#region Fields

		// Generation settings
		private const int TerraceCountMin = 3;
		private const int TerraceCountMax = 20;

		private const ushort SidesMin = 3;
		private const ushort SidesMax = 10;

		private const ushort DepthMin = 3;
		private const ushort DepthMax = 9;

		private const float Radius = 20;
		private static readonly AnimationCurve HeightCurve = AnimationCurve.Linear(0, 0, 1, 1);

		// Deformation settings
		private const float HeightMin = 8f;
		private const float HeightMax = 25f;

		private const float FrequencyMin = 0.02f;
		private const float FrequencyMax = 0.45f;
		
		// Loop fields  
		private const float Interval = 5f;
		private float _lastGeneration;
		private CancellationTokenSource _cancellationTokenSource;

		#endregion
		
		#region Setup

		private void Awake()
		{
			Application.targetFrameRate = 60;
			_cancellationTokenSource = new CancellationTokenSource();
		}

		private async void Start()
		{
			try
			{
				await GenerateTerrain();
			}
			catch (OperationCanceledException)
			{
				Debug.Log("Random terrain generation on start stopped because the operation was cancelled.");
			}
		}

		private void OnDestroy()
		{
			_cancellationTokenSource?.Cancel();
		}

		#endregion
		
		#region Update

		private async void Update()
		{
			if (Time.realtimeSinceStartup - _lastGeneration < Interval)
				return;

			try
			{
				await GenerateTerrain();
			}
			catch (OperationCanceledException)
			{
				Debug.Log("Random terrain generation stopped because the operation was cancelled.");
			}
		}

		#endregion

		#region Private

		private async Task GenerateTerrain()
		{
			// Deformation settings
			var height = Random.Range(HeightMin, HeightMax);
			var frequency = Random.Range(FrequencyMin, FrequencyMax);
			var deformationSettings = new DeformationSettings(height, frequency, HeightCurve);
			// Generation settings
			var sides = (ushort) Random.Range(SidesMin, SidesMax);
			var depth = (ushort) Random.Range(DepthMin, DepthMax);
			var terraceCount = Random.Range(TerraceCountMin, TerraceCountMax);
			
			var generator = new TerrainGenerator(sides, Radius, deformationSettings, depth, terraceCount);
			_lastGeneration = Time.realtimeSinceStartup;
			_meshFilter.mesh = await generator.GenerateTerrainAsync(_cancellationTokenSource.Token);
			Debug.Log($"Generated a terrain with {sides} sides, height {height}, depth {depth}, " +
			          $"{terraceCount} terraces and detail frequency {frequency:F3}.");
		}

		#endregion
	}
}