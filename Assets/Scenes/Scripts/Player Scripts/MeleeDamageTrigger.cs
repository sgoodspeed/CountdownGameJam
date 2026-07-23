using System.Collections.Generic;
using UnityEngine;

public class MeleeDamageTrigger : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f;

    // Track targets hit during the CURRENT swing to prevent hitting the same enemy every frame
    private readonly HashSet<Collider> _hitTargetsThisSwing = new HashSet<Collider>();

    private void OnEnable()
    {
        // Reset hit list whenever the sword visual becomes active at start of a swing
        _hitTargetsThisSwing.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"Sword hit: {other.name}"); // Temporary sanity check
        // Ignore if we already hit this collider during this swing
        if (_hitTargetsThisSwing.Contains(other)) return;

        // Check if hit object is an enemy
        if (other.TryGetComponent(out EnemyBase enemy))
        {
            _hitTargetsThisSwing.Add(other);
            enemy.TakeDamage(damageAmount);
        }
    }
}