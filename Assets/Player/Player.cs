using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float jumpForce;
    [Header("Component Refs")]
    [SerializeField] private Rigidbody2D rb;

    //physics vars
    private Vector2 velocity = Vector2.zero;
    private bool jumped = false;

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


}
