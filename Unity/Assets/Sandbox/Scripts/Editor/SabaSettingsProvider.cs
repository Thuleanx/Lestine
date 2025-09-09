#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;

namespace Saba {
	public class SabaSettingsProvider : SettingsProvider {
		public const string sabaSettingsPath = "Assets/Resources/SabaSettings.asset";

		SerializedObject m_CustomSettings;

		public SabaSettingsProvider(string path, SettingsScope scope = SettingsScope.Project) : base(path, scope) {}

		public override void OnActivate(string searchContext, VisualElement rootElement) {
			m_CustomSettings = SabaSettings.GetSerializedSettings();
		}

		public override void OnGUI(string searchContext) {
			EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("entityBudget"));
			m_CustomSettings.ApplyModifiedPropertiesWithoutUndo();
		}

		[SettingsProvider]
		public static SettingsProvider CreateMySabaSettingsProvider() {
			return new SabaSettingsProvider("Project/Saba", SettingsScope.Project);
		}
	}
}
#endif
