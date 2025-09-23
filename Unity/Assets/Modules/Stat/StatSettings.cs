using UnityEngine;
using UnityEngine.Assertions;

using Scriptables;

namespace Stats {
	public class StatSettings : ScriptableSettings<StatSettings> {
		public const string settingsPath = "Assets/Resources/Settings/Stats.asset";
		public const string settingsPathNoDirectory = "Settings/Stats";

		public static StatSettings Value;
		public int NumCoreStats = 100;
		public int NumBaseStats = 100;
		public int NumCoreScaling = 100;

		public static StatSettings Get() {
			if (!cachedSettings) {
				Assert.IsNotNull(Resources.Load(settingsPathNoDirectory));
				cachedSettings = Resources.Load<StatSettings>(settingsPathNoDirectory);
				Assert.IsNotNull(cachedSettings);
			}
			return cachedSettings;
		}
	}
}
