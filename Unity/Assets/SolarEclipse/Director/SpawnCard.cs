using UnityEngine;

using NaughtyAttributes;
using Stats;

namespace eclipse.director {
[System.Serializable]
public struct SpawnCard {
    [ShowAssetPreview]
    public Entity prefab;

    [Min(0.0f)]
    public float weight;

    [Min(0.0f)]
    public float cost;

    public SingleCoreStatsEntry coreStats;
}
}
