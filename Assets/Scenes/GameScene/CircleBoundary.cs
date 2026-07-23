using UnityEngine;

namespace Countdown
{
    /// <summary>
    /// Circular play-area boundary. Builds its own visual at runtime and exposes
    /// ClampInside so other objects (e.g. the player character) can be kept within its radius.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EdgeCollider2D))]
    public class CircleBoundary : MonoBehaviour
    {
        [Tooltip("The radius of the circular boundary.")]
        public float radius = 5f;

        [Tooltip("Higher numbers mean a smoother circle. 30-60 is usually perfect.")]
        public int pointsCount = 50;

        void Start()
        {
            CreateCirclePoints();
        }

        // Automatically updates the visual ring in the Unity Editor scene view
        void OnValidate()
        {
            CreateCirclePoints();
        }

        void CreateCirclePoints()
        {
            EdgeCollider2D edgeCollider = GetComponent<EdgeCollider2D>();
            Vector2[] points = new Vector2[pointsCount + 1];

            for (int i = 0; i <= pointsCount; i++)
            {
                // Calculate the angle for each point around the circle
                float angle = (i / (float)pointsCount) * 2f * Mathf.PI;
            
                // Convert polar coordinates to Cartesian X and Y
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;

                points[i] = new Vector2(x, y);
            }

            // Apply the loop points to the Edge Collider
            edgeCollider.points = points;
        }
    }
}
