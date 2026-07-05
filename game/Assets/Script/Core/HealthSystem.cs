using System;
using UnityEngine;

public class HealthSystem
{
    // Public Actions
    public event Action<float /* oldHealth */, float /* newHealth */> OnHealthChange;


    // Public Properties
    public float MaxHealth { get; private set; } = 100.0f; 
    public float CurrentHealth { get; private set; } = 0.0f;


    // Constructor
    public HealthSystem()
    {
        CurrentHealth = MaxHealth;
    }


    // Public Methods
    public void ReduceHealth(float amount)
    {
        // Reduce health
        float oldHealth = CurrentHealth;
        CurrentHealth = Math.Max(CurrentHealth - amount, 0.0f);


        // Broadcast health changed
        OnHealthChange?.Invoke(oldHealth, CurrentHealth);
    }
    public void IncreaseHealth(float amount)
    {
        // Increase health
        float oldHealth = CurrentHealth;
        CurrentHealth = Math.Min(CurrentHealth + amount, MaxHealth);


        // Broadcast health changed
        OnHealthChange?.Invoke(oldHealth, CurrentHealth);
    }
}
