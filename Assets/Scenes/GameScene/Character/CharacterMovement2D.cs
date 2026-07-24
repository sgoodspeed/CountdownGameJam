using UnityEngine;
using UnityEngine.InputSystem;

namespace Countdown
{
    /// <summary>
    /// Basic 2D axis movement for the player character. Movement is applied as
    /// velocity on a dynamic Rigidbody2D, so Unity's physics engine stops the
    /// character against the level's EdgeCollider2D boundary instead of moving
    /// through it.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterMovement2D : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private Collider2D collision;
        [SerializeField] private float skinDistance = 0.1f;
        [SerializeField] private float moveSpeed = 5f;

        [Header("Aiming")]
        [SerializeField] private float aimDeadZone = 0.5f;
        [SerializeField] private float aimDamping = 15f;

        private InputAction _moveAction;
        private Camera _mainCamera;

        private readonly RaycastHit2D[] hits = new RaycastHit2D[10];

        private void Awake()
        {
            body.freezeRotation = true;
            _mainCamera = Camera.main;
        }

        private void Start()
        {
            // assign input actions
            _moveAction = InputSystem.actions.FindAction("Move");
        }

        private void FixedUpdate()
        {
            HandleMovement();
            HandleAim();
        }

        private void HandleMovement()
        {
            if (_moveAction == null) return;

            Vector2 input = _moveAction.ReadValue<Vector2>();
            if (input.sqrMagnitude > 1f) { input.Normalize(); }

            var distance = moveSpeed * Time.deltaTime;
            var direction = input.normalized * (distance);
            var hitCount = collision.Cast(direction, hits, direction.magnitude + Mathf.Epsilon);
            for (var i = 0; i < hitCount; i++) {
                distance = Mathf.Min(distance, hits[i].distance - skinDistance);
            }

            body.MovePosition(body.position + direction * distance);
        }

        private void HandleAim()
        {
            if (_mainCamera == null || Mouse.current == null) return;

            //Convert mouse screen position to a world point on the character's depth plane
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 screenDepth = _mainCamera.WorldToScreenPoint(transform.position);
            Vector3 worldPoint = _mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, screenDepth.z));

            Vector2 targetDir = (Vector2)worldPoint - body.position;

            //Only rotate if mouse is beyond the threshold distance
            if (targetDir.sqrMagnitude > (aimDeadZone * aimDeadZone))
            {
                float targetAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;

                //slerp it up: Smoothly rotate toward the cursor over time
                float newAngle = Mathf.LerpAngle(body.rotation, targetAngle, Time.deltaTime * aimDamping);
                body.MoveRotation(newAngle);
            }
        }
    }
}
