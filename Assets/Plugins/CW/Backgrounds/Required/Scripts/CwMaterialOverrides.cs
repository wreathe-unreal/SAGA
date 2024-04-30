using System.Collections.Generic;
using UnityEngine;

namespace CW.Backgrounds
{
	[System.Serializable]
	public class CwMaterialOverrides
	{
		public enum PropertyType
		{
			Float,
			Vector,
			Color
		}

		[System.Serializable]
		public struct Pair
		{
			public string Key;

			public PropertyType ValueT;

			public float ValueF;

			public Vector4 ValueV;

			public Color ValueC;
		}

		[SerializeField]
		private List<Pair> pairs = new List<Pair>();

		public void Apply(MaterialPropertyBlock properties)
		{
			if (pairs != null)
			{
				foreach (var pair in pairs)
				{
					switch (pair.ValueT)
					{
						case PropertyType.Float: properties.SetFloat(pair.Key, pair.ValueF); break;
						case PropertyType.Vector: properties.SetVector(pair.Key, pair.ValueV); break;
						case PropertyType.Color: properties.SetColor(pair.Key, pair.ValueC); break;
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace CW.Backgrounds
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(CwMaterialOverrides))]
	public class CwMaterialOverrides_Drawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var material = property.serializedObject.FindProperty("material").objectReferenceValue as Material;
			
			if (material != null && material.shader != null)
			{
				var pairs = property.FindPropertyRelative("pairs");

				return (pairs.arraySize + 1) * EditorGUIUtility.singleLineHeight;
			}

			return EditorGUIUtility.singleLineHeight;
		}

		private static void DoInsert(SerializedProperty list, Material m, string k, UnityEngine.Rendering.ShaderPropertyType t)
		{
			list.arraySize += 1;

			var element = list.GetArrayElementAtIndex(list.arraySize - 1);

			element.FindPropertyRelative("Key").stringValue = k;

			switch (t)
			{
				case UnityEngine.Rendering.ShaderPropertyType.Color:
				{
					element.FindPropertyRelative("ValueT").enumValueIndex = (int)CwMaterialOverrides.PropertyType.Color;
					element.FindPropertyRelative("ValueC").colorValue     = m.GetColor(k);
				}
				break;

				case UnityEngine.Rendering.ShaderPropertyType.Vector:
				{
					element.FindPropertyRelative("ValueT").enumValueIndex = (int)CwMaterialOverrides.PropertyType.Vector;
					element.FindPropertyRelative("ValueV").vector4Value   = m.GetVector(k);
				}
				break;

				case UnityEngine.Rendering.ShaderPropertyType.Float:
				case UnityEngine.Rendering.ShaderPropertyType.Range:
				{
					element.FindPropertyRelative("ValueT").enumValueIndex = (int)CwMaterialOverrides.PropertyType.Float;
					element.FindPropertyRelative("ValueF").floatValue     = m.GetFloat(k);
				}
				break;
			}

			list.serializedObject.ApplyModifiedProperties();
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var pairs    = property.FindPropertyRelative("pairs");
			var material = property.serializedObject.FindProperty("material").objectReferenceValue as Material;

			position.height = EditorGUIUtility.singleLineHeight;

			EditorGUI.BeginProperty(position, label, property);
			{
				GUI.Label(position, label);

				if (material != null && material.shader != null)
				{
					var shader = material.shader;
					var rectA       = new Rect(position.xMax - 40, position.y, 40, position.height);
					var removeIndex = -1;

					if (GUI.Button(rectA, "Add") == true)
					{
						var menu  = new GenericMenu();
						var count = shader.GetPropertyCount();

						for (var i = 0; i < count; i++)
						{
							var t = shader.GetPropertyType(i);

							if (t != UnityEngine.Rendering.ShaderPropertyType.Texture)
							{
								var k = shader.GetPropertyName(i);

								menu.AddItem(new GUIContent(k), false, () => DoInsert(pairs, material, k, t));
							}
						}

						menu.ShowAsContext();
					}

					EditorGUI.indentLevel++;

					for (var i = 0; i < pairs.arraySize; i++)
					{
						position.y += EditorGUIUtility.singleLineHeight;

						var pair  = pairs.GetArrayElementAtIndex(i);
						var pairK = pair.FindPropertyRelative("Key").stringValue;
						var pairC = pair.FindPropertyRelative("ValueC");
						var pairV = pair.FindPropertyRelative("ValueV");
						var pairF = pair.FindPropertyRelative("ValueF");
						var pairT = (CwMaterialOverrides.PropertyType)pair.FindPropertyRelative("ValueT").enumValueIndex;
						var rectB = new Rect(position.x, position.y, position.width - 30, position.height);
						var rectC = new Rect(position.xMax - 25, position.y, 25, position.height);
						var index = shader.FindPropertyIndex(pairK);

						GUI.Label(EditorGUI.IndentedRect(position), pairK);

						if (index >= 0)
						{
							var t = shader.GetPropertyType(index);

							switch (t)
							{
								case UnityEngine.Rendering.ShaderPropertyType.Color:
								{
									pairC.colorValue = EditorGUI.ColorField(rectB, " ", pairC.colorValue);
								}
								break;

								case UnityEngine.Rendering.ShaderPropertyType.Vector:
								{
									pairV.vector4Value = EditorGUI.Vector4Field(rectB, " ", pairV.vector4Value);
								}
								break;

								case UnityEngine.Rendering.ShaderPropertyType.Float:
								{
									pairF.floatValue = EditorGUI.FloatField(rectB, " ", pairF.floatValue);
								}
								break;

								case UnityEngine.Rendering.ShaderPropertyType.Range:
								{
									var range = shader.GetPropertyRangeLimits(index);

									pairF.floatValue = EditorGUI.Slider(rectB, " ", pairF.floatValue, range.x, range.y);
								}
								break;
							}
						}

						if (GUI.Button(rectC, "x") == true)
						{
							removeIndex = i;
						}
					}

					EditorGUI.indentLevel--;

					if (removeIndex >= 0)
					{
						pairs.DeleteArrayElementAtIndex(removeIndex);
					}
				}
			}
			EditorGUI.EndProperty();
		}
	}
}
#endif