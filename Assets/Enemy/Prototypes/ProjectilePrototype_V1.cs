using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePrototype_V1 : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Speed at which the projectile moves")]
    [SerializeField] private float speed = 1.0f;
    [Tooltip("Direction the projectile moves")]
    [SerializeField] private Vector2 direction = Vector2.down;

    private bool isInitialized = true;

    // Update is called once per frame
    private void Update()
    {
        // Game logic that runs every frame
        if (!isInitialized) return;

        Vector3 dir = new Vector3 { x = direction.x, y = direction.y };
        transform.position += dir.normalized * speed * Time.deltaTime;
    }
}


// /// <summary>
// /// [Brief description of the script's purpose]
// /// </summary>
// public class ScriptTemplate : MonoBehaviour
// {
//     // Serialized Fields - visible in Unity Inspector
//     [Header("Configuration")]
//     [Tooltip("Description of this variable")]
//     [SerializeField] private float exampleFloat = 0f;

//     [Tooltip("Reference to another game object or component")]
//     [SerializeField] private GameObject targetObject;

//     [Tooltip("Optional component reference")]
//     [SerializeField] private Rigidbody optionalRigidbody;

//     // Private variables
//     private bool isInitialized = false;

//     // Public Properties
//     public bool IsActive { get; private set; }

//     // Awake is called when the script instance is being loaded
//     private void Awake()
//     {
//         // Initial setup that doesn't depend on other objects being initialized
//         InitializeComponents();
//     }

//     // Start is called before the first frame update
//     private void Start()
//     {
//         // Setup that can depend on other objects
//         SetupReferences();
//     }

//     // Update is called once per frame
//     private void Update()
//     {
//         // Game logic that runs every frame
//         if (!isInitialized) return;

//         // Example of frame-based logic
//         HandleRegularUpdates();
//     }

//     // Physics updates - called at a fixed interval
//     private void FixedUpdate()
//     {
//         // Physics-based calculations and updates
//         if (!isInitialized) return;

//         // Example of physics-based movement or calculations
//         PerformPhysicsCalculations();
//     }

//     /// <summary>
//     /// Initialize core components and state
//     /// </summary>
//     private void InitializeComponents()
//     {
//         // Null checks and initial setup
//         if (targetObject == null)
//         {
//             Debug.LogWarning("Target object is not assigned!");
//         }

//         isInitialized = true;
//         IsActive = true;
//     }

//     /// <summary>
//     /// Setup references and dependencies
//     /// </summary>
//     private void SetupReferences()
//     {
//         // Additional setup logic
//         if (optionalRigidbody == null)
//         {
//             optionalRigidbody = GetComponent<Rigidbody>();
//         }
//     }

//     /// <summary>
//     /// Handle regular frame-based updates
//     /// </summary>
//     private void HandleRegularUpdates()
//     {
//         // Example update logic
//     }

//     /// <summary>
//     /// Perform physics-based calculations
//     /// </summary>
//     private void PerformPhysicsCalculations()
//     {
//         // Physics update logic
//     }

//     /// <summary>
//     /// Public method to enable the script's functionality
//     /// </summary>
//     public void Enable()
//     {
//         IsActive = true;
//         // Additional enable logic
//     }

//     /// <summary>
//     /// Public method to disable the script's functionality
//     /// </summary>
//     public void Disable()
//     {
//         IsActive = false;
//         // Additional disable logic
//     }

//     // Optional: OnCollisionEnter for physics interactions
//     private void OnCollisionEnter(Collision collision)
//     {
//         // Handle collision logic
//         Debug.Log($"Collided with: {collision.gameObject.name}");
//     }

//     // Optional: OnTriggerEnter for trigger-based interactions
//     private void OnTriggerEnter(Collider other)
//     {
//         // Handle trigger logic
//         Debug.Log($"Triggered by: {other.gameObject.name}");
//     }

//     // Optional: Cleanup method
//     private void OnDisable()
//     {
//         // Cleanup logic when script is disabled
//         isInitialized = false;
//     }
// }