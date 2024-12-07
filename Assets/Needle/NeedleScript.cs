using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleScript : MonoBehaviour
{

    private Rigidbody2D rb;

    private bool isColliding;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D rigidBody = transform.GetComponent<Rigidbody2D>();
        Debug.Log($"Center of Mass: {rigidBody.centerOfMass}");
        Debug.Log($"Position: {transform.position}");
        // Set a new center of mass
        rigidBody.centerOfMass = new Vector2(0, 0);
        Debug.Log($"New Center of Mass: {rigidBody.centerOfMass}");
        rb = GetComponent<Rigidbody2D>();
        isColliding = false;
    }

    void OnDrawGizmos()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 worldCenterOfMass = rb.worldCenterOfMass;

        // Draw a small red line to mark the center of mass
        Gizmos.color = Color.red;
        Gizmos.DrawLine(worldCenterOfMass - Vector2.up * 0.1f, worldCenterOfMass + Vector2.up * 0.1f);
        Gizmos.DrawLine(worldCenterOfMass - Vector2.right * 0.1f, worldCenterOfMass + Vector2.right * 0.1f);
    }

    void FixedUpdate()
    {
        // Get the velocity vector
        Vector2 velocity = rb.velocity;

        // Only update rotation if the object is moving
        if (velocity.sqrMagnitude > 0.1f && !isColliding)
        {
            // Calculate the angle in degrees
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg + 270;

            // Apply the rotation (2D rotation is around the Z-axis)
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    // This method is called when a collision starts
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Set the isColliding flag to true
        isColliding = true;
    }

    // This method is called as long as the object is colliding
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Keep the isColliding flag true while the object is colliding
        isColliding = true;
    }

    // This method is called when a collision ends
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Set the isColliding flag to false when the collision ends
        isColliding = false;
    }
}
