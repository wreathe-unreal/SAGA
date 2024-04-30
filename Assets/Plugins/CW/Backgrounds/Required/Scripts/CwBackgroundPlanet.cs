using System.Collections.Generic;
using UnityEngine;
using CW.Common;

namespace CW.Backgrounds
{
	/// <summary>This component allows you to render a planet in the background of your scene.
	/// <b>SHADER SETTINGS</b>
	/// _CW_SurfaceRadius			= The radius of the planet relative to the radius of the planet mesh it's being rendered on. This is normally close to 1, but a lower value allows you to have a thicker atmosphere.
	/// _CW_SurfaceWarp				= This controls how much the planet texture is distorted toward the edges of the planet. A higher value can be used to give the illusion of a larger planet, or a more distant camera.
	/// _CW_SurfaceTiling			= The amount of times the planet texture is repeated across the planet mesh.
	/// _CW_SurfaceScroll			= The surface texture is scroll at this rate to give the illusion of rotation.
	/// _CW_SurfaceEdge				= The edge of the planet will fade out at this rate, to make the edge softer.
	/// _CW_AlbedoShift				= The planet texture color hue will be rotated by this amount.
	/// _CW_AlbedoTint				= The planet texture color will be tinted/multiplied by this color
	/// 
	/// _CW_LightingAngle			= The light will hit the planet from this angle in radians.
	/// _CW_LightingOffset			= The light/dark side will be offset across the planet by this distance.
	/// _CW_LightingSharpness		= The light/dark side transition distance sharpness.
	/// _CW_LightingColor			= The planet surface color will transition toward this, before it goes to the dark side.
	/// _CW_LightingPower			= The above color will transition with this sharpness.
	/// 
	/// _CW_CloudsEdge				= The clouds will disappear at this point toward the edge of the planet.
	/// _CW_CloudsTiling			= The cloud texture will be tiled this many times.
	/// _CW_CloudsScroll			= The cloud texture will scroll at this rate to give the illusion of rotation.
	/// _CW_CloudsThreshold			= This allows you to reduce the amount of visible clouds.
	/// _CW_CloudsColor				= The clouds will be given this color.
	/// 
	/// _CW_CloudsShadowColor		= The cloud shadows will have this color.
	/// _CW_CloudsShadowOffset		= The cloud shadow position will be offset by this distance.
	/// _CW_CloudsShadowBlur		= The cloud shadow blur amount.
	/// 
	/// _CW_AtmosphereEdgeShift		= The atmosphere transition will be shifted in/out by this distance.
	/// _CW_AtmosphereFog			= The atmosphere will be at least this thick at the center of the planet.
	/// _CW_AtmosphereFogMul		= The atmosphere's final thickness will be multiplied by this.
	/// _CW_AtmosphereFogPower		= The atmosphere's center to horizon thickness transition will be this sharp.
	/// _CW_AtmosphereBrightness	= The atmosphere color will be multiplied by this.
	/// _CW_AtmosphereOuter			= The atmosphere's horizon to outer color transition will be offset by this distance.
	/// _CW_AtmosphereInnerColor	= The atmosphere's color at the center of the planet.
	/// _CW_AtmosphereOuterColor	= The atmosphere's color at the horizon and into space.
	/// </summary>
	[ExecuteInEditMode]
	[HelpURL(CwCommon.HelpUrlPrefix + "CwBackgroundPlanet")]
	[AddComponentMenu(CwCommon.ComponentMenuPrefix + "Background Planet")]
    public class CwBackgroundPlanet : MonoBehaviour, CwChild.IHasChildren, CwBackground.IOrderable
    {
		/// <summary>The material used to render this component.
		/// NOTE: This must use the <b>CW/Backgrounds/BackgroundPlanet</b> shader.</summary>
		public Material Material { set { material = value; materialDirty = true; } get { return material; } } [SerializeField] private Material material;

		/// <summary>If this planet has lighting, should the lighting angle automatically change based on the scene light position?</summary>
		//public bool AutoRotateLighting { set { autoRotateLighting = value; } get { return autoRotateLighting; } } [SerializeField] private bool autoRotateLighting;

		/// <summary>The size of this planet in the sky.</summary>
		public float Arc { set { arc = value; meshDirty = true; } get { return arc; } } [SerializeField] private float arc = 0.1f;

		public int SegmentQuads { set { segmentQuads = value; meshDirty = true; } get { return segmentQuads; } } [SerializeField] private int segmentQuads = 64;

		public int RingQuads { set { ringQuads = value; meshDirty = true; } get { return ringQuads; } } [SerializeField] private int ringQuads = 32;

