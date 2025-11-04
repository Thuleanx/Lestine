using UnityEngine;
using UnityEngine.UI;

using ADammy;

namespace eclipse.ui {
    public struct FocusInteractableDrop : IEvent {};

    public struct FocusInteractableChange : IEvent {
        public Sprite sprite;
        public string prompt;
        public Vector3 location;
    };
}
