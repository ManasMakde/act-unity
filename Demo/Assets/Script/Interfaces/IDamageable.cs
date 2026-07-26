using System;

public interface IDamageable
{
    event Action OnKilled;
    void TakeDamage(float amount);
}
