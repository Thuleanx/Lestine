using UnityEngine;

using StatusEffects;

namespace Saba {
    public class SabaStatusEffect : StatusEffect<SabaEntity> {
        [SerializeField]
        SabaBuffData[] onGranted;

        public override bool IsTickable() => false;

        public override void OnGranted(SabaEntity entity, float intensity) {
            foreach (SabaBuffData buffData in onGranted)
                buffData.ApplyTo(entity);
        }

        public override void OnRemoved(SabaEntity entity, float intensity) {
            foreach (SabaBuffData buffData in onGranted)
                buffData.RemoveFrom(entity);
        }

        public override void Tick(SabaEntity entity) {}
    }
}
