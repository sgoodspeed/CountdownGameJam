using UnityEngine;

namespace Countdown
{
    public interface IDamageable
    {
        void TakeDamage(float amount, Vector2 hitDirection, float knockbackDistance = 0f);
    }
}
