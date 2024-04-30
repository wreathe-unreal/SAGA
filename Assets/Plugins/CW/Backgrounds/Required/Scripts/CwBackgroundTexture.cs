using System.Collections.Generic;
using UnityEngine;
using CW.Common;

namespace CW.Backgrounds
{
	/// <summary>This component allows you to render a single texture in the background of your scene (e.g. an image of the sun).
	/// <b>SHADER SETTINGS</b>
	/// _CW_Color			= The texture color will be multiplied/tinted by this color.
	/// _CW_Brightness		= The final texture color will be multiplied by this.
	/// _CW_AlbedoShift		= The texture color hue will be rotated by this angle.</summary>
	[ExecuteInEditMode]
	[HelpURL(CwCommon.HelpUrlPrefix + "CwBackgroundTexture")]
	[AddComponentMenu(CwCommon.ComponentMenuPrefix + "Background Texture")]
    public class CwBackgroundTexture : MonoBehaviour, CwChild.IHasChildren, CwBackground.IOrderable
    {
		/// <summary>The material used to render this component.
		/// NOTE: This must use the <b>CW/Backgrounds/BackgroundTexture</b> shader.</summary>
		public Material Material { set { material = value; materialDirty = true; } get { return material; } } [SerializeField] private Material material;

		/// <summary>The size of this texture in the sky.</summary>
		public Vector2 Arc { set { arc = value; meshDirty = true; } get { return arc; } } [SerializeField] private Vector2 arc = new Vector2(10.0f, 10.0f);

		/// <summary>The offset of this texture in the sky.</summary>
		public Vector2 Offset { set { offset = value; meshDirty = true; } get { return offset; } } [SerializeField] private Vector2 offset;

		/// <summary>This background texture will use this texture rect from the <b>Material</b>'s texture.</summary>
		public Rect Rect { set { rect = value; meshDirty = true; } get { return rect; } } [SerializeField] private Rect rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

		public int SegmentQuads { set { segmentQuads = value; meshDirty = true; } get { return segmentQuads; } } [SerializeField] private int segmentQuads = 64;

		public int RingQuads { set { ringQuads = value; meshDirty = true; } get { return ringQuads; } } [SerializeField] private int ringQuads = 32;

		/// <summary>This allows you to make this component override any settings of the base material.</summary>
		public CwMaterialOverrides MaterialOverrides { get { if (materialOverrides == null) materialOverrides = new CwMaterialOverrides(); return materialOverrides; } } [SerializeField] private CwMaterialOverrides materialOverrides;

		[SerializeField] private CwBackgroundTextureModel model;

		[System.NonSerialized]
		private Mesh generatedMesh;

		[System.NonSerialized]
		private bool meshDirty = true;

		[System.NonSerialized]
		private bool materialDirty = true;

		[System.NonSerialized]
		private int order;

		[System.NonSerialized]
		private MaterialPropertyBlock properties;

		private static List<Vector3> tempPositions = new List<Vector3>();

		private static List<Vector2> tempCoords = new List<Vector2>();

		private static List<int> tempIndices = new List<int>();

		private static Bounds tempBounds;

