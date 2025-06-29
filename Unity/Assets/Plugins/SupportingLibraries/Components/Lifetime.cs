using UnityEngine;
using System.Collections;
using Optimization;

namespace Behaviours {
    public class Lifetime : MonoBehaviour {
        [SerializeField] float time;
        Coroutine destroying;

        void OnEnable() {
            destroying = StartCoroutine(DestroyDelayed(time));
        }

        void OnDisable() {
            if (destroying != null) StopCoroutine(destroying);
        }

        IEnumerator DestroyDelayed(float time) {
            yield return new WaitForSeconds(time);
            ObjectPoolManager.Destroy(gameObject);
        }
    }
}
