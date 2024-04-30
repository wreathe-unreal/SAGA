using System.Collections.Generic;
using UnityEngine;
using CW.Common;

namespace CW.Backgrounds
{
	/// <summary>This component allows you to render a skybox in the background of your scene.
	/// <b>SHADER SETTINGS</b>
	/// _CW_Brightness				= The final brightness of the skybox will be multiplied by this.
	/// 
	/// _CW_DetailTiling			= The detail texture will repeat across the sky this many times.
	/// _CW_DetailStrength			= The brightness of the detail texture will be multiplied by this.
	/// 
	/// _CW_DetailOffsetTiling		= The detail offset texture will repeat across the sky this many times.
	/// _CW_DetailOffsetStrength	= The detail offset texture will influence the final sampling by this much.
	/// _CW_DetailBands				= The detail texture will be sliced into this many bands above and below the origin.
	/// _CW_DetailJitter			= The detail bands will each be offset by up to this random distance.
	/// _CW_DetailTransition		= The detail bands will fade into each other with this transition sharpness.
	/// </summary>
	[ExecuteInEditMode]
	[HelpURL(CwCommon.HelpUrlPrefix + "CwBackgroundSkybox")]
	[AddComponentMenu(CwCommon.ComponentMenuPrefix + "Background Skybox")]
    public class CwBackgroundSkybox : MonoBehaviour, CwChild.IHasChildren, CwBackground.IOrderable, CwBackground.IProcedural
    {
		/// <summary>The material used to render this component.
		/// NOTE: This must use the <b>CW/Backgrounds/BackgroundPlanet</b> shader.</summary>
		public Material Material { set { if (material != value) { material = value; stateDirty = true; } } get { return material; } } [SerializeField] private Material material;

		/// <summary>The random seed used when generating this component.
		/// NOTE: If the parent <b>CwBackground</b> component's <b>Seed</b> setting is not 0, then this seed value will automatically be overidden.</summary>
		public int Seed { set { seed = value; } get { return seed; } } [SerializeField] [CwSeed] private int seed;

		/// <summary>The amount of rows in the generated cloud mesh.</summary>
		public int Detail { set { if (detail != value) { detail = value; meshDirty = true; } } get { return detail; } } [SerializeField] private int detail = 20;

		/// <summary>This allows you to make this component override any settings of the base material.</summary>
		public CwMaterialOverrides MaterialOverrides { get { if (materialOverrides == null) materialOverrides = new CwMaterialOverrides(); return materialOverrides; } } [SerializeField] private CwMaterialOverrides materialOverrides;

		[SerializeField] private CwBackgroundSkyboxModel model;

		[System.NonSerialized]
		private Mesh generatedMesh;

		[System.NonSerialized]
		private bool stateDirty = true;

		[System.NonSerialized]
		private bool meshDirty;

		[System.NonSerialized]
		private int order;

		[System.NonSerialized]
		private MaterialPropertyBlock properties;

		private static List<Vector3> tempPositions = new List<Vector3>();

		private static List<Vector4> tempCoords = new List<Vector4>();

		private static List<int> tempIndices = new List<int>();