		public static CwBackgroundTexture Create(int layer = 0, Transform parent = null)
		{
			return Create(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
		}

		public static CwBackgroundTexture Create(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
		{
			var gameObject = CwHelper.CreateGameObject("Background Texture", layer, parent, localPosition, localRotation, localScale);
			var instance   = gameObject.AddComponent<CwBackgroundTexture>();

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
			TryUpdate();
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

		private void TryUpdate()
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

		public void UpdateMesh()
		{
			meshDirty = false;

			ClearTempLists();
			GenerateTempLists();

			if (model == null)
			{
				model = CwBackgroundTextureModel.Create(this);
			}

			if (generatedMesh == null)
			{
				generatedMesh = new Mesh();

				generatedMesh.name = "CwBackgroundTexture (" + name + ")";

				generatedMesh.hideFlags = HideFlags.DontSave;
			}
			else
			{
				generatedMesh.Clear(false);
			}

			generatedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
			generatedMesh.SetVertices(tempPositions);
			generatedMesh.SetUVs(0, tempCoords);
			generatedMesh.SetTriangles(tempIndices, 0, false);
			generatedMesh.bounds = tempBounds;

			model.CachedMeshFilter.sharedMesh = generatedMesh;

			model.CachedMeshRenderer.sortingOrder = order;
		}

		public void UpdateMaterial()
		{
			materialDirty = false;

			if (model != null)
			{
				model.CachedMeshRenderer.sharedMaterial = material;
			}
		}

		private void ClearTempLists()
		{
			tempPositions.Clear();
			tempCoords.Clear();
			tempIndices.Clear();
		}

		// return.x = -PI   .. +PI
		// return.y = -PI/2 .. +PI/2
		public static Vector2 CartesianToPolar(Vector3 xyz)
		{
			var longitude = Mathf.Atan2(xyz.x, xyz.z);
			var latitude  = Mathf.Asin(xyz.y / xyz.magnitude);

			return new Vector2(longitude, latitude);
		}

		private Vector3 GetPoint(float u, float v)
		{
			u = arc.x * (u - 0.5f) + offset.x;
			v = arc.y * (v - 0.5f) + offset.y;

			return Quaternion.Euler(1.0f - v, u, 0.0f) * new Vector3(0.0f, 0.0f, 1.0f);
		}

		private void GenerateTempLists()
		{
			var segmentVerts = segmentQuads + 1;
			var ringVerts    = ringQuads + 1;
			var stepU        = 1.0f / segmentQuads;
			var stepV        = 1.0f / ringQuads;

			for (var v = 0; v < ringVerts; v++)
			{
				for (var u = 0; u < segmentVerts; u++)
				{
					WritePoint(u * stepU, v * stepV);
				}
			}

			for (var r = 0; r < ringQuads; r++)
			{
				var o = r * segmentVerts;

				for (var s = 0; s < segmentQuads; s++)
				{
					var a = o + s;
					var b = o + s + 1;
					var c = a + segmentVerts;
					var d = b + segmentVerts;

					tempIndices.Add(a); tempIndices.Add(c); tempIndices.Add(b);
					tempIndices.Add(d); tempIndices.Add(b); tempIndices.Add(c);
				}
			}
		}

		private void WritePoint(float u, float v)
		{
			var position = GetPoint(u, v);
			var polar    = CartesianToPolar(position);

			//tempPositions.Add(new Vector3(polar.x, polar.y, 0.0f));
			tempPositions.Add(position);

			if (tempPositions.Count == 1)
			{
				tempBounds = new Bounds(position, Vector3.zero);
			}
			else
			{
				tempBounds.Encapsulate(position);
			}

			u = Mathf.Lerp(rect.xMin, rect.xMax, u);
			v = Mathf.Lerp(rect.yMin, rect.yMax, v);

			tempCoords.Add(new Vector2(u, v));
		}
	}
}

#if UNITY_EDITOR
namespace CW.Backgrounds
{
	using UnityEditor;
	using TARGET = CwBackgroundTexture;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwBackgroundTexture_Editor : CwEditor
	{
		[MenuItem(CwCommon.GameObjectMenuPrefix + "Background Texture", false, 10)]
		public static void CreateMenuItem()
		{
			var parent   = CwHelper.GetSelectedParent();
			var instance = CwBackgroundTexture.Create(parent != null ? parent.gameObject.layer : 0, parent);

			CwHelper.SelectAndPing(instance);
		}

		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			CwBackground_Editor.DrawNeedsBackground(tgts);

			var updateMesh     = false;
			var updateMaterial = false;

			BeginError(Any(tgts, t => t.Material == null));
				Draw("material", ref updateMaterial, "The material used to render this component.\n\nNOTE: This must use the <b>CW/Backgrounds/BackgroundTexture</b> shader.");
			EndError();
			Draw("materialOverrides", "This allows you to make this component override any settings of the base material.");

			Separator();

			Draw("arc", ref updateMesh, "The size of this texture in the sky.");
			Draw("offset", ref updateMesh, "The offset of this texture in the sky.");
			Draw("rect", ref updateMesh, "This background texture will use this texture rect from the <b>Material</b>'s texture.");
			Draw("segmentQuads", ref updateMesh);
			Draw("ringQuads", ref updateMesh);

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