using UnityEngine;
using DG.Tweening;
using MathUtils;
using NaughtyAttributes;

namespace Anime {
    public class MoveOnTrigger : EventBasedAnimation {
        [SerializeField] Ease easing;
        [SerializeField] Vector3 offset;
        [SerializeField, MinMaxSlider(0, 10f)] Vector2 durationRange;
        [SerializeField, MinMaxSlider(0, 10f)] Vector2 delayRange;

        public override void Animate() {
            Vector3 originalPosition;
            originalPosition = transform.localPosition;
            transform.localPosition += offset;

            float duration = Mathx.RandomRange(durationRange.x, durationRange.y);
            float delay = Mathx.RandomRange(delayRange.x, delayRange.y);

            transform.DOLocalMove(originalPosition, duration).SetDelay(delay).SetEase(easing)
                .OnComplete(() => onAnimateDone?.Invoke());
        }
    }
}
