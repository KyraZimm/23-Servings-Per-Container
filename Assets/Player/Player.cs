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

    //physics vars
    private Vector2 velocity = Vector2.zero;
    private bool jumped = false;

    public static float CurrHealth { get; private set; }
    public static float MaxHealth { get { return Instance.maxHealth; } }

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
    }
    
    private void UpdateMovement() {
        velocity = rb.velocity;
        velocity.x = Input.GetAxis("Horizontal") * walkSpeed;

        rb.velocity = velocity;

        if (!jumped && Input.GetButtonDown("Jump")) {
            rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
            jumped = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("Floor")) jumped = false;
    }

    public static void TakeDamage(float damage) {
        CurrHealth -= damage;
        if (CurrHealth <= 0) Debug.Log("Player died!");
    }

}
