using UnityEngine;
using System.Collections.Generic;

public class SabaEntity : MonoBehaviour {
    public SabaAttributes Stats;
    public SabaResource Resource;

    void Awake() {
        Resource.Health = Stats.MaxHealth;
    }

    public static void Kill(IEnumerable<SabaEntity> entities) {
        foreach (SabaEntity entity in entities)
            Destroy(entity.gameObject);
    }
}
