using System.Collections.Generic;
using UnityEngine;
using CW.Common;

namespace CW.Backgrounds
{
	/// <summary>This component allows you to render a skybox in the background of your scene.
	/// <b>SHADER SETTINGS</b>
	/// _CW_Color				= The final ring color will be tinted by this color.
	/// _CW_Brightness			= The final ring color will be multiplied by this.
	/// _CW_AlbedoShift			= The ring texture colors will be hue rotated by this many radians.
	/// 
	/// _CW_ShadowAngle			= The ring shadow will cast at this angle in radians.
	/// _CW_ShadowCircle		= The oblateness/roundness of the shadow shape.
	/// _CW_ShadowRadius		= The size of the shadow.
	/// _CW_ShadowBlur			= The penumbra thickness of the shadow.
	/// _CW_ShadowPenumbraColor	= The color of the penumbra.
	/// _CW_ShadowUmbraColor	= The color of the umbra.</summary>
	[ExecuteInEditMode]
	[HelpURL(CwCommon.HelpUrlPrefix + "CwBackgroundRing")]
	[AddComponentMenu(CwCommon.ComponentMenuPrefix + "Background Ring")]
    public class CwBackgroundRing : MonoBehaviour, CwChild.IHasChildren, CwBackground.IOrderable
    {
		/// <summary>The material used to render this component.
		/// NOTE: This must use the <b>CW/Backgrounds/BackgroundRing</b> shader.</summary>
		public Material Material { set { material = value; materialDirty = true; } get { return material; } } [SerializeField] private Material material;

		/// <summary>If this ring has lighting, should the lighting angle automatically change based on the scene light position?</summary>
		//public bool AutoRotateLighting { set { autoRotateLighting = value; } get { return autoRotateLighting; } } [SerializeField] private bool autoRotateLighting;

		/// <summary>The radius of the outer edge of the ring.</summary>
		public float Radius { set { radius = value; meshDirty = true; } get { return radius; } } [SerializeField] private float radius = 1.0f;

		/// <summary>Thickness of the ring, where 0.1 is 10% of the radius.</summary>
		public float Thickness { set { thickness = value; meshDirty = true; } get { return thickness; } } [SerializeField] [Range(0.0f, 1.0f)] private float thickness = 0.25f;

		/// <summary>The ring will be squashed by this 0..1 amount.</summary>
		public float Angle { set { angle = value; meshDirty = true; } get { return angle; } } [SerializeField] [Range(0.0f, 90.0f)] private float angle = 45.0f;

		/// <summary>How much the foreground and background parts of the ring will differ in thickness.</summary>
		public float Perspective { set { perspective = value; meshDirty = true; } get { return perspective; } } [SerializeField] [Range(0.01f, 0.99f)] private float perspective = 0.01f;

		public int SegmentQuads { set { segmentQuads = value; meshDirty = true; } get { return segmentQuads; } } [SerializeField] private int segmentQuads = 64;

		public int RingQuads { set { ringQuads = value; meshDirty = true; } get { return ringQuads; } } [SerializeField] private int ringQuads = 32;

		/// <summary>This allows you to make this component override any settings of the base material.</summary>
		public CwMaterialOverrides MaterialOverrides { get { if (materialOverrides == null) materialOverrides = new CwMaterialOverrides(); return materialOverrides; } } [SerializeField] private CwMaterialOverrides materialOverrides;

		[SerializeField] private float shadowAngle;

		[SerializeField] private CwBackgroundRingModel backgroundModel;

		[SerializeField] private CwBackgroundRingModel foregroundModel;

		[System.NonSerialized]
		private Mesh foregroundMesh;

		[System.NonSerialized]
		private Mesh backgroundMesh;

		[System.NonSerialized]
		private bool meshDirty;

		[System.NonSerialized]
		private bool materialDirty;

		[System.NonSerialized]
		private int order;

		private static List<Vector3> tempPositions = new List<Vector3>();

		private static List<Vector2> tempCoords = new List<Vector2>();

		private static List<int> tempIndices = new List<int>();

		private static Bounds tempBounds;

		private static MaterialPropertyBlock properties;

