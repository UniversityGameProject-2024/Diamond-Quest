using UnityEngine;

public class Blade : MonoBehaviour
{
    private Collider bladeColider;

    // Reference to the main camera to convert screen positions to world positions
    private Camera mainCamera;

    private bool slicing;// Indicates whether the blade is currently slicing

    //private TrailRenderer bladeTrail;
    [Tooltip("Defines the force applied to fruits when they are sliced.")]
    [SerializeField]
    private float sliceForce = 5f;
    public float SliceForce // sliceForce is private
    {
        get { return sliceForce; }
    }
    public Vector3 direction { get; private set; }// The direction of the blade's movement

    // The minimum velocity required to enable the blade's collider
    [Tooltip("The minimum velocity required to enable the blade's collider")]
    [SerializeField]
    private float minSpliceVelocity = 0.01f;

    private void Awake()
    {
        // Initialize references to the blade's collider and the main camera
        bladeColider = GetComponent<Collider>();
        mainCamera = Camera.main;
        //bladeTrail = GetComponentInChildren<TrailRenderer>();
    }

    private void OnEnable()
    {
        // Ensure slicing is stopped when the blade is enabled
        StopSlicing();
    }

    private void OnDisable()
    {
        // Ensure slicing is stopped when the blade is disabled
        StopSlicing();
    }

    private void Update()
    {
        // Handle input for starting, stopping, or continuing slicing
        if (Input.GetMouseButtonDown(0))
        {
            StartSlicing();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopSlicing();
        }
        else if (slicing)
        {
            ContinueSlicing();
        }
    }

    private void StartSlicing()
    {
        // Convert mouse position to world position and set the blade's initial position
        Vector3 newPosition = mainCamera.ScreenToViewportPoint(Input.mousePosition);
        newPosition.z = 0f;// Ensure the blade stays on a fixed plane(2D)
        transform.position = newPosition;// Move the blade to the new position

        slicing = true;// Set slicing to active
        bladeColider.enabled = true;// Enable the blade's collider for slicing detection
        //bladeTrail.enabled = true;
        //bladeTrail.Clear();//remove blade trail
    }

    private void StopSlicing()
    {
        // Stop slicing and disable the blade's collider
        slicing = false;
        bladeColider.enabled = false;
        //bladeTrail.enabled = false;
    }

    private void ContinueSlicing()
    {
        // Convert mouse position to world position and calculate slicing motion
        Vector3 newPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        newPosition.z = 0f;// Ensure the blade stays on a fixed plane(2D)

        // Calculate the direction of movement
        direction = newPosition - transform.position;

        /* 
        Calculate the velocity of the blade's movement
        The magnitude (length) of the direction vector, which represents
        the distance the blade moved between frames.
        */
        float velocity = direction.magnitude / Time.deltaTime;

        // Enable the blade's collider only if the velocity exceeds the minimum required for slicing
        bladeColider.enabled = velocity > minSpliceVelocity;

        transform.position = newPosition;// Update the blade's position to the new position

    }
}
