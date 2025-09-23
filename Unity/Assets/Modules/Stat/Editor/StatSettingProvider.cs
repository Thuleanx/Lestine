#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;

namespace Stats {
	public class StatSettingProvider : SettingsProvider {
		SerializedObject m_CustomSettings;

		public StatSettingProvider(string path, SettingsScope scope = SettingsScope.Project) : base(path, scope) {}

		public override void OnActivate(string searchContext, VisualElement rootElement) {
			m_CustomSettings = StatSettings.GetSerializedSettings(StatSettings.settingsPath);
		}

		public override void OnGUI(string searchContext) {
			EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("NumCoreStats"));
			EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("NumBaseStats"));
			EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("NumCoreScaling"));
			m_CustomSettings.ApplyModifiedPropertiesWithoutUndo();
		}

		[SettingsProvider]
		public static SettingsProvider CreateMySabaSettingsProvider() {
			return new StatSettingProvider("Module/Stats", SettingsScope.Project);
		}
	}
}
#endif
