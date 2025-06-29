using UnityEngine;
using UnityEngine.Events;

namespace Scriptables {
    [CreateAssetMenu(fileName = "Data",
                    menuName = "~/Scriptable/Event", order = 1)]
    public class ScriptableEvent : ScriptableObject {
        [System.NonSerialized]
        UnityEvent Event = new UnityEvent();

        public void Invoke() => Event?.Invoke();
        public void Subscribe(UnityAction action) => Event.AddListener(action);
        public void Unsubscribe(UnityAction action) => Event.RemoveListener(action);
    }
}
