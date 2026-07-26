using System;


[Serializable]
public class HealthSystem
{
    // Public Actions
    public event Action<float /* oldHealth */, float /* newHealth */> OnHealthChange;


    // Public Properties
    public float maxHealth { get; private set; } = 100.0f; 
    public float currentHealth { get; private set; } = 0.0f;


    // Constructor
    public HealthSystem()
    {
        currentHealth = maxHealth;
    }


    // Public Methods
    public void ReduceHealth(float amount)
    {
        // Reduce health
        float oldHealth = currentHealth;
        currentHealth = Math.Max(currentHealth - amount, 0.0f);


        // Broadcast health changed
        OnHealthChange?.Invoke(oldHealth, currentHealth);
    }
    public void IncreaseHealth(float amount)
    {
        // Increase health
        float oldHealth = currentHealth;
        currentHealth = Math.Min(currentHealth + amount, maxHealth);


        // Broadcast health changed
        OnHealthChange?.Invoke(oldHealth, currentHealth);
    }
}
