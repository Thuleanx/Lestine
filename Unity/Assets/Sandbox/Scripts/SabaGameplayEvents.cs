using UnityEngine;

namespace Saba {
    public class SabaGameplayEvents {
        // This event might be processed after the target has already died
        public struct Kill {
            public Vector2 location;
        }
    }
}
