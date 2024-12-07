using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] float maxHealth;
    [Header("Component Refs")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] float throwRechargeTime;

    //physics vars
    private Vector2 velocity = Vector2.zero;
    private bool jumped = false;

    public static float CurrHealth { get; private set; }
    public static float MaxHealth { get { return Instance.maxHealth; } }

    private float nextThrowTime = 0f;

    private void Awake() {

        //singleton initialization
        if (Instance != null) {
            Debug.LogWarning($"A later instance of {nameof(Player)} on {gameObject.name} was destroyed to keep an earlier instance on {Instance.gameObject.name}.");
            DestroyImmediate(this);
            return;
        }
        Instance = this;

        CurrHealth = maxHealth;
    }

    private void Update() {
        UpdateMovement();
        UpdateNeedle();
    }

    [SerializeField] public GameObject needlePrefab;

    [SerializeField] public float shootForce;

    // Boolean (or some other structure) for tracking whether or not the player has a needle
    [SerializeField] public bool hasNeedle;
    
    private void UpdateMovement() {
        velocity = rb.velocity;
        velocity.x = Input.GetAxis("Horizontal") * walkSpeed;

        rb.velocity = velocity;

        if (!jumped && Input.GetButtonDown("Jump")) {
            rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
            jumped = true;
        }
    }

    private void UpdateNeedle()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextThrowTime) // 0 for left mouse button
        {
            // Get the mouse position in world space
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Ensure it's on the same 2D plane as the player

            // Calculate direction from player to mouse
            Vector2 shootDirection = (mousePosition - transform.position).normalized;

            // Offset the instantiation location to be outside the player collider
            float spawnDistance = 1.0f; // Adjust based on your player's collider size
            Vector3 instantiationLocation = transform.position + (Vector3)(shootDirection * spawnDistance);

            // Calculate the rotation for the needle
            float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg + 270;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            // Instantiate the needle
            GameObject needle = Instantiate(needlePrefab, instantiationLocation, rotation);

            // Apply velocity to the needle
            Rigidbody2D rb = needle.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = shootDirection * shootForce;
            }
            nextThrowTime = Time.time + throwRechargeTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Floor")) jumped = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player touches a spear
        if (other.CompareTag("Needle"))
        {
            // Destroy the spear if the recharge time has passed
            if (Time.time >= nextThrowTime)
            {
                Destroy(other.gameObject);
            }
        }
    }

    public static void TakeDamage(float damage) {
        CurrHealth -= damage;
        if (CurrHealth <= 0) Debug.Log("Player died!");
    }

}