		/// <summary>This allows you to make this component override any settings of the base material.</summary>
		public CwMaterialOverrides MaterialOverrides { get { if (materialOverrides == null) materialOverrides = new CwMaterialOverrides(); return materialOverrides; } } [SerializeField] private CwMaterialOverrides materialOverrides;

		[SerializeField] private CwBackgroundPlanetModel model;

		[SerializeField] private float lightingAngle;

		[System.NonSerialized]
		private Mesh generatedMesh;

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

		private static int _CW_LightingAngle = Shader.PropertyToID("_CW_LightingAngle");

		public static CwBackgroundPlanet Create(int layer = 0, Transform parent = null)
		{
			return Create(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
		}

		public static CwBackgroundPlanet Create(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
		{
			var gameObject = CwHelper.CreateGameObject("Background Planet", layer, parent, localPosition, localRotation, localScale);
			var instance   = gameObject.AddComponent<CwBackgroundPlanet>();

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

			if (generatedMesh == null)
			{
				UpdateMesh();
			}

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

			//if (autoRotateLighting == true)
			//{
			//	properties.SetFloat(_CW_LightingAngle, lightingAngle * Mathf.Deg2Rad);
			//}

			if (materialOverrides != null)
			{
				materialOverrides.Apply(properties);
			}

			if (model != null)
			{
				model.CachedMeshRenderer.SetPropertyBlock(properties);
			}
		}

		//public void UpdateLightingAngle()
		//{
		//	if (CwBackgroundSun.Instances.Count > 0)
		//	{
		//		var sun   = CwBackgroundSun.GetInstance();
		//		var local = transform.InverseTransformPoint(sun.transform.forward);
		//
		//		lightingAngle = Mathf.Repeat(Vector2.SignedAngle(local, Vector2.down), 360.0f);
		//	}
		//}

		public void UpdateMesh()
		{
			meshDirty = false;

			ClearTempLists();
			GenerateTempLists();

			if (model == null)
			{
				model = CwBackgroundPlanetModel.Create(this);
			}

			if (generatedMesh == null)
			{
				generatedMesh = new Mesh();

				generatedMesh.name = "CwBackgroundPlanet (" + name + ")";

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

		private static Vector3 GetPoint(float s, float r)
		{
			return Quaternion.Euler(0.0f, 0.0f, s) * Quaternion.Euler(r, 0.0f, 0.0f) * new Vector3(0.0f, 0.0f, 1.0f);
		}

		private void GenerateTempLists()
		{
			var segmentVerts = segmentQuads + 1;
			var ringVerts    = ringQuads + 1;
			var stepS        = 360.0f / segmentQuads;
			var stepR        = arc    / ringQuads;

			for (var r = 1; r < ringVerts; r++)
			{
				for (var s = 0; s < segmentVerts; s++)
				{
					WritePoint(s * stepS, r * stepR);
				}
			}

			WritePoint(0.0f, 0.0f);

			var lastIndex = tempPositions.Count - 1;

			for (var s = 0; s < segmentQuads; s++)
			{
				var a = lastIndex;
				var b = s;
				var c = s + 1;

				tempIndices.Add(a); tempIndices.Add(c); tempIndices.Add(b);
			}

			for (var r = 1; r < ringQuads; r++)
			{
				var o = (r - 1) * segmentVerts;

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

		private void WritePoint(float s, float r)
		{
			var position = GetPoint(s, r);
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

			tempCoords.Add(new Vector2(r / arc, 0.0f));
		}
    }
}

#if UNITY_EDITOR
namespace CW.Backgrounds
{
	using UnityEditor;
	using TARGET = CwBackgroundPlanet;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwBackgroundPlanet_Editor : CwEditor
	{
		[MenuItem(CwCommon.GameObjectMenuPrefix + "Background Planet", false, 10)]
		public static void CreateMenuItem()
		{
			var parent   = CwHelper.GetSelectedParent();
			var instance = CwBackgroundPlanet.Create(parent != null ? parent.gameObject.layer : 0, parent);

			CwHelper.SelectAndPing(instance);
		}

		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			CwBackground_Editor.DrawNeedsBackground(tgts);

			var updateMesh     = false;
			var updateMaterial = false;

			BeginError(Any(tgts, t => t.Material == null));
				Draw("material", ref updateMaterial, "The material used to render this component.\n\nNOTE: This must use the <b>CW/Backgrounds/BackgroundPlanet</b> shader.");
			EndError();
			Draw("materialOverrides", "This allows you to make this component override any settings of the base material.");
			//Draw("autoRotateLighting", "If this planet has lighting, should the lighting angle automatically change based on the scene light position?");

			Separator();

			Draw("arc", ref updateMesh, "The size of this planet in the sky.");
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