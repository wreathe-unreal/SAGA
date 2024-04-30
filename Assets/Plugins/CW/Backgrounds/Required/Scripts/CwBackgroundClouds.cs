using System.Collections.Generic;
using UnityEngine;
using CW.Common;

namespace CW.Backgrounds
{
	/// <summary>This component allows you to render thousands of high quality background clouds with minimal memory usage, and minimal performance impact.
	/// <b>SHADER SETTINGS</b>
	/// _CW_Brightness		= The final cloud color will be multiplied by this.
	/// _CW_AlbedoShift		= The cloud texture color hue will be rotated by this.
	/// _CW_AlbedoTint		= The cloud texture color will be multiplied/tinted by this color.
	/// 
	/// _CW_GridX			= The cloud texture is split up into this many columns.
	/// _CW_GridY			= The cloud texture is split up into this many rows.
	/// 
	/// _CW_DetailTiling	= The detail texture is tiled this many times across the clouds.
	/// _CW_DetailJitter	= The detail texture is randomly offset by up to this distance.
	/// _CW_DetailExposure	= The detail texture has this much influence over the cloud texture.
	/// _CW_DetailPowerMin	= The final cloud color will darkened by this amount in areas where the detail texture is dark.
	/// _CW_DetailPowerMax	= The final cloud color will darkened by this amount in areas where the detail texture is light.
	/// </summary>
	[ExecuteInEditMode]
	[HelpURL(CwCommon.HelpUrlPrefix + "CwBackgroundClouds")]
	[AddComponentMenu(CwCommon.ComponentMenuPrefix + "Background Clouds")]
    public class CwBackgroundClouds : MonoBehaviour, CwChild.IHasChildren, CwBackground.IOrderable, CwBackground.IProcedural
    {
		[System.Serializable]
		public struct StarBundle
		{
			public int            Count;
			public float          Scale;
			public AnimationCurve RadiusDistribution;
		}

		public enum OcclusionType
		{
			Auto,
			None,
			Explicit
		}

		/// <summary>The material used to render this component.
		/// NOTE: This must use the <b>CW/Backgrounds/BackgroundClouds</b> shader.</summary>
		public Material Material { set { material = value; materialDirty = true; } get { return material; } } [SerializeField] private Material material;

		/// <summary>The random seed used when generating this component.</summary>
		public int Seed { set { seed = value; meshDirty = true; meshDirty = true; } get { return seed; } } [SerializeField] [CwSeed] private int seed;

		/// <summary>The amount of clouds that will be generated.</summary>
		public int Count { set { count = value; meshDirty = true; } get { return count; } } [SerializeField] private int count = 64;

		/// <summary>The size of each cloud in the sky.</summary>
		public float Arc { set { arc = value; meshDirty = true; } get { return arc; } } [SerializeField] private float arc = 10.0f;

		/// <summary>Clouds will randomly be offset up to this distance so they appear less uniform.</summary>
		public float Noise { set { noise = value; meshDirty = true; } get { return noise; } } [SerializeField] private float noise;

		/// <summary>This allows you to make this component override any settings of the base material.</summary>
		public CwMaterialOverrides MaterialOverrides { get { if (materialOverrides == null) materialOverrides = new CwMaterialOverrides(); return materialOverrides; } } [SerializeField] private CwMaterialOverrides materialOverrides;

		[SerializeField] private CwBackgroundCloudsModel model;

		[System.NonSerialized]
		private Mesh generatedMesh;

		[System.NonSerialized]
		private bool meshDirty;

		[System.NonSerialized]
		private bool materialDirty;

		[System.NonSerialized]
		private int order;

		[System.NonSerialized]
		private MaterialPropertyBlock properties;

		private static List<Vector3> tempPoints = new List<Vector3>();

		private static List<Vector3> tempPositions = new List<Vector3>();

		private static List<Color32> tempColors = new List<Color32>();

		private static List<int> tempIndices = new List<int>();