		public static CwBackgroundSkybox Create(int layer = 0, Transform parent = null)
		{
			return Create(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
		}

		public static CwBackgroundSkybox Create(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
		{
			var gameObject = CwHelper.CreateGameObject("Background Skybox", layer, parent, localPosition, localRotation, localScale);
			var instance   = gameObject.AddComponent<CwBackgroundSkybox>();

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

		public void SetSeed(int newSeed)
		{
			seed = newSeed;
		}

		protected virtual void OnEnable()
		{
			CwHelper.OnCameraPreRender += HandleCameraPreRender;

			if (model != null)
			{
				model.CachedMeshRenderer.enabled = true;
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
			if (stateDirty == true)
			{
				UpdateState();
			}

			if (meshDirty == true)
			{
				UpdateMesh();
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

			properties.SetFloat("_CW_DetailSeed", seed);

			if (materialOverrides != null)
			{
				materialOverrides.Apply(properties);
			}

			if (model != null)
			{
				model.CachedMeshRenderer.SetPropertyBlock(properties);
			}
		}

		[ContextMenu("Update Mesh")]
		public void UpdateMesh()
		{
			meshDirty = false;

			ClearTempLists();
			GenerateTempLists();

			if (generatedMesh == null)
			{
				generatedMesh = new Mesh();

				generatedMesh.name = "CwBackgroundSkybox (" + name + ")";

				generatedMesh.hideFlags = HideFlags.DontSave;

				if (model != null)
				{
					model.CachedMeshFilter.sharedMesh = generatedMesh;
				}
			}
			else
			{
				generatedMesh.Clear(false);
			}

			generatedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
			generatedMesh.SetVertices(tempPositions);
			generatedMesh.SetUVs(0, tempCoords);
			generatedMesh.SetTriangles(tempIndices, 0, false);
			generatedMesh.bounds = new Bounds(Vector3.zero, Vector3.one * 2.0f);
		}

		[ContextMenu("Update State")]
		public void UpdateState()
		{
			stateDirty = false;

			if (model == null)
			{
				model = CwBackgroundSkyboxModel.Create(this);
			}

			if (generatedMesh == null)
			{
				UpdateMesh();
			}

			model.CachedMeshFilter.sharedMesh = generatedMesh;

			model.CachedMeshRenderer.sharedMaterial = material;
			model.CachedMeshRenderer.sortingOrder   = order;
		}

		private void ClearTempLists()
		{
			tempPositions.Clear();
			tempCoords.Clear();
			tempIndices.Clear();
		}

		private void GenerateTempLists()
		{
			var quads = detail;
			var verts = detail + 1;
			var stepU = 1.0f / quads;
			var stepV = 1.0f / quads;

			for (var y = 0; y < verts; y++)
			{
				for (var x = 0; x < verts; x++)
				{
					var u = x * stepU;
					var v = y * stepV;
					var p = u * Mathf.PI * 2.0f + Mathf.PI;
					var q = v * Mathf.PI - Mathf.PI * 0.5f;
					var s = Mathf.Sin(p) * Mathf.Cos(q);
					var t = Mathf.Cos(p) * Mathf.Cos(q);

					tempPositions.Add(new Vector3(Mathf.Sin(p) * Mathf.Cos(q), Mathf.Sin(q), Mathf.Cos(p) * Mathf.Cos(q)));
					tempCoords.Add(new Vector4(u * 4.0f, v * 2.0f, s, t));
				}
			}

			for (var y = 0; y < quads; y++)
			{
				for (var x = 0; x < quads; x++)
				{
					var a = x + y * verts;
					var b = a + 1;
					var c = a + verts;
					var d = c + 1;

					tempIndices.Add(a); tempIndices.Add(c); tempIndices.Add(b);
					tempIndices.Add(d); tempIndices.Add(b); tempIndices.Add(c);
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace CW.Backgrounds
{
	using UnityEditor;
	using TARGET = CwBackgroundSkybox;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwBackgroundSkybox_Editor : CwEditor
	{
		[MenuItem(CwCommon.GameObjectMenuPrefix + "Background Skybox", false, 10)]
		public static void CreateMenuItem()
		{
			var parent   = CwHelper.GetSelectedParent();
			var instance = CwBackgroundSkybox.Create(parent != null ? parent.gameObject.layer : 0, parent);

			CwHelper.SelectAndPing(instance);
		}

		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			CwBackground_Editor.DrawNeedsBackground(tgts);

			var updateMesh  = false;
			var updateState = false;

			BeginError(Any(tgts, t => t.Material == null));
				Draw("material", ref updateState, "The material used to render this component.\n\nNOTE: This must use the <b>CW/Backgrounds/BackgroundPlanet</b> shader.");
			EndError();
			Draw("materialOverrides", "This allows you to make this component override any settings of the base material.");

			Separator();

			Draw("seed", "The random seed used when generating this component.\r\n\t\t/// NOTE: If the parent <b>CwBackground</b> component's <b>Seed</b> setting is not 0, then this seed value will automatically be overidden.");
			Draw("detail", ref updateMesh, "The amount of rows in the generated cloud mesh.");

			if (updateState == true)
			{
				Each(tgts, t => t.UpdateState(), true, true, "Update Material");
			}

			if (updateMesh == true)
			{
				Each(tgts, t => t.UpdateMesh(), true, true, "Update Mesh");
			}
		}
	}
}
#endif