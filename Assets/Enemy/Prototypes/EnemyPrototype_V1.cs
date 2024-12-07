using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPrototype_V1 : MonoBehaviour
{
    [SerializeField] float maxHealth;
    [SerializeField] float testDamage; //will likely be set from Config or from needle prefab in the future

    public static float CurrHealth { get; private set; }
    public static float MaxHealth { get; private set; }

    private void Awake() {
        CurrHealth = maxHealth;
        MaxHealth = maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.CompareTag("Needle")) TakeDamage(testDamage);
    }

    public void TakeDamage(float damage) {
        CurrHealth -= damage;
        if (CurrHealth <= 0) Debug.Log("The Child is dead!");
    }
}