		public static CwBackgroundClouds Create(int layer = 0, Transform parent = null)
		{
			return Create(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
		}

		public static CwBackgroundClouds Create(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
		{
			var gameObject = CwHelper.CreateGameObject("Background Clouds", layer, parent, localPosition, localRotation, localScale);
			var instance   = gameObject.AddComponent<CwBackgroundClouds>();

			return instance;
		}

		public bool HasChild(CwChild child)
		{
			return model == child;
		}

		public void SetOrder(ref int newOrder)
		{
			newOrder += CwBackground.DRAW_ORDER_STEP;

			order = newOrder;

			if (model != null)
			{
				model.CachedMeshRenderer.sortingOrder = order;
			}
		}

		protected virtual void OnEnable()
		{
			CwHelper.OnCameraPreRender += HandleCameraPreRender;

			if (model != null)
			{
				model.CachedMeshRenderer.enabled = true;
			}

			if (generatedMesh == null)
			{
				UpdateMesh();
			}
		}

		protected virtual void OnDisable()
		{
			CwHelper.OnCameraPreRender -= HandleCameraPreRender;

			if (model != null)
			{
				model.CachedMeshRenderer.enabled = false;
			}
		}

		protected virtual void OnDestroy()
		{
			DestroyImmediate(generatedMesh);
		}

		protected virtual void LateUpdate()
		{
			if (meshDirty == true)
			{
				UpdateMesh();
			}

			if (materialDirty == true)
			{
				UpdateMaterial();
			}
		}

		private void HandleCameraPreRender(Camera camera)
		{
			if (properties == null)
			{
				properties = new MaterialPropertyBlock();
			}
			else
			{
				properties.Clear();
			}

			if (materialOverrides != null)
			{
				materialOverrides.Apply(properties);
			}

			if (model != null)
			{
				model.CachedMeshRenderer.SetPropertyBlock(properties);
			}
		}

		public void UpdateMesh()
		{
			meshDirty = false;

			ClearTempLists();
			GenerateTempLists();

			if (generatedMesh == null)
			{
				generatedMesh = new Mesh();

				generatedMesh.name = "CwBackgroundClouds (" + name + ")";

				generatedMesh.hideFlags = HideFlags.DontSave;
			}
			else
			{
				generatedMesh.Clear(false);
			}

			generatedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
			generatedMesh.SetVertices(tempPositions);
			generatedMesh.SetColors(tempColors);
			generatedMesh.SetTriangles(tempIndices, 0);
			generatedMesh.bounds      = new Bounds(Vector3.zero, Vector3.one * 2.0f);

			if (model == null)
			{
				model = CwBackgroundCloudsModel.Create(this);
			}

			model.CachedMeshFilter.sharedMesh = generatedMesh;

			model.CachedMeshRenderer.sortingOrder = order;
		}

		public void UpdateMaterial()
		{
			if (model != null)
			{
				model.CachedMeshRenderer.sharedMaterial    = material;
				model.CachedMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				model.CachedMeshRenderer.receiveShadows    = false;
				model.CachedMeshRenderer.sortingOrder      = order;
			}

			materialDirty = false;
		}

		private void GenerateTempPoints()
		{
			tempPoints.Clear();

			var phi = Mathf.PI * (3.0f - Mathf.Sqrt(5.0f));

			CwHelper.BeginSeed(seed);
				for (var i = 0; i < count; i++)
				{
					// Rotate to Fibonacci sphere
					var y = 1.0f - (i / (count - 1.0f)) * 2.0f;
					var r = Mathf.Sqrt(1.0f - y * y);
					var t = phi * i;
					var x = Mathf.Cos(t) * r;
					var z = Mathf.Sin(t) * r;

					tempPoints.Add(new Vector3(x, y, z));
				}
			CwHelper.EndSeed();

			// Relax iterations
			for (var i = 0; i < 100; i++)
			{
				for (var p = 0; p < tempPoints.Count; p++)
				{
					Relax(p, 0.01f);
				}
			}
		}

		private void Relax(int i, float weight)
		{
			var closestIndex = -1;
			var closestDist  = float.PositiveInfinity;

			for (var p = 0; p < tempPoints.Count; p++)
			{
				if (p != i)
				{
					var dist = Vector3.Distance(tempPoints[p], tempPoints[i]);

					if (dist < closestDist)
					{
						closestDist  = dist;
						closestIndex = p;
					}
				}
			}

			if (closestIndex >= 0)
			{
				var vec = tempPoints[i] - tempPoints[closestIndex];

				tempPoints[i] = Vector3.Normalize(tempPoints[i] + vec * weight);
			}
		}

		private void GenerateTempLists()
		{
			GenerateTempPoints();

			CwHelper.BeginSeed(seed);
				for (var i = 0; i < count; i++)
				{
					var random   = (byte)Random.Range(0, 255);
					var alpha    = (byte)255;
					var index    = (byte)Random.Range(0, 255); // TODO: Allow the user to specify this?
					var rotation = Quaternion.LookRotation(tempPoints[i] + Random.insideUnitSphere * noise) * Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f));

					tempPositions.Add(rotation * Quaternion.Euler(-arc, -arc, 0.0f) * Vector3.forward);
					tempPositions.Add(rotation * Quaternion.Euler( arc, -arc, 0.0f) * Vector3.forward);
					tempPositions.Add(rotation * Quaternion.Euler(-arc,  arc, 0.0f) * Vector3.forward);
					tempPositions.Add(rotation * Quaternion.Euler( arc,  arc, 0.0f) * Vector3.forward);

					tempColors.Add(new Color32(random, alpha, index, 0));
					tempColors.Add(new Color32(random, alpha, index, 1));
					tempColors.Add(new Color32(random, alpha, index, 2));
					tempColors.Add(new Color32(random, alpha, index, 3));

					tempIndices.Add(i * 4 + 0);
					tempIndices.Add(i * 4 + 2);
					tempIndices.Add(i * 4 + 1);
					tempIndices.Add(i * 4 + 3);
					tempIndices.Add(i * 4 + 1);
					tempIndices.Add(i * 4 + 2);
				}
			CwHelper.EndSeed();
		}

