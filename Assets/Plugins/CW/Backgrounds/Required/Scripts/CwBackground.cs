using UnityEngine;
using CW.Common;
using System.Collections.Generic;

namespace CW.Backgrounds
{
	[ExecuteInEditMode]
    public class CwBackground : MonoBehaviour
    {
		public interface IOrderable
		{
			void SetOrder(ref int order);
		}

		public interface IProcedural
		{
			void SetSeed(int seed);
		}

		public enum FollowType
		{
			TargetTransform,
			MainCamera
		}

		public enum UpdateType
		{
			Update,
			LateUpdate
		}

		public static readonly int DRAW_ORDER_STEP = 10;

		/// <summary>What should this component follow?</summary>
		public FollowType Follow { set { follow = value; } get { return follow; } } [SerializeField] private FollowType follow = FollowType.MainCamera;

		/// <summary>The transform that will be followed.</summary>
		public Transform Target { set { target = value; } get { return target; } } [SerializeField] private Transform target;

		/// <summary>Where in the game loop should this component update?</summary>
		public UpdateType FollowIn { set { followIn = value; } get { return followIn; } } [SerializeField] private UpdateType followIn = UpdateType.LateUpdate;

		/// <summary>The base seed for all procedural background features.</summary>
		public int Seed { set { seed = value; } get { return seed; } } [SerializeField] [CwSeed] private int seed;

		private static List<IOrderable> tempOrderables  = new List<IOrderable>();
		private static List<IOrderable> tempOrderables2 = new List<IOrderable>();

		private static List<IProcedural> tempProcedurals  = new List<IProcedural>();
		private static List<IProcedural> tempProcedurals2 = new List<IProcedural>();

		/// <summary>This method will update the follow position now.</summary>
		[ContextMenu("Update Position")]
		public void UpdatePosition()
		{
			var finalTarget = target;

			if (follow == FollowType.MainCamera)
			{
				var mainCamera = Camera.main;

				if (mainCamera != null)
				{
					finalTarget = mainCamera.transform;
				}
			}

			if (finalTarget != null)
			{
				transform.position = finalTarget.position;
			}
		}

		private void GetComponentsInChildrenOrdered<T>(List<T> list, List<T> list2)
		{
			list.Clear();

			GetComponentsInChildrenOrdered_(transform, list, list2);
		}

		private static void GetComponentsInChildrenOrdered_<T>(Transform root, List<T> list, List<T> list2)
		{
			root.GetComponents(list2);

			list.AddRange(list2);

			for (var i = 0; i < root.childCount; i++)
			{
				GetComponentsInChildrenOrdered_(root.GetChild(i), list, list2);
			}
		}

		/// <summary>This method will update the sorting order of all child background objects.</summary>
		[ContextMenu("Update Order")]
		public void UpdateOrder()
		{
			GetComponentsInChildrenOrdered(tempOrderables, tempOrderables2);

			var order = tempOrderables.Count * -DRAW_ORDER_STEP;

			foreach (var tempOrderable in tempOrderables)
			{
				tempOrderable.SetOrder(ref order);
			}
		}

		/// <summary>This method will update the seed values of all child background objects.</summary>
		[ContextMenu("Update Seeds")]
		public void UpdateSeeds()
		{
			GetComponentsInChildrenOrdered(tempProcedurals, tempProcedurals2);

			var newSeed = seed;

			foreach (var tempProcedural in tempProcedurals)
			{
				tempProcedural.SetSeed(newSeed);

				newSeed = (newSeed + 899) * 881;
			}
		}

		protected virtual void OnEnable()
		{
			UpdateOrder();
			UpdateSeeds();
		}

		protected virtual void Update()
		{
			if (followIn == UpdateType.Update)
			{
				UpdatePosition();
			}

#if UNITY_EDITOR
			UpdateOrder();
			UpdateSeeds();
#endif
		}

		protected virtual void LateUpdate()
		{
			if (followIn == UpdateType.LateUpdate)
			{
				UpdatePosition();
			}
		}
	}
}

#if UNITY_EDITOR
namespace CW.Backgrounds
{
	using UnityEditor;
	using TARGET = CwBackground;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwBackground_Editor : CwEditor
	{
		public static void DrawNeedsBackground<U>(U[] tgts)
			where U : MonoBehaviour
		{
			var fix = false;

			foreach (var tgt in tgts)
			{
				if (tgt.GetComponentInParent<CwBackground>() == null)
				{
					if (HelpButton("To render properly, all background objects should be children of the CwBackground component.", MessageType.Warning, "Fix", 40) == true)
					{
						fix = true;
					}

					break;
				}
			}

			if (fix == true)
			{
				var background = CwHelper.FindAnyObjectByType<CwBackground>(true);

				if (background == null)
				{
					background = new GameObject("Background").AddComponent<CwBackground>();
				}

				foreach (var tgt in tgts)
				{
					if (tgt.GetComponentInParent<CwBackground>() == null)
					{
						tgt.transform.SetParent(background.transform, false);
					}
				}
			}
		}

		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			var updateSeeds = false;

			Draw("follow", "What should this component follow?");
			if (Any(tgts, t => t.Follow == CwBackground.FollowType.TargetTransform))
			{
				BeginIndent();
					BeginError(Any(tgts, t => t.Target == null));
						Draw("target", "The transform that will be followed.");
					EndError();
				EndIndent();
			}
			Draw("followIn", "Where in the game loop should this component update?");

			Draw("seed", ref updateSeeds, "The base seed for all procedural background features.");

			if (updateSeeds == true)
			{
				Each(tgts, t => t.UpdateSeeds());
			}
		}
	}
}
#endif