		private static int _CW_RingArc = Shader.PropertyToID("_CW_RingArc");
		//private static int _CW_ShadowAngle = Shader.PropertyToID("_CW_ShadowAngle");

		public static CwBackgroundRing Create(int layer = 0, Transform parent = null)
		{
			return Create(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
		}

		public static CwBackgroundRing Create(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
		{
			var gameObject = CwHelper.CreateGameObject("Background Ring", layer, parent, localPosition, localRotation, localScale);
			var instance   = gameObject.AddComponent<CwBackgroundRing>();

			return instance;
		}

		public bool HasChild(CwChild child)
		{
			return backgroundModel == child || foregroundModel == child;
		}

		public void SetOrder(ref int newOrder)
		{
			order = newOrder;

			if (backgroundModel != null)
			{
				backgroundModel.CachedMeshRenderer.sortingOrder = order - CwBackground.DRAW_ORDER_STEP / 2;
			}

			if (foregroundModel != null)
			{
				foregroundModel.CachedMeshRenderer.sortingOrder = order + CwBackground.DRAW_ORDER_STEP / 2;
			}
		}

		protected virtual void OnEnable()
		{
			CwHelper.OnCameraPreRender += HandleCameraPreRender;

			if (foregroundMesh == null || backgroundMesh == null)
			{
				UpdateMesh();
			}

			if (backgroundModel != null)
			{
				backgroundModel.CachedMeshRenderer.enabled = true;
			}

			if (foregroundModel != null)
			{
				foregroundModel.CachedMeshRenderer.enabled = true;
			}
		}

		protected virtual void OnDisable()
		{
			CwHelper.OnCameraPreRender -= HandleCameraPreRender;

			if (backgroundModel != null)
			{
				backgroundModel.CachedMeshRenderer.enabled = false;
			}

			if (foregroundModel != null)
			{
				foregroundModel.CachedMeshRenderer.enabled = false;
			}
		}

		protected virtual void OnDestroy()
		{
			DestroyImmediate(foregroundMesh);
			DestroyImmediate(backgroundMesh);
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

			//if (autoRotateLighting == true)
			//{
			//	UpdateLightingAngle();
			//}
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

			properties.SetVector(_CW_RingArc, new Vector2(radius * (1.0f - thickness), CwHelper.Reciprocal(radius * thickness)));

			///if (autoRotateLighting == true)
			//{
			//	properties.SetFloat(_CW_ShadowAngle, shadowAngle * Mathf.Deg2Rad);
			//}

			if (materialOverrides != null)
			{
				materialOverrides.Apply(properties);
			}

			if (backgroundModel != null)
			{
				backgroundModel.CachedMeshRenderer.SetPropertyBlock(properties);
			}

			if (foregroundModel != null)
			{
				foregroundModel.CachedMeshRenderer.SetPropertyBlock(properties);
			}
		}

		/*
		public void UpdateLightingAngle()
		{
			if (CwBackgroundSun.Instances.Count > 0)
			{
				var sun   = CwBackgroundSun.GetInstance();
				var local = transform.InverseTransformPoint(sun.transform.forward);

				local.y = Mathf.Abs(local.y);

				shadowAngle = Mathf.Repeat(Vector2.SignedAngle(local, Vector2.down), 360.0f);
			}
		}
		*/

		public void UpdateMesh()
		{
			meshDirty = false;

			if (backgroundModel == null)
			{
				backgroundModel = CwBackgroundRingModel.Create(this);
			}

			if (foregroundModel == null)
			{
				foregroundModel = CwBackgroundRingModel.Create(this);
			}

			UpdateMesh(ref foregroundMesh, true );
			UpdateMesh(ref backgroundMesh, false);

			backgroundModel.CachedMeshFilter.sharedMesh = backgroundMesh;
			foregroundModel.CachedMeshFilter.sharedMesh = foregroundMesh;

			backgroundModel.CachedMeshRenderer.sortingOrder = order - CwBackground.DRAW_ORDER_STEP / 2;
			foregroundModel.CachedMeshRenderer.sortingOrder = order + CwBackground.DRAW_ORDER_STEP / 2;
		}

		public void UpdateMesh(ref Mesh generatedMesh, bool foreground)
		{
			var distance       = 1.0f / perspective;
			var visibleRadians = Mathf.PI;

			if (distance > 0.0)
			{
				var arc = (radius * (1.0f - thickness)) / distance;

				if (arc > -1.0f && arc < 1.0f)
				{
					visibleRadians = Mathf.Acos(arc) * 2.0f;
				}
				else
				{
					visibleRadians = 0.0f;
				}
			}

			var inverseRadians = Mathf.PI * 2.0f - visibleRadians;

			ClearTempLists();

			if (foreground == true)
			{
				GenerateTempLists(Mathf.PI * 1.0f, visibleRadians);
			}
			else
			{
				GenerateTempLists(0.0f, inverseRadians);
			}

			if (generatedMesh == null)
			{
				generatedMesh = new Mesh();

				generatedMesh.name = "CwBackgroundRing (" + name + ")";

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
		}

		public void UpdateMaterial()
		{
			materialDirty = false;

			if (backgroundModel != null)
			{
				backgroundModel.CachedMeshRenderer.sharedMaterial = material;
			}

			if (foregroundModel != null)
			{
				foregroundModel.CachedMeshRenderer.sharedMaterial = material;
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

		private void GenerateTempLists(float bearing, float visible)
		{
			var segmentVerts = segmentQuads + 1;
			var ringVerts    = ringQuads + 1;
			var stepS        = visible / segmentQuads;
			var baseS        = bearing - visible * 0.5f;
			var stepR        = 1.0f / ringQuads;

			for (var r = 0; r < ringVerts; r++)
			{
				for (var s = 0; s < segmentVerts; s++)
				{
					WritePoint(baseS + s * stepS, r * stepR);
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

					tempIndices.Add(a); tempIndices.Add(b); tempIndices.Add(c);
					tempIndices.Add(d); tempIndices.Add(c); tempIndices.Add(b);
				}
			}
		}

		private Vector3 GetPoint(float s, float r01)
		{
			var r = Mathf.Lerp(radius * (1.0f - thickness), radius, r01);
			var x = Mathf.Sin(s) * r;
			var z = Mathf.Cos(s) * r;
			var p = Quaternion.Euler(Angle, 0.0f, 0.0f) * new Vector3(x, 0.0f, z);

			p.x /= perspective;
			p.z /= perspective;

			return (p + Vector3.forward / perspective).normalized;
		}

		private void WritePoint(float s, float r01)
		{
			var position = GetPoint(s, r01);
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

			var r = Mathf.Lerp(radius * (1.0f - thickness), radius, r01);
			var u = Mathf.Sin(s) * r;
			var v = Mathf.Cos(s) * r;

			tempCoords.Add(new Vector2(u, v));
		}
    }
}

#if UNITY_EDITOR
namespace CW.Backgrounds
{
	using UnityEditor;
	using TARGET = CwBackgroundRing;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwBackgroundRing_Editor : CwEditor
	{
		[MenuItem(CwCommon.GameObjectMenuPrefix + "Background Ring", false, 10)]
		public static void CreateMenuItem()
		{
			var parent   = CwHelper.GetSelectedParent();
			var instance = CwBackgroundRing.Create(parent != null ? parent.gameObject.layer : 0, parent);

			CwHelper.SelectAndPing(instance);
		}

		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			CwBackground_Editor.DrawNeedsBackground(tgts);

			var updateMesh     = false;
			var updateMaterial = false;

			BeginError(Any(tgts, t => t.Material == null));
				Draw("material", ref updateMaterial, "The material used to render this component.\n\nNOTE: This must use the <b>CW/Backgrounds/BackgroundRing</b> shader.");
			EndError();
			Draw("materialOverrides", "This allows you to make this component override any settings of the base material.");
			//Draw("autoRotateLighting", "If this ring has lighting, should the lighting angle automatically change based on the scene light position?");

			Separator();

			Draw("radius", ref updateMesh, "The radius of the outer edge of the ring.");
			Draw("thickness", ref updateMesh, "Thickness of the ring, where 0.1 is 10% of the radius.");
			Draw("angle", ref updateMesh, "The size of this ring in the sky.");
			Draw("perspective", ref updateMesh, "How much the foreground and background parts of the ring will differ in thickness.");
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