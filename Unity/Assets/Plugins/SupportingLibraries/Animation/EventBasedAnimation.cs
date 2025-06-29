using UnityEngine;
using UnityEngine.Events;

namespace Anime {
    public class EventBasedAnimation : MonoBehaviour {
        [SerializeField] bool shouldTriggerOnAwake;
        [SerializeField] bool shouldTriggerOnEnable;
        [SerializeField] protected UnityEvent onAnimateDone;

        public virtual void Awake() {
            if (shouldTriggerOnAwake) Animate();
        }

        public virtual void OnEnable() {
            if (shouldTriggerOnEnable) Animate();
        }

        public virtual void Animate() {
            onAnimateDone?.Invoke();
        }
    }
}
