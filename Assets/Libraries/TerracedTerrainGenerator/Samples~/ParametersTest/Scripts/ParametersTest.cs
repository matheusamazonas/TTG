using System;
using System.Threading;
using System.Threading.Tasks;
using SneakySquirrelLabs.TerracedTerrainGenerator.Settings;
using UnityEngine;

namespace SneakySquirrelLabs.TerracedTerrainGenerator.Samples
{
	[RequireComponent(typeof(Renderer))]
	public class ParametersTest : MonoBehaviour
	{
		#region Serialized fields

		[SerializeField, Range(3, 10)] private ushort _sides;
		[SerializeField, Range(0, 10)] private ushort _depth;
		[SerializeField, Range(1, 100)] private float _radius;
		[SerializeField, Range(0.1f, 100)] private float _height;
		[SerializeField, Range(0.01f, 1f)] private float _frequency;
		[SerializeField, Range(1, 50)] private int _terraceCount;
		[SerializeField] private Renderer _renderer;
		[SerializeField] private MeshFilter _meshFilter;
		[SerializeField] private AnimationCurve _heightCurve;
		[SerializeField] private bool _async;

		#endregion

		#region Fields

		private const float Interval = 5f;
		private CancellationTokenSource _cancellationTokenSource;
		private float _lastGeneration;

		#endregion

		#region Setup

		private void Awake()
		{
			Application.targetFrameRate = 60;
			_cancellationTokenSource = new CancellationTokenSource();
		}

		private async void Start()
		{
			await Generate();
		}

		private void OnDestroy()
		{
			_cancellationTokenSource.Cancel();
		}

		#endregion

		#region Update

		private async void Update()
		{
			if (Time.realtimeSinceStartup - _lastGeneration < Interval)
				return;

			await Generate();
		}

		#endregion

		#region Event handlers

		private void OnValidate()
		{
			if (_renderer == null) return;

			var materials = _renderer.sharedMaterials;
			if (materials.Length >= _terraceCount) return;
			
			var newMaterials = new Material[_terraceCount];
			Array.Copy(materials, newMaterials, materials.Length);
			var lastMaterial = materials[^1];
			for (var i = materials.Length; i < _terraceCount; i++)
				newMaterials[i] = lastMaterial;
			_renderer.sharedMaterials = newMaterials;
		}

		#endregion

		#region Private

		private async Task Generate()
		{
			_lastGeneration = Time.realtimeSinceStartup;
			var deformerSettings = new DeformationSettings(_height, _frequency, _heightCurve);
			var generator = new TerrainGenerator(_sides, _radius, deformerSettings, _depth, _terraceCount);
			var startTime = Time.realtimeSinceStartup;
			if (_async)
				await GenerateAsync(generator);
			else
				GenerateSynchronously(generator);
			var endTime = Time.realtimeSinceStartup;
			Debug.Log($"Generated terrain in {(endTime - startTime) * 1_000} milliseconds.");

			void GenerateSynchronously(TerrainGenerator terrainGenerator)
			{
				_meshFilter.mesh = terrainGenerator.GenerateTerrain();
			}

			async Task GenerateAsync(TerrainGenerator terrainGenerator)
			{
				var token = _cancellationTokenSource.Token;

				try
				{
					_meshFilter.mesh = await terrainGenerator.GenerateTerrainAsync(token);
				}
				catch (OperationCanceledException)
				{
					Debug.Log("Terrain generation was cancelled.");
				}
			}
		}

		#endregion
	}
}
