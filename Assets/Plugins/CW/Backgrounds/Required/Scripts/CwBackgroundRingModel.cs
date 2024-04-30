using UnityEngine;
using CW.Common;

namespace CW.Backgrounds
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[HelpURL(CwCommon.HelpUrlPrefix + "CwBackgroundRingModel")]
	[AddComponentMenu(CwCommon.ComponentMenuPrefix + "Background Ring Model")]
    public class CwBackgroundRingModel : CwChild
    {
		[SerializeField]
		private CwBackgroundRing parent;

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

		public static CwBackgroundRingModel Create(CwBackgroundRing parent)
		{
			var gameObject = CwHelper.CreateGameObject("CwBackgroundRingModel", parent.gameObject.layer, parent.transform);
			var instance   = gameObject.AddComponent<CwBackgroundRingModel>();

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
	using TARGET = CwBackgroundRingModel;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwBackgroundRingModel_Editor : CwEditor
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