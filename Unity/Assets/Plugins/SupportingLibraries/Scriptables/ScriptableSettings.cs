using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Scriptables {
	public abstract class ScriptableSettings<T> : ScriptableObject
		where T : ScriptableObject {
#if UNITY_EDITOR
		public static T GetOrCreateSettings(string path) {
			T settings = AssetDatabase.LoadAssetAtPath<T>(path);
			if (settings == null) {
				settings = ScriptableObject.CreateInstance<T>();
				AssetDatabase.CreateAsset(settings, path);
				AssetDatabase.SaveAssets();
			}
			return settings;
		}

		public static SerializedObject GetSerializedSettings(string path) {
			return new SerializedObject(GetOrCreateSettings(path));
		}
#endif

		protected static T cachedSettings = null;
	}
}
