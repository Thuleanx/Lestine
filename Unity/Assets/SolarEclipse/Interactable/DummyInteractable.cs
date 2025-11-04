using UnityEngine;

using NaughtyAttributes;

namespace eclipse.interactable {
    public class DummyInteractable : Interactable {
        [SerializeField, ShowAssetPreview]
        Sprite sprite;

        public override string GetInteractionPrompt() => "Dummy";
        public override Sprite GetInteractionSprite() => sprite;
    }
}