		private void ClearTempLists()
		{
			tempPositions.Clear();
			tempColors.Clear();
			tempIndices.Clear();
		}

		public void SetSeed(int newSeed)
		{
			if (seed != newSeed)
			{
				seed      = newSeed;
				meshDirty = true;
			}
		}
	}
}

#if UNITY_EDITOR
namespace CW.Backgrounds
{
	using UnityEditor;
	using TARGET = CwBackgroundClouds;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwBackgroundClouds_Editor : CwEditor
	{
		[MenuItem(CwCommon.GameObjectMenuPrefix + "Background Clouds", false, 10)]
		public static void CreateMenuItem()
		{
			var parent   = CwHelper.GetSelectedParent();
			var instance = CwBackgroundClouds.Create(parent != null ? parent.gameObject.layer : 0, parent);

			CwHelper.SelectAndPing(instance);
		}

		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			CwBackground_Editor.DrawNeedsBackground(tgts);

			var updateMesh     = false;
			var updateMaterial = false;

			BeginError(Any(tgts, t => t.Material == null));
				Draw("material", ref updateMaterial, "The material used to render this component.\n\nNOTE: This must use the <b>CW/Backgrounds/BackgroundClouds</b> shader.");
			EndError();
			Draw("materialOverrides", "This allows you to make this component override any settings of the base material.");

			Separator();

			Draw("seed", ref updateMesh, ref updateMesh, "The random seed used when generating this component.");
			BeginError(Any(tgts, t => t.Count < 1));
				Draw("count", ref updateMesh, "The amount of clouds that will be generated.");
			EndError();
			Draw("arc", ref updateMesh, "The size of each clouds in the sky.");
			Draw("noise", ref updateMesh, "Clouds will randomly be offset up to this distance so they appear less uniform.");

			if (updateMesh == true)
			{
				Each(tgts, t => t.UpdateMesh(), true, true, "Update Mesh");
			}

			if (updateMaterial == true)
			{
				Each(tgts, t => t.UpdateMaterial(), true, true, "Update Material");
			}
		}
	}
}
#endif