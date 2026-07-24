using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 15f;

    [Header("Aiming Settings")]
    [SerializeField] private float aimDeadZone = 0.5f;
    [SerializeField] private float aimDamping = 15f;

    [Header("Level Bounds")]
    [SerializeField] private Transform levelPlane;
    [Tooltip("Stops player slightly before reaching the visual edge of the plane")]
    [SerializeField] private float edgeBuffer = 0.5f;

    private CharacterController _controller;
    private InputAction _moveAction;
    private Camera _mainCamera;
    private float _minX, _maxX, _minZ, _maxZ;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        // assign input actions
        _moveAction = InputSystem.actions.FindAction("Move");

        CalculateBounds();
    }

    private void Update()
    {
        HandleMovement();
        HandleAim();
    }

    private void HandleMovement()
    {
        if (_moveAction == null) return;

        //Read movement from input system value and apply through character controller
        Vector2 input = _moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(input.x, 0f, input.y);
        _controller.Move(moveDirection * speed * Time.deltaTime);

        //Keep player inside level bounds
        ApplyClamping();
    }

    private void HandleAim()
    {
        if (_mainCamera == null) return;

        //Convert mouse screen position to a world ray
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = _mainCamera.ScreenPointToRay(mousePos);

        //Create a plane at player pos
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        //find where the ray from cam hits the flat floor
        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);
            Vector3 targetDir = worldPoint - transform.position;
            targetDir.y = 0; // rotate y only

            //Only rotate if mouse is beyond the threshold distance
            if (targetDir.sqrMagnitude > (aimDeadZone * aimDeadZone))
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDir);

                //slerp it up: Smoothly rotate toward the cursor over time
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime * aimDamping
                );
            }
        }
    }

    private void ApplyClamping()
    {
        if (levelPlane == null) return;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, _minX, _maxX);
        pos.z = Mathf.Clamp(pos.z, _minZ, _maxZ);
        transform.position = pos;
    }

    private void CalculateBounds()
    {
        if (levelPlane == null)
        {
            Debug.LogWarning("PlayerMovement: Level Plane is not assigned. Clamping is disabled.");
            return;
        }

        // A default Unity Plane primitive scaled 1:1 is 10x10 units wide
        float halfWidth = (levelPlane.localScale.x * 10f) / 2f;
        float halfDepth = (levelPlane.localScale.z * 10f) / 2f;

        _minX = levelPlane.position.x - halfWidth + edgeBuffer;
        _maxX = levelPlane.position.x + halfWidth - edgeBuffer;
        _minZ = levelPlane.position.z - halfDepth + edgeBuffer;
        _maxZ = levelPlane.position.z + halfDepth - edgeBuffer;
    }
}