using UnityEngine;
using UnityEngine.InputSystem;

namespace Countdown
{
    public class CharacterRanged2D : MonoBehaviour
    {
        [Header("Firing")]
        [SerializeField] private float fireRate = 5f;
        [SerializeField] private float spawnOffset = 0.5f;
        [SerializeField] private float spawnJitter = 0.5f;
        [SerializeField] private Transform staffArm;
        
        
        [Tooltip("Name of the InputSystem action that triggers firing.")]
        [SerializeField] private string inputActionName = "Shoot";

        [Header("References")]
        [SerializeField] private ProjectilePool projectilePool;

        private InputAction _shootAction;
        private float _nextFireTime;

        private void Start()
        {
            _shootAction = InputSystem.actions.FindAction(inputActionName);
        }

        private void Update()
        {
            if (_shootAction == null || projectilePool == null) return;

            if (_shootAction.ReadValue<float>() > 0f && Time.time >= _nextFireTime)
            {
                Fire();
                _nextFireTime = Time.time + 1f / fireRate;
            }
        }

        private void Fire()
        {
            GameCamera.Shake(.1f, .3f);
            
            Vector2 direction = -staffArm.right;
            Vector2 spawnPos = (Vector2)transform.position + direction * spawnOffset;
            Vector2 jitter = staffArm.up * (Random.Range(-1f, 1f) * spawnJitter);

            var projectile = projectilePool.Get();
            projectile.Fire(spawnPos + jitter, direction);
        }
    }
}
