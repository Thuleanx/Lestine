using UnityEngine;

namespace StatusEffects {
    public enum EMultipleApplicationsMode {
        Stack,
        Steal,
        Refresh,
        Ignore
    }

	public abstract class StatusEffect<T> : ScriptableObject {
        public float durationSeconds;
        public EMultipleApplicationsMode multiApplicationMode;

        public abstract void OnGranted(T entity, float intensity);
        public abstract void OnRemoved(T entity, float intensity);

        public abstract bool IsTickable();
        public abstract void Tick(T entity);
	}
}
