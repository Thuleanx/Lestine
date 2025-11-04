using UnityEngine;
using UnityEngine.Events;

using PrettyPatterns;

namespace eclipse.interactable {
	public class InteractionSource : MonoBehaviour {
        public UnityEvent<Interactable, Interactable> OnInteractableChange;

		Interactable closestInteractable;

		[SerializeField]
		Optional<float> updateClosestInteractableInterval;
		[SerializeField, Range(0, 10)]
		float range;
		[SerializeField,
		 Range(0, 3),
		 Tooltip("Time after leaving interaction range where we consider the interactable invalid")]
		float persistentSeconds;

		float secondsUntilTick = 0;
		float timeWhenLastInRange = 0;
		bool isInRangeOfInteractable = false;

		void Start() {
			// Don't need to update
			if (!updateClosestInteractableInterval.IsValid) enabled = false;
		}

		void Update() {
			secondsUntilTick -= Time.deltaTime;

			bool shouldInteractableExpire =
				!isInRangeOfInteractable &&
				timeWhenLastInRange + persistentSeconds + updateClosestInteractableInterval.GetWithDefault(0) <
					Time.time;
			if (shouldInteractableExpire && closestInteractable != null) {
                OnInteractableChange.Invoke(closestInteractable, null);
                closestInteractable = null;
            }

			if (secondsUntilTick <= 0) {
				Interactable nextInteractable = InteractableSystem.instance.PoolClosest(transform.position);
				Vector2 displacement = nextInteractable.transform.position - transform.position;
				float distanceSquared = displacement.sqrMagnitude;
                bool isInRange = distanceSquared > range * range;
				if (isInRange) nextInteractable = null;

				isInRangeOfInteractable = nextInteractable != null;
				if (isInRangeOfInteractable) {
                    timeWhenLastInRange = Time.time;

                    bool isNewInteractable = closestInteractable != nextInteractable;
                    if (isNewInteractable) {
                        OnInteractableChange.Invoke(closestInteractable, nextInteractable);
                        closestInteractable = nextInteractable;
                    }
                }
				secondsUntilTick = updateClosestInteractableInterval.Value;
			}
		}

		public void TryInteract() {
			if (!updateClosestInteractableInterval.IsValid) {
                Interactable last = closestInteractable;
				closestInteractable = InteractableSystem.instance.PoolClosest(transform.position);
                if (last != closestInteractable) {
                    OnInteractableChange.Invoke(last, closestInteractable);
                }
            }
			if (closestInteractable) {
                closestInteractable.Interact(this);
                // If the interactable is consumed
                if (!closestInteractable || !closestInteractable.gameObject.activeInHierarchy) {
                    OnInteractableChange.Invoke(closestInteractable, null);
                    closestInteractable = null;
                }
            }
		}
	}
}
