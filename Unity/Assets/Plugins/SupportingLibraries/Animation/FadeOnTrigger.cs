using UnityEngine;
using DG.Tweening;
using MathUtils;
using NaughtyAttributes;

namespace Anime {
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeOnTrigger : EventBasedAnimation {
        [SerializeField] bool shouldFadeIn;
        [SerializeField] Ease easing = Ease.Linear;
        [SerializeField, MinMaxSlider(0, 10f)] Vector2 durationRange;
        [SerializeField, MinMaxSlider(0, 10f)] Vector2 delayRange;

        CanvasGroup canvasGroup;

        public override void Awake() {
            base.Awake();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public override void Animate() {
            float duration = Mathx.RandomRange(durationRange.x, durationRange.y);
            float delay = Mathx.RandomRange(delayRange.x, delayRange.y);

            canvasGroup.alpha = shouldFadeIn ? 0.0f : 1.0f;
            canvasGroup.DOFade(shouldFadeIn ? 1.0f : 0.0f, duration).SetDelay(delay).SetEase(easing).OnComplete(() => onAnimateDone?.Invoke());
        }
    }
}
