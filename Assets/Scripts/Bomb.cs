using UnityEngine;

public class Bomb : MonoBehaviour
{
    // This method is called when player slice the Bomb object
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the object tagged as "Player"
        if (other.CompareTag("Player"))
        {
            // Find the ScoreManager in the scene and call its Explode() method
            FindFirstObjectByType<ScoreManager>().Explode();
        }
    }
}
