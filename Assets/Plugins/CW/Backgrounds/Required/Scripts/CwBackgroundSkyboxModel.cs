using UnityEngine;
using CW.Common;

namespace CW.Backgrounds
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[HelpURL(CwCommon.HelpUrlPrefix + "CwBackgroundSkyboxModel")]
	[AddComponentMenu(CwCommon.ComponentMenuPrefix + "Background Skybox Model")]
    public class CwBackgroundSkyboxModel : CwChild
    {
		[SerializeField]
		private CwBackgroundSkybox parent;

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

		public static CwBackgroundSkyboxModel Create(CwBackgroundSkybox parent)
		{
			var gameObject = CwHelper.CreateGameObject("CwBackgroundSkyboxModel", parent.gameObject.layer, parent.transform);
			var instance   = gameObject.AddComponent<CwBackgroundSkyboxModel>();

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
	using TARGET = CwBackgroundSkyboxModel;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwBackgroundSkyboxModel_Editor : CwEditor
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