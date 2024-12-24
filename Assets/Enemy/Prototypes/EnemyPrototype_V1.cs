using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPrototype_V1 : MonoBehaviour
{
    [SerializeField] float maxHealth;

    public static float CurrHealth { get; private set; }
    public static float MaxHealth { get; private set; }

    private bool acceptCollision = true;
    private float timeOfLastCollision;
    private const float COLLISION_BUFFER_DURATION = 1f;

    private void Awake() {
        CurrHealth = maxHealth;
        MaxHealth = maxHealth;
    }

    private void Update() {
        //check if it's been long enough from the last collision to accept collisions again
        if (!acceptCollision)
            acceptCollision = Time.time - timeOfLastCollision >= COLLISION_BUFFER_DURATION;
    }

    public void EyeCollision(Collider2D col) {
        if (col.gameObject.CompareTag("Needle") && acceptCollision) {
            NeedleScript needle = col.GetComponent<NeedleScript>();

            //safety check: has NeedleScript component at root
            if (needle == null) {
                Debug.LogError($"Could not retrieve {nameof(NeedleScript)} at the root of {col.gameObject.name},  even though this object is tagged as Needle.");
                return;
            }

            TakeDamage(needle.Damage);
            HandleReflection(needle);

            acceptCollision = false;
        }
    }

    public void TakeDamage(float damage) {
        CurrHealth -= damage;
        if (CurrHealth <= 0) Debug.Log("The Child is dead!");
    }

    private void HandleReflection(NeedleScript needle) {
        Vector2 normal = needle.transform.position - transform.position;
        Vector2 reflectedDir = Vector2.Reflect(needle.Velocity, normal).normalized;

        float speed = needle.Velocity.magnitude;
        needle.SetVelocity(reflectedDir * speed);
    }
}
