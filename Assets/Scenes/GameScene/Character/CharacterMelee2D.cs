using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Countdown
{
    public class CharacterMelee2D : MonoBehaviour
    {
        [Header("Swing Timing & Motion")]
        [SerializeField] private float attackDuration = 0.2f;
        [SerializeField] private float startAngle = -90f;
        [SerializeField] private float endAngle = 90f;
        [SerializeField] private Ease easeType = Ease.OutBack;

        [Header("References")]
        [SerializeField] private Transform swordPivot;
        [SerializeField] private GameObject meleeHitbox;
        [SerializeField] private SpriteRenderer staffRenderer;
        [SerializeField] private Animator meleeAnimator;

        [Header("Animation")]
        [SerializeField] private string attackTrigger = "Attack";

        private InputAction _attackAction;
        private bool _isAttacking;
        private Tween _swingTween;

        private void Start()
        {
            _attackAction = InputSystem.actions.FindAction("Attack");

            if (meleeHitbox != null)
                meleeHitbox.SetActive(false);
        }

        private void Update()
        {
            if (_attackAction == null) return;

            if (_attackAction.triggered && !_isAttacking)
                Swing();
        }

        private void Swing()
        {
            _isAttacking = true;

            if (meleeHitbox != null)
                meleeHitbox.SetActive(true);
            
            GameCamera.Shake(.3f, .5f);

            if (swordPivot != null)
            {
                bool isFlipped = staffRenderer != null && staffRenderer.flipY;

                float currentStartAngle = isFlipped ? startAngle : -startAngle;
                float currentEndAngle = isFlipped ? endAngle : -endAngle;

                float baseZ = swordPivot.localEulerAngles.z;
                swordPivot.localRotation = Quaternion.Euler(0f, 0f, baseZ + currentStartAngle);

                _swingTween?.Kill();
                _swingTween = swordPivot
                    .DOLocalRotate(new Vector3(0f, 0f, baseZ + currentEndAngle), attackDuration)
                    .SetEase(easeType)
                    .OnComplete(OnSwingComplete);
            }
        }

        private void OnSwingComplete()
        {
            if (meleeHitbox != null)
                meleeHitbox.SetActive(false);

            _isAttacking = false;
        }

        private void OnDestroy()
        {
            _swingTween?.Kill();
        }
    }
}
