using DG.Tweening;
using UnityEngine;

namespace Countdown
{
    public class HitEffect2D : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [SerializeField] private float duration = 0.3f;
        [SerializeField] private float startScale = 0.5f;
        [SerializeField] private float endScale = 1.5f;
        [SerializeField] private Ease easeType = Ease.OutQuad;

        private Tween _tween;

        private void OnEnable()
        {
            transform.localScale = Vector3.one * startScale;

            var seq = DOTween.Sequence();
            seq.Append(transform.DOScale(endScale, duration).SetEase(easeType));
            seq.Join(spriteRenderer.DOFade(0f, duration));

            _tween = seq.OnComplete(() => Destroy(gameObject));
        }

        private void OnDestroy()
        {
            _tween?.Kill();
        }
    }
}
