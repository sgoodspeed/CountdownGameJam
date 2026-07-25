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
        [Tooltip("The pivot object that will rotate during the attack arc.")]
        [SerializeField] private Transform swordPivot;
        [Tooltip("The actual visual object inside the pivot.")]
        [SerializeField] private GameObject swordVisual;

        private InputAction _attackAction;
        private bool _isAttacking;
        private Tween _swingTween;

        private void Start()
        {
            //reference the input action for attacking
            _attackAction = InputSystem.actions.FindAction("Attack");

            // Ensure sword visual starts inactive
            if (swordVisual != null)
            {
                swordVisual.SetActive(false);
            }
        }

        private void Update()
        {
            if (_attackAction == null) return;

            //trigger attack on input if not currently mid-attack
            if (_attackAction.triggered && !_isAttacking)
            {
                Swing();
            }
        }

        private void Swing()
        {
            _isAttacking = true;

            if (swordVisual != null)
            {
                swordVisual.SetActive(true);
            }

            if (swordPivot != null)
            {
                // Negated: rotating around Z (2D, facing the camera) reads in the
                // opposite visual direction from rotating around Y (3D top-down)
                // for the same signed angle, so flip it to match the intended swing.
                swordPivot.localRotation = Quaternion.Euler(0f, 0f, -startAngle);

                _swingTween?.Kill();
                _swingTween = swordPivot
                    .DOLocalRotate(new Vector3(0f, 0f, -endAngle), attackDuration)
                    .SetEase(easeType)
                    .OnComplete(OnSwingComplete);
            }
            else
            {
                OnSwingComplete();
            }
        }

        private void OnSwingComplete()
        {
            // Hide visual after swing completes
            if (swordVisual != null)
            {
                swordVisual.SetActive(false);
            }

            _isAttacking = false;
        }

        private void OnDestroy()
        {
            _swingTween?.Kill();
        }
    }
}
