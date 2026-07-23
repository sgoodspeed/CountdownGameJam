using UnityEngine;

namespace Countdown
{
    /// <summary>
    /// Basic 2D axis movement for the player character. Position is clamped every
    /// physics step to stay inside the assigned CircleBoundary, so the character
    /// slides along the boundary edge instead of exiting the circle.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterMovement2D : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float characterRadius = 0.3f;

        private void Awake()
        {
            body.bodyType = RigidbodyType2D.Kinematic;
            body.gravityScale = 0f;
        }

        private void FixedUpdate()
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (input.sqrMagnitude > 1f)
            {
                input.Normalize();
            }

            Vector2 targetPosition = body.position + input * moveSpeed * Time.fixedDeltaTime;
            body.MovePosition(targetPosition);
        }
    }
}
