using UnityEngine;
using Optimization;

namespace Behaviours {
    public class Spawner : MonoBehaviour {
        [SerializeField] GameObject objectToSpawn;

        public void Spawn() {
            if (ObjectPoolManager.Instance) {
                ObjectPoolManager.Instance.Borrow(gameObject.scene, objectToSpawn.transform, transform.position, transform.rotation);
            } else {
                Instantiate(objectToSpawn, transform.position, transform.rotation);
            }
        }
    }
}
