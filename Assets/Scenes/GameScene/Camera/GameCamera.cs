using System;
using Unity.Cinemachine;
using UnityEngine;

namespace Countdown
{
    /// <summary>
    /// Drives the Cinemachine camera that follows the player in GameScene. Singleton so
    /// gameplay code can retarget it (SetFollowTarget) without holding a scene reference.
    ///
    /// The virtual camera never tracks the player directly - instead it follows an
    /// internal anchor that is clamped to a fixed circular play area, so the camera
    /// itself can't wander outside the level no matter how loose its damping/dead zone
    /// is. On top of that, the on-screen framing (Composition.ScreenPosition) is biased
    /// toward whichever side of that area the player is approaching, so the camera
    /// leans into that direction instead of just hard-clamping when it gets there.
    /// </summary>
    [DisallowMultipleComponent]
    public class GameCamera : Singleton<GameCamera>
    {
        [SerializeField] private CinemachineCamera virtualCamera;
        [SerializeField] private CinemachinePositionComposer composer;
        [SerializeField] private Transform followTarget;

        [Header("Fixed area")]
        [Tooltip("World-space center of the play area the camera is allowed to look at.")]
        [SerializeField] private Vector2 boundsCenter = Vector2.zero;
        [Tooltip("Radius of the play area. Should match the level's circular boundary.")]
        [SerializeField] private float boundsRadius = 5f;

        [Header("Lean")]
        [Tooltip("How far (in normalized screen-position units) the framing shifts when the player is at the edge of the play area.")]
        [SerializeField, Range(0f, 0.5f)] private float leanAmount = 0.12f;
        [Tooltip("Smoothing time for the lean, so it eases in/out instead of snapping.")]
        [SerializeField] private float leanSmoothTime = 0.35f;

        private Transform anchor;
        private float currentLean;
        private float leanVelocity;

        protected override void Awake()
        {
            base.Awake();

            anchor = new GameObject("GameCameraAnchor").transform;
            anchor.SetParent(transform, false);

            virtualCamera.Follow = anchor;
        }

        private void Start()
        {
            if (followTarget == null)
            {
                var character = FindFirstObjectByType<CharacterMovement2D>();
                if (character != null)
                {
                    followTarget = character.transform;
                }
            }
        }

        private void Update()
        {
            if (followTarget == null)
            {
                return;
            }

            Vector2 offsetFromCenter = (Vector2)followTarget.position - boundsCenter;
            Vector2 clampedOffset = Vector2.ClampMagnitude(offsetFromCenter, boundsRadius);
            anchor.position = boundsCenter + clampedOffset;

            float targetLean = boundsRadius > 0f ? Mathf.Clamp(offsetFromCenter.x / boundsRadius, -1f, 1f) : 0f;
            currentLean = Mathf.SmoothDamp(currentLean, targetLean, ref leanVelocity, leanSmoothTime);

            ApplyLean(currentLean);
        }

        private void ApplyLean(float lean)
        {
            if (composer == null)
            {
                return;
            }

            ScreenComposerSettings composition = composer.Composition;
            composition.ScreenPosition = new Vector2(Mathf.Clamp(-lean * leanAmount, -0.5f, 0.5f), composition.ScreenPosition.y);
            composer.Composition = composition;
        }
    }
}
