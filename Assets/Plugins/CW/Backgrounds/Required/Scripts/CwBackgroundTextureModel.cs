using UnityEngine;
using CW.Common;

namespace CW.Backgrounds
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[HelpURL(CwCommon.HelpUrlPrefix + "CwBackgroundTextureModel")]
	[AddComponentMenu(CwCommon.ComponentMenuPrefix + "Background Texture Model")]
    public class CwBackgroundTextureModel : CwChild
    {
		[SerializeField]
		private CwBackgroundTexture parent;

		[SerializeField]
		private MeshFilter cachedMeshFilter;

		[SerializeField]
		private MeshRenderer cachedMeshRenderer;

		public CwBackgroundTexture Parent
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

		public static CwBackgroundTextureModel Create(CwBackgroundTexture parent)
		{
			var gameObject = CwHelper.CreateGameObject("CwBackgroundTextureModel", parent.gameObject.layer, parent.transform);
			var instance   = gameObject.AddComponent<CwBackgroundTextureModel>();

			instance.parent             = parent;
			instance.cachedMeshFilter   = instance.GetComponent<MeshFilter>();
			instance.cachedMeshRenderer = instance.GetComponent<MeshRenderer>();

			return instance;
		}

		protected override IHasChildren GetParent()
		{
			return parent;
		}

#if UNITY_EDITOR
		protected virtual void Update()
		{
			if (parent != null && parent.HasChild(this) == false)
			{
				parent = null;
			}
		}
#endif
	}
}

#if UNITY_EDITOR
namespace CW.Backgrounds
{
	using UnityEditor;
	using TARGET = CwBackgroundTextureModel;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwBackgroundTextureModel_Editor : CwEditor
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