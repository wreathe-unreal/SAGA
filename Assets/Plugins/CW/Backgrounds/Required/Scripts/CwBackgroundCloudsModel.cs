using UnityEngine;
using CW.Common;

namespace CW.Backgrounds
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[HelpURL(CwCommon.HelpUrlPrefix + "CwBackgroundCloudsModel")]
	[AddComponentMenu(CwCommon.ComponentMenuPrefix + "Background Clouds Model")]
    public class CwBackgroundCloudsModel : CwChild
    {
		[SerializeField]
		private CwBackgroundClouds parent;

		[SerializeField]
		private MeshFilter cachedMeshFilter;

		[SerializeField]
		private MeshRenderer cachedMeshRenderer;

		public CwBackgroundClouds Parent
		{
			get
			{
				return parent;
			}
		}

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

		public static CwBackgroundCloudsModel Create(CwBackgroundClouds parent)
		{
			var gameObject = CwHelper.CreateGameObject("CwBackgroundCloudsModel", parent.gameObject.layer, parent.transform);
			var instance   = gameObject.AddComponent<CwBackgroundCloudsModel>();

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
	using TARGET = CwBackgroundCloudsModel;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwBackgroundCloudsModel_Editor : CwEditor
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