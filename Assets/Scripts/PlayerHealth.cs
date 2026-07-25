using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        // Initialize the UI health bar at start
        if (UIManager.instance != null)
            UIManager.instance.UpdateHealth(currentHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        // Update the red slider
        if (UIManager.instance != null)
            UIManager.instance.UpdateHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Killed!");
        // Use TogglePause to show the Game Over / Pause panel
        if (UIManager.instance != null)
            UIManager.instance.TogglePause();
    }
}