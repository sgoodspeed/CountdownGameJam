using UnityEngine;

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
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private float moveSpeed = 5f;

        private void Awake()
        {
            body.bodyType = RigidbodyType2D.Dynamic;
            body.gravityScale = 0f;
            body.freezeRotation = true;
        }

        private void FixedUpdate()
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (input.sqrMagnitude > 1f)
            {
                input.Normalize();
            }

            body.linearVelocity = input * moveSpeed;
        }
    }
}
