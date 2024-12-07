using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPrototype_V1 : MonoBehaviour
{
    [SerializeField] float maxHealth;

    public static float CurrHealth { get; private set; }
    public static float MaxHealth { get; private set; }

    private void Awake() {
        CurrHealth = maxHealth;
        MaxHealth = maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.CompareTag("Needle")) {
            NeedleScript needle = col.GetComponent<NeedleScript>();

            //safety check: has NeedleScript component at root
            if (needle == null) {
                Debug.LogError($"Could not retrieve {nameof(NeedleScript)} at the root of {col.gameObject.name},  even though this object is tagged as Needle.");
                return;
            }

            TakeDamage(needle.Damage);
            HandleReflection(needle);
        }
    }

    public void TakeDamage(float damage) {
        CurrHealth -= damage;
        if (CurrHealth <= 0) Debug.Log("The Child is dead!");
    }

    private void HandleReflection(NeedleScript needle) {
        Vector2 dir = (needle.transform.position - transform.position).normalized;
        float speed = needle.Velocity.magnitude;
        needle.SetVelocity(dir * speed);
    }

}
