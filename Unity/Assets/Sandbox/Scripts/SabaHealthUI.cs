using UnityEngine;
using UnityEngine.UI;

namespace Saba {
    [RequireComponent(typeof(Slider))]
	public class SabaHealthUI : MonoBehaviour {
        Slider slider;

        void Awake() {
            slider = GetComponent<Slider>();
        }
    }
}
