using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
public abstract class EnemyBase : MonoBehaviour
{
    public enum AIState { Chasing, Guarding }

    [Header("Base Properties")]
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float pathUpdateInterval = 0.25f;

    [Header("Navigation & Targeting")]
    public AIState currentState = AIState.Chasing;
    public Transform target;

    protected float currentHealth;
    protected NavMeshAgent agent;
    protected Rigidbody rb;

    // Wobble variables to spread out pathing
    private float _wobbleSpeed;
    private float _wobbleIntensity;
    private float _wobbleOffset;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        // Ensure physics doesn't fight NavMesh navigation
        rb.isKinematic = true;

        // Give each enemy instance a unique wobble personality
        _wobbleSpeed = Random.Range(0.8f, 2.5f);
        _wobbleIntensity = Random.Range(1.0f, 3.5f);
        _wobbleOffset = Random.Range(0f, 10f);
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;

        // find player by tag if no target is assigned
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }

        StartCoroutine(UpdatePathRoutine());
    }

    private IEnumerator UpdatePathRoutine()
    {
        // Stagger pathing start slightly to avoid spikes
        yield return new WaitForSeconds(Random.Range(0f, pathUpdateInterval));

        while (true)
        {
            if (agent.enabled && agent.isOnNavMesh)
            {
                if (currentState == AIState.Chasing && target != null)
                {
                    // Calculate circular jitter offset using Sin/Cos
                    float x = Mathf.Sin(Time.time * _wobbleSpeed + _wobbleOffset) * _wobbleIntensity;
                    float z = Mathf.Cos(Time.time * _wobbleSpeed + _wobbleOffset) * _wobbleIntensity;
                    Vector3 jitter = new Vector3(x, 0f, z);

                    agent.SetDestination(target.position + jitter);
                }
            }

            yield return new WaitForSeconds(pathUpdateInterval);
        }
    }

    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // Notify spawner so it can decrement active enemy count
        if (EnemySpawner.Instance != null)
        {
            EnemySpawner.Instance.EnemyDied();
        }

        Destroy(gameObject);
    }
}