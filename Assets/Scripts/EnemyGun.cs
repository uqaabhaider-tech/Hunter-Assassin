using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    // This function MUST be named 'Shoot' and take 'AssassinController' 
    // to satisfy the GuardAI script
    public void Shoot(AssassinController player)
    {
        // Try to find health on the player object OR its children
        PlayerHealth health = player.GetComponentInChildren<PlayerHealth>();

        if (health != null)
        {
            health.TakeDamage(10f);
            Debug.Log("Player Hit!");
        }
        else
        {
            // This will tell you exactly why the enemy isn't killing you
            Debug.LogError("Enemy hit the player, but NO PlayerHealth script was found!");
        }
    }
}