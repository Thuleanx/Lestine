using UnityEngine;
using DG.Tweening;
using MathUtils;
using NaughtyAttributes;

namespace Anime {
    public class ScaleOnTrigger : EventBasedAnimation {
        [SerializeField] Ease easing;
        [SerializeField] float scaleTo;
        [SerializeField, MinMaxSlider(0, 10f)] Vector2 durationRange;
        [SerializeField, MinMaxSlider(0, 10f)] Vector2 delayRange;

        public override void Animate() {
            float duration = Mathx.RandomRange(durationRange.x, durationRange.y);
            float delay = Mathx.RandomRange(delayRange.x, delayRange.y);

            transform.DOScale(Vector3.one * scaleTo, duration).SetDelay(delay).SetEase(easing)
                .OnComplete(() => onAnimateDone?.Invoke());
        }
    }
}
