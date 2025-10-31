using UnityEditor;
using UnityEngine;
using Framework.Editor;
using Framework.Editor.GUIUtilities;

namespace PrettyPatterns.Editor {
	[CustomPropertyDrawer(typeof(FrugalList<>))]
	public class FrugalListPropertyDrawer : PropertyDrawer {
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return 0;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			var hasFirstProp = property.FindPropertyRelative("hasFirst");
			var firstProp = property.FindPropertyRelative("first");
			var rest = property.FindPropertyRelative("rest");

            // Just a precaution, shouldn't ever trigger
            if (!hasFirstProp.boolValue) {
                while (rest.arraySize > 0) {
                    rest.DeleteArrayElementAtIndex(0);
                }
            }

			int size = rest.arraySize + (hasFirstProp.boolValue ? 1 : 0);

			using (new EditorGUILayout.VerticalScope()) {
				using (new EditorGUILayout.HorizontalScope()) {
					property.isExpanded = EditorGUILayout.Foldout(
						property.isExpanded, new GUIContent($"{property.displayName} ({size})")
					);
					if (GUILayout.Button(new GUIContent("+", "Add new element"), GUILayout.Width(40f))) {
						if (!hasFirstProp.boolValue) {
							hasFirstProp.boolValue = true;
						} else {
							rest.InsertArrayElementAtIndex(rest.arraySize);
							rest.GetArrayElementAtIndex(rest.arraySize - 1).isExpanded = true;
						}
					}
				}

				if (property.isExpanded) {
					using (new GUIIndent()) {
						if (size > 0) {
							var indexToRemove = -1;
                            using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
                                using (new EditorGUILayout.VerticalScope()) {
                                    DefaultDrawElement(firstProp, 0);
                                }

                                if (size == 1 && GUILayout.Button(new GUIContent("×", "Remove element"), GUILayout.Width(25f))) {
                                    indexToRemove = 0;
                                }
                            }

							for (var i = 0; i < size-1; i++) {
								using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
									using (new EditorGUILayout.VerticalScope()) {
										DefaultDrawElement(rest.GetArrayElementAtIndex(i), i+1);
									}

									if (GUILayout.Button(new GUIContent("×", "Remove element"), GUILayout.Width(25f))) {
										indexToRemove = i + 1;
									}
								}
							}

							if (indexToRemove >= 0) {
								if (indexToRemove == 0) {
									hasFirstProp.boolValue = false;
								} else {
									rest.DeleteArrayElementAtIndex(indexToRemove - 1);
								}
							}
						} else {
							EditorGUILayout.LabelField("Empty...");
						}
					}
				}
			}
		}

		static void DefaultDrawElement(SerializedProperty element, int index) {
			var guiContent = new GUIContent($"Element {index}");

			if (element.propertyType == SerializedPropertyType.Generic) {
				element.isExpanded = EditorGUILayout.Foldout(element.isExpanded, guiContent);

				if (element.isExpanded) {
					using (new EditorGUILayout.VerticalScope()) {
						using (new GUIIndent()) {
							foreach (var property in element.GetChildren()) {
								EditorGUILayout.PropertyField(property, true);
							}
						}
					}
				}
			} else {
				EditorGUILayout.PropertyField(element, guiContent, true);
			}
		}
	}
}
