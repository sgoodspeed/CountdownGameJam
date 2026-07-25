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
        [Tooltip("Layers considered as obstacles by the movement's avoidance cast.")]
        [SerializeField] private LayerMask movementCastLayers = ~0;

        [Header("Aiming")]
        [SerializeField] private float aimDeadZone = 0.5f;
        [SerializeField] private float aimDamping = 15f;

        [Header("Visual & Aim References")]
        [SerializeField] private Transform bodyVisual;       // Drag 'BodyVisual' here
        [SerializeField] private Transform staffPivot;       // Drag 'StaffPivot' here
        [SerializeField] private SpriteRenderer staffRenderer;// Drag 'StaffArm' SpriteRenderer here

        private InputAction _moveAction;
        private Camera _mainCamera;

        private readonly RaycastHit2D[] hits = new RaycastHit2D[10];
        private ContactFilter2D movementCastFilter;

        private void Awake()
        {
            body.freezeRotation = true;
            _mainCamera = Camera.main;

            movementCastFilter = ContactFilter2D.noFilter;
            movementCastFilter.SetLayerMask(movementCastLayers);
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

            var distance = moveSpeed * Time.fixedDeltaTime;
            var direction = input.normalized * (distance);
            var hitCount = collision.Cast(direction, movementCastFilter, hits, direction.magnitude + Mathf.Epsilon);
            for (var i = 0; i < hitCount; i++)
            {
                distance = Mathf.Min(distance, hits[i].distance - skinDistance);
            }

            body.MovePosition(body.position + direction * distance);
        }

        private void HandleAim()
        {
            if (_mainCamera == null || Mouse.current == null || staffPivot == null) return;

            // 1. Get mouse world position on character plane
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 screenDepth = _mainCamera.WorldToScreenPoint(transform.position);
            Vector3 worldPoint = _mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, screenDepth.z));

            // 2. Rotate StaffPivot toward mouse (+180 offset for staff art orientation)
            Vector2 targetDir = ((Vector2)worldPoint - (Vector2)staffPivot.position).normalized;
            if (targetDir.sqrMagnitude > (aimDeadZone * aimDeadZone))
            {
                float targetAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg + 180f;
                staffPivot.rotation = Quaternion.Euler(0f, 0f, targetAngle);
            }

            // 3. Determine if mouse is to the left of the player character
            bool isMouseToLeft = worldPoint.x > transform.position.x;

            // 4. Flip body visual horizontally based on mouse direction
            if (bodyVisual != null)
            {
                Vector3 scale = bodyVisual.localScale;
                scale.x = isMouseToLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
                bodyVisual.localScale = scale;
            }

            // 5. Mirror StaffPivot's local X position so the shoulder socket flips sides
            Vector3 pivotPos = staffPivot.localPosition;
            pivotPos.x = isMouseToLeft ? Mathf.Abs(pivotPos.x) : -Mathf.Abs(pivotPos.x);
            staffPivot.localPosition = pivotPos;

            // 6. Flip staff sprite vertically when aiming left so it stays upright
            if (staffRenderer != null)
            {
                staffRenderer.flipY = isMouseToLeft;
            }
        }
    }
}
