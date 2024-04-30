using UnityEngine;
using CW.Common;

namespace CW.Backgrounds
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[HelpURL(CwCommon.HelpUrlPrefix + "CwBackgroundPlanetModel")]
	[AddComponentMenu(CwCommon.ComponentMenuPrefix + "Background Planet Model")]
    public class CwBackgroundPlanetModel : CwChild
    {
		[SerializeField]
		private CwBackgroundPlanet parent;

		[SerializeField]
		private MeshFilter cachedMeshFilter;

		[SerializeField]
		private MeshRenderer cachedMeshRenderer;

		public MeshFilter CachedMeshFilter
		{
			get
			{
				return cachedMeshFilter;
			}
		}

		public MeshRenderer CachedMeshRenderer
		{
			get
			{
				return cachedMeshRenderer;
			}
		}

		public static CwBackgroundPlanetModel Create(CwBackgroundPlanet parent)
		{
			var gameObject = CwHelper.CreateGameObject("CwBackgroundPlanetModel", parent.gameObject.layer, parent.transform);
			var instance   = gameObject.AddComponent<CwBackgroundPlanetModel>();

			instance.parent             = parent;
			instance.cachedMeshFilter   = instance.GetComponent<MeshFilter>();
			instance.cachedMeshRenderer = instance.GetComponent<MeshRenderer>();

			return instance;
		}

		protected override IHasChildren GetParent()
		{
			return parent;
		}
	}
}

#if UNITY_EDITOR
namespace CW.Backgrounds
{
	using UnityEditor;
	using TARGET = CwBackgroundPlanetModel;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwBackgroundPlanetModel_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			BeginDisabled();
				Draw("parent");
			EndDisabled();
		}
	}
}
#endif