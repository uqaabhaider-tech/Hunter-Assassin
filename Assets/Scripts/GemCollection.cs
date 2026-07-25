using UnityEngine;

public class GemPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (UIManager.instance != null)
            {
                UIManager.instance.AddGem(); // Adds 1 to the UI
            }
            Destroy(gameObject);
        }
    }
}