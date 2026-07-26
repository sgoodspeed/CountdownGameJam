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
    [DefaultExecutionOrder(100)]
    public class GameCamera : Singleton<GameCamera>
    {
        [SerializeField] private CinemachineCamera virtualCamera;
        [SerializeField] private CinemachinePositionComposer composer;
        [SerializeField] private Transform followTarget;
        
        [SerializeField] private HourMarkerContainer container;
        [SerializeField] private CharacterMovement2D player;

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
        
        [Header("Zoom")]
        [SerializeField] private float minOrthoSize = 10f;
        [SerializeField] private float maxOrthoSize = 13f;
        [SerializeField] private float zoomSmoothTime = 0.5f;

        [Header("Aim Offset")]
        [Tooltip("How far (world units) the aim direction pushes the camera anchor horizontally.")]
        [SerializeField] private float aimInfluenceX = 1.5f;
        [Tooltip("How far (world units) the aim direction pushes the camera anchor vertically.")]
        [SerializeField] private float aimInfluenceY = 1.5f;
        [Tooltip("Maximum world-unit distance the aim can push the anchor (independent of bounds).")]
        [SerializeField] private float aimMaxDistance = 3f;
        [Tooltip("Smoothing time for the aim offset. Higher = slower/softer camera response to aiming.")]
        [SerializeField] private float aimSmoothTime = 0.4f;

        private Transform anchor;
        private Camera _outputCamera;
        private float currentLean;
        private float leanVelocity;
        private float zoomVelocity;
        private Vector2 currentAimOffset;
        private Vector2 aimOffsetVelocity;

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
            
            this.player = FindAnyObjectByType<CharacterMovement2D>();
            this.container = FindAnyObjectByType<HourMarkerContainer>();
            _outputCamera = Camera.main;
        }

        private void Update()
        {
            if (followTarget == null) return;

            Vector2 playerOffset = (Vector2)followTarget.position - boundsCenter;

            Vector2 clampedPlayerOffset = Vector2.ClampMagnitude(playerOffset, boundsRadius);

            Vector2 aimOffset = Vector2.zero;
            if (player != null)
            {
                Vector2 aimDir = player.AimDirection.normalized;
                Vector2 targetAimOffset = new Vector2(aimDir.x * aimInfluenceX, aimDir.y * aimInfluenceY);
                currentAimOffset = Vector2.SmoothDamp(currentAimOffset, targetAimOffset, ref aimOffsetVelocity, aimSmoothTime);
                aimOffset = Vector2.ClampMagnitude(currentAimOffset, aimMaxDistance);
            }

            anchor.position = boundsCenter + clampedPlayerOffset + aimOffset;

            float targetLean = boundsRadius > 0f ? Mathf.Clamp(playerOffset.x / boundsRadius, -1f, 1f) : 0f;
            currentLean = Mathf.SmoothDamp(currentLean, targetLean, ref leanVelocity, leanSmoothTime);
            ApplyLean(currentLean);
        }

        private void LateUpdate()
        {
            if (_outputCamera == null || container == null || container.MarkerCount == 0) return;

            float activeRatio = (float)(container.HighestActiveMarker?.Hour ?? 0) / container.MarkerCount;
            float targetSize = Mathf.Lerp(minOrthoSize, maxOrthoSize, activeRatio);
            _outputCamera.orthographicSize = Mathf.SmoothDamp(
                _outputCamera.orthographicSize, targetSize, ref zoomVelocity, zoomSmoothTime);
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
