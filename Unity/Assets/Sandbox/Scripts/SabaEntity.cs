using UnityEngine;
using System.Collections.Generic;

using NaughtyAttributes;

public class SabaEntity : MonoBehaviour {
    public SabaAttributes Attributes;
    [ReadOnly]
    public SabaResource Resource;

    void Awake() {
        Resource.Health = Attributes.MaxHealth;
    }

    public static void Kill(IEnumerable<SabaEntity> entities) {
        foreach (SabaEntity entity in entities)
            Destroy(entity.gameObject);
    }
}
