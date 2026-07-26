using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Countdown
{
    public class CharacterMelee2D : MonoBehaviour
    {
        [Header("Timing")]
        [SerializeField] private float attackDuration = 0.2f;

        [Header("References")]
        [SerializeField] private Animator meleeAnimator;
        [SerializeField] private GameObject meleeHitbox;

        [Header("Animation")]
        [SerializeField] private string attackTrigger = "Attack";

        private InputAction _attackAction;
        private bool _isAttacking;

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
                StartCoroutine(SwingRoutine());
        }

        private IEnumerator SwingRoutine()
        {
            _isAttacking = true;

            if (meleeHitbox != null)
                meleeHitbox.SetActive(true);

            if (meleeAnimator != null)
                meleeAnimator.SetTrigger(attackTrigger);

            yield return new WaitForSeconds(attackDuration);

            if (meleeHitbox != null)
                meleeHitbox.SetActive(false);

            _isAttacking = false;
        }
    }
}
