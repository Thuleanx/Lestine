using UnityEngine;

using NaughtyAttributes;

namespace Saba {
	[CreateAssetMenu(menuName = "Saba/ItemDefinition")]
	public class SabaItemDefinition : ScriptableObject {
        public string displayName;
        [ShowAssetPreview]
        public Sprite icon;
        public SabaBuffData[] buffsToApply;
        public SabaBuffData[] buffsOnKill;
    }
}
