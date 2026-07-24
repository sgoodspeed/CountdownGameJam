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
        [SerializeField] private Collider2D collision;
        [SerializeField] private float skinDistance = 0.1f;
        
        [SerializeField] private float moveSpeed = 5f;
        
        private readonly RaycastHit2D[] hits = new RaycastHit2D[10];

        private void Awake()
        {
            body.freezeRotation = true;
        }

        private void FixedUpdate()
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (input.sqrMagnitude > 1f) { input.Normalize(); }

            var distance = moveSpeed * Time.deltaTime;
            var direction = input.normalized * (distance);
            var hitCount = collision.Cast(direction, hits, direction.magnitude + Mathf.Epsilon);
            for (var i = 0; i < hitCount; i++) {
                distance = Mathf.Min(distance, hits[i].distance - skinDistance);
            }
            
            body.MovePosition(body.position + direction * distance);
        }
    }
}
