using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMelee : MonoBehaviour
{
    [Header("Swing Timing & Motion")]
    [SerializeField] private float attackDuration = 0.2f;
    [SerializeField] private float startAngle = -90f;
    [SerializeField] private float endAngle = 90f;
    [SerializeField] private EasingFunction.Ease easeType = EasingFunction.Ease.EaseOutBack;

    [Header("References")]
    [Tooltip("The pivot object that will rotate during the attack arc.")]
    [SerializeField] private Transform swordPivot;
    [Tooltip("The actual visual object inside the pivot.")]
    [SerializeField] private GameObject swordVisual;

    private InputAction _attackAction;
    private bool _isAttacking;

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
            StartCoroutine(SwingRoutine());
        }
    }

    private IEnumerator SwingRoutine()
    {
        _isAttacking = true;

        if (swordVisual != null)
        {
            swordVisual.SetActive(true);
        }

        float elapsed = 0f;

        // Cache the ease function
        EasingFunction.Function easeFunc = EasingFunction.GetEasingFunction(easeType);

        while (elapsed < attackDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsed / attackDuration);

            //Y angle = progress from start to end based on easing method
            float currentY = easeFunc(startAngle, endAngle, normalizedTime);

            if (swordPivot != null)
            {
                swordPivot.localRotation = Quaternion.Euler(0f, currentY, 0f);
            }

            yield return null;
        }

        // Hide visual after swing completes
        if (swordVisual != null)
        {
            swordVisual.SetActive(false);
        }

        _isAttacking = false;
    }
}