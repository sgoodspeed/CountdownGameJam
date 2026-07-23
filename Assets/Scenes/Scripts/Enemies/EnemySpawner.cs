using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform playerTransform;

    [Header("Spawn Settings")]
    [SerializeField] private int enemyCap = 10;
    [SerializeField] private float spawnRadius = 15f;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float navMeshCheckDistance = 2f;

    private int _currentEnemyCount = 0;
    private int _lastAngleIndex = -1;

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

        StartCoroutine(DripSpawnRoutine());
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

            // Add slight variance to spawn timing so it feels natural
            float nextWait = spawnInterval + Random.Range(-spawnInterval * 0.3f, spawnInterval * 0.3f);
            yield return new WaitForSeconds(Mathf.Max(0.2f, nextWait));
        }
    }

    public void ExecuteSingleSpawn()
    {
        int nextIndex;
        // Ensure we don't spawn in the exact same direction or immediately adjacent angle twice in a row
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
        float jitteredRadius = spawnRadius + Random.Range(-3f, 3f);

        Vector3 targetPos = playerTransform.position + CalculateRadialOffset(angle, jitteredRadius);

        if (TryGetValidNavMeshPoint(targetPos, out Vector3 finalPosition))
        {
            InstantiateEnemy(finalPosition);
        }
    }

    private Vector3 CalculateRadialOffset(float angleInDegrees, float radius)
    {
        float radians = angleInDegrees * Mathf.Deg2Rad;
        float x = Mathf.Cos(radians) * radius;
        float z = Mathf.Sin(radians) * radius;
        return new Vector3(x, 0f, z);
    }

    private bool TryGetValidNavMeshPoint(Vector3 center, out Vector3 result)
    {
        if (NavMesh.SamplePosition(center, out NavMeshHit hit, navMeshCheckDistance, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    private void InstantiateEnemy(Vector3 position)
    {
        if (enemyPrefab == null) return;

        GameObject newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        _currentEnemyCount++;

        if (newEnemy.TryGetComponent(out EnemyBase enemyScript))
        {
            enemyScript.target = playerTransform;
            enemyScript.currentState = EnemyBase.AIState.Chasing;
        }
    }
}