using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

namespace eclipse {
    [RequireComponent(typeof(Entity))]
	public class EntityDebugger : MonoBehaviour {
        [SerializeField, ReadOnly]
        float health;

        Entity entity;

        void Awake() {
            entity = GetComponent<Entity>();
        }

        void Update() {
            health = Alias.health[entity.resource];
        }
	}
}
