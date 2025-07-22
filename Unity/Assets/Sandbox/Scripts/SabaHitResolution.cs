using UnityEngine;
using System.Collections.Generic;

using PrettyPatterns;

public class SabaHitResolution : Singleton<SabaHitResolution> {
    const int MAX_HIT = 100000;

    public struct Hit {
        public SabaEntity Entity;
        public Vector3 Location;
        public float Damage;
    }

    int NumUnresolvedHits = 0;
    Hit[] UnresolvedHits = new Hit[MAX_HIT];

    public void RegisterHits(IEnumerable<Hit> hits) {
        foreach (Hit hit in hits)
            UnresolvedHits[NumUnresolvedHits++] = hit;
    }

    void Resolve() {
        List<SabaEntity> deadEntities = new List<SabaEntity>();
        for (int i = 0; i < NumUnresolvedHits; i++) {
            Hit hit = UnresolvedHits[i];

            hit.Entity.Resource.Health -= hit.Damage;
            if (hit.Entity.Resource.Health < 0)
                deadEntities.Add(hit.Entity);
        }

        SabaEntity.Kill(deadEntities);

        NumUnresolvedHits = 0;
    }

    public void LateUpdate() {
        Resolve();
    }
}
