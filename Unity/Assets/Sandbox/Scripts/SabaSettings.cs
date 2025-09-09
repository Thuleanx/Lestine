using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Saba {

	public class SabaSettings : ScriptableObject {
		public const string settingsPath = "Assets/Resources/SabaSettings.asset";
		public const string settingsPathNoDirectory = "SabaSettings";

        [System.Serializable]
        public struct FrameBudget {
            public float spawnMiliseconds;
        } 
        public FrameBudget frameBudget;
        public SabaEntityBudget entityBudget;

#if UNITY_EDITOR
		internal static SabaSettings GetOrCreateSettings() {
			var settings = AssetDatabase.LoadAssetAtPath<SabaSettings>(settingsPath);
			if (settings == null) {
				settings = ScriptableObject.CreateInstance<SabaSettings>();
				AssetDatabase.CreateAsset(settings, settingsPath);
				AssetDatabase.SaveAssets();
			}
			return settings;
		}

		public static SerializedObject GetSerializedSettings() { return new SerializedObject(GetOrCreateSettings()); }
#endif

		static SabaSettings cachedSettings = null;
		public static SabaSettings Get() {
			if (!cachedSettings) {
                Assert.IsNotNull(Resources.Load(settingsPathNoDirectory));
                cachedSettings = Resources.Load<SabaSettings>(settingsPathNoDirectory);
                Assert.IsNotNull(cachedSettings);
            }
			return cachedSettings;
		}
	}
}
