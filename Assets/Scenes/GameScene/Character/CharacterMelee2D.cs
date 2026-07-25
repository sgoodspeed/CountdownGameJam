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
        [SerializeField] private GameObject meleeHitbox;
        [Tooltip("Reference to the staff renderer to check if we are flipped.")]
        [SerializeField] private SpriteRenderer staffRenderer; // Drag StaffArm's SpriteRenderer here

        private InputAction _attackAction;
        private bool _isAttacking;
        private Tween _swingTween;

        private void Start()
        {
            //reference the input action for attacking
            _attackAction = InputSystem.actions.FindAction("Attack");

            // Ensure sword visual starts inactive
            if (meleeHitbox != null)
            {
                //meleeHitbox.SetActive(false);
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

            // Turn ON the hitbox at the start of the swing
            if (meleeHitbox != null)
            {
                meleeHitbox.SetActive(true);
            }
            _isAttacking = false;

            if (swordPivot != null)
            {
                // Check if staff sprite is currently flipped (facing right vs left)
                bool isFlipped = staffRenderer != null && staffRenderer.flipY;

                // Invert the swing arc direction when facing the opposite way
                float currentStartAngle = isFlipped ? startAngle : -startAngle;
                float currentEndAngle = isFlipped ? endAngle : -endAngle;

                // Apply starting offset relative to current z rotation
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
            // Hide visual after swing completes
            if (meleeHitbox != null)
            {
                meleeHitbox.SetActive(false);
            }

            _isAttacking = false;
        }

        private void OnDestroy()
        {
            _swingTween?.Kill();
        }
    }
}
