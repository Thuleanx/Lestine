using UnityEngine;

namespace Saba {
	[CreateAssetMenu(menuName = "Saba/ItemDefinition")]
	public class SabaItemDefinition : ScriptableObject {
        public SabaBuffData[] buffsToApply;
    }
}
