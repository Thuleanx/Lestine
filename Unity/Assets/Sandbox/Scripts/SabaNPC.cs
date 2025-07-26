using UnityEngine;

namespace Saba {
    [RequireComponent(typeof(SabaEntity))]
    [RequireComponent(typeof(SabaMovementComponent))]
	public class SabaNPC : MonoBehaviour {
        public void OnEnable() => SabaNPCController.instance?.Register(this);
        public void OnDisable() => SabaNPCController.instance?.Deregister(this);
    }
}
