using UnityEngine;

public class Fruit : MonoBehaviour
{
    // References to the whole and sliced fruit GameObjects
    [SerializeField]
    [Tooltip("Whole fruit image")]
    private GameObject whole;
    [SerializeField]
    [Tooltip("Sliced fruit image")]
    private GameObject sliced;

    // References to the Rigidbody and Collider components of the fruit
    private Rigidbody fruitRigidbody;
    private Collider fruitCollider;

    private void Awake()
    {
        // Cache references to the Rigidbody and Collider components for optimization
        fruitRigidbody = GetComponent<Rigidbody>();
        fruitCollider = GetComponent<Collider>();
    }
    // Method to slice the fruit when hit by the blade
    private void Slice(Vector3 direction, Vector3 position, float force)
    {
        // Increment the player's score through the ScoreManager
        FindFirstObjectByType<ScoreManager>().IncreasingScore();

        // Disable the whole fruit and enable the sliced version
        whole.SetActive(false);
        sliced.SetActive(true);

        // Disable the fruit's collider to prevent further interactions
        fruitCollider.enabled = false;

        // Calculate the angle of the slice based on the blade's direction and set the sliced rotation
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        sliced.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Apply physics to each piece of the sliced fruit
        Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody slice in slices)
        {
            // Match the sliced pieces' velocity to the original fruit's velocity
            slice.linearVelocity = fruitRigidbody.linearVelocity;
            // Apply an impulse force at the slice position to simulate realistic slicing physics
            slice.AddForceAtPosition(direction * force, position, ForceMode.Impulse);
        }
    }

    // Trigger event when the fruit interacts with a blade
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger has the "Player" tag
        if (other.CompareTag("Player"))
        {
            // Retrieve the Blade component from the colliding object
            Blade blade = other.GetComponent<Blade>();
            // Call the Slice method, passing the blade's direction, position, and slice force
            Slice(blade.direction, blade.transform.position, blade.SliceForce);
        }
    }
}
