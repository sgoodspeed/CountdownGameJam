using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Countdown
{
    /// <summary>
    /// 2D counterpart to EnemySpawner. There is no NavMesh in the 2D scene, so spawn
    /// point validity is checked against the circular level boundary instead.
    /// </summary>
    public class EnemySpawner2D : MonoBehaviour
    {
        public static EnemySpawner2D Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private Transform playerTransform;
        [Tooltip("Optional - spawn points outside this boundary are skipped.")]
        [SerializeField] private CircleBoundary levelBoundary;

        [Header("Spawn Settings")]
        [SerializeField] private int enemyCap = 10;
        [SerializeField] private float spawnRadius = 8f;
        [SerializeField] private float spawnInterval = 2f;

        [Header("Flying Enemy")]
        [SerializeField] private GameObject flyingEnemyPrefab;
        [Tooltip("Chance (0-1) that a spawn produces a flying enemy.")]
        [SerializeField] private float flyingEnemyChance = 0.15f;
        [Tooltip("How far beyond the boundary edge the flying enemy spawns/exits.")]
        [SerializeField] private float flyingSpawnOffset = 1f;

        private int _currentEnemyCount = 0;
        private int _lastAngleIndex = -1;
        private Coroutine _spawnCoroutine;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (playerTransform == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null) playerTransform = playerObj.transform;
            }

            if (levelBoundary == null)
            {
                levelBoundary = GameState.Instance.Boundary;
            }
        }

        public void StartSpawning()
        {
            if (_spawnCoroutine != null) return;
            _spawnCoroutine = StartCoroutine(DripSpawnRoutine());
        }

        public void StopSpawning()
        {
            if (_spawnCoroutine == null) return;
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }

        public void EnemyDied()
        {
            _currentEnemyCount = Mathf.Max(0, _currentEnemyCount - 1);
        }

        private IEnumerator DripSpawnRoutine()
        {
            while (true)
            {
                if (_currentEnemyCount < enemyCap && playerTransform != null)
                {
                    ExecuteSingleSpawn();
                }

                yield return new WaitForSeconds(spawnInterval);
            }
        }

        public void ExecuteSingleSpawn()
        {
            if (flyingEnemyPrefab != null && Random.value < flyingEnemyChance)
            {
                SpawnFlyingEnemy();
                return;
            }

            int nextIndex;
            do
            {
                nextIndex = Random.Range(0, 8);
            }
            while (
                _lastAngleIndex != -1 && (
                nextIndex == _lastAngleIndex ||
                nextIndex == (_lastAngleIndex + 1) % 8 ||
                nextIndex == (_lastAngleIndex + 7) % 8)
            );

            _lastAngleIndex = nextIndex;

            float angle = _lastAngleIndex * (360f / 8f);

            Vector2 targetPos = (Vector2)playerTransform.position + CalculateRadialOffset(angle, spawnRadius);
            InstantiateEnemy(targetPos);
        }

        private void SpawnFlyingEnemy()
        {
            Vector2 center = levelBoundary != null
                ? (Vector2)levelBoundary.transform.position
                : Vector2.zero;
            float radius = levelBoundary != null ? levelBoundary.radius : 8f;
            float edgeDist = radius + flyingSpawnOffset;

            float spawnAngle = Random.Range(0f, 360f);
            float exitAngle = spawnAngle + 180f + Random.Range(-30f, 30f);

            Vector2 spawnPos = center + CalculateRadialOffset(spawnAngle, edgeDist);
            Vector2 destPos = center + CalculateRadialOffset(exitAngle, edgeDist);

            GameObject newEnemy = Instantiate(flyingEnemyPrefab, spawnPos, Quaternion.identity);
            _currentEnemyCount++;

            if (newEnemy.TryGetComponent(out FlyingEnemy2D flyingEnemy))
            {
                flyingEnemy.target = playerTransform;
                flyingEnemy.SetDestination(destPos);
            }
        }

        private Vector2 CalculateRadialOffset(float angleInDegrees, float radius)
        {
            float radians = angleInDegrees * Mathf.Deg2Rad;
            float x = Mathf.Cos(radians) * radius;
            float y = Mathf.Sin(radians) * radius;
            return new Vector2(x, y);
        }

        public void SpawnNear(Vector2 center, int count, float scatter = 1.5f)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 offset = Random.insideUnitCircle * scatter;
                InstantiateEnemy(center + offset);
            }
        }

        private void InstantiateEnemy(Vector2 position)
        {
            if (enemyPrefab == null) return;

            GameObject newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
            _currentEnemyCount++;

            if (newEnemy.TryGetComponent(out EnemyBase2D enemyScript))
            {
                enemyScript.target = playerTransform;
                enemyScript.currentState = EnemyBase2D.AIState.Chasing;
            }
        }
    }
}
