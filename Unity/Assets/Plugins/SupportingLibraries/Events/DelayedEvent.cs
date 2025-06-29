using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Events {
    public class DelayedEvent : MonoBehaviour {
        [SerializeField] float delaySeconds;
        [SerializeField] UnityEvent callAfterDelay;

        public void Invoke() => StartCoroutine(InvokeDelayed());
        IEnumerator InvokeDelayed() {
            yield return new WaitForSeconds(delaySeconds);
            callAfterDelay?.Invoke();
        }
    }
}
