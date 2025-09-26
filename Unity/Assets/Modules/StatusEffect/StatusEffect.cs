using UnityEngine;

namespace StatusEffects {
    public enum EMultipleApplicationsMode {
        Stack,
        Steal,
        Refresh,
        Ignore
    }

	public abstract class StatusEffect : ScriptableObject {
        public float durationSeconds;
        public EMultipleApplicationsMode multiApplicationMode;

        public abstract void OnGranted();
        public abstract void OnRemoved();

        public abstract bool IsTickable();
	}
}
