using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SpawnerPrototype : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Projectile spawned by the spawner")]
    [SerializeField] private GameObject projectile;
    [Tooltip("How long the spawner takes")]
    [SerializeField] private float time = 3f;
    [Tooltip("How many projectiles the spawner spawns")]
    [SerializeField] private uint count = 3;

    private Vector2 pos_0;
    [Tooltip("The vector along which the spawner moves")]
    [SerializeField] private Vector2 travel;

    private bool isActive = true;
    private float elapsedTime = 0f;
    private uint spawned = 0;

    // Start is called before the first frame update
    void Start()
    {
        pos_0 = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;

        elapsedTime += Time.deltaTime;

        float t = Mathf.Clamp01(elapsedTime / time);
        transform.position = Vector2.Lerp(pos_0, pos_0 + travel, t);

        // Optionally spawn the projectile
        SpawnProjectile();

        // Reset or stop when movement is complete
        if (t >= 1f)
        {
            isActive = false;
        }
    }

    void SpawnProjectile()
    {
        if (projectile == null || count <= 0) return;
        if (spawned >= count) return;

        float spawnTime = time / (count - 1) * spawned;
        if (elapsedTime >= spawnTime)
        {
            spawned += 1;
            Instantiate(projectile, transform.position, Quaternion.identity);
        }
    }

    public void Reset()
    {
        elapsedTime = 0f;
        transform.position = pos_0;
        isActive = true;
    }

    // Visualization in editor to help with positioning
    private void OnDrawGizmosSelected()
    {
        // Draw a line between pos_0 and pos_1
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)travel);
    }
}
