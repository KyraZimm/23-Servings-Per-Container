using System.Collections;
using UnityEngine;

public class thwomp : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float damage = 5f;
    [SerializeField] private float horizontalSpeed = 5f;
    [SerializeField] private float verticalSpeed = 15f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float aboveRange = 0.5f;
    [SerializeField] private float hoverHeight = 5f;
    [SerializeField] private float pauseBeforeSlam = 0.5f;
    [SerializeField] private float groundPauseTime = 1f;

    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask groundLayer;

    // State variables
    private Vector2 initialPosition;
    private bool isIdle = true;
    private bool isTracking = false;
    private bool isSlamming = false;

    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isIdle)
        {
            Idle();
        }
        else if (isTracking)
        {
            TrackPlayer();
        }
    }

    void OnCollisionEnter(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            Player.TakeDamage(damage);
        }
    }

    public void SetPlayer(GameObject player) { this.player = player; }

    // Return to default position
    private void Idle()
    {
        float distanceToPlayer = Mathf.Abs(transform.position.x - player.transform.position.x);
        bool playerAboveThwomp = player.transform.position.y > transform.position.y;
        if (distanceToPlayer < detectionRange && !playerAboveThwomp)
        {
            isIdle = false;
            isTracking = true;
            return;
        }

        // Move to start height
        if (transform.position.y != initialPosition.y)
        {
            transform.position = Vector2.MoveTowards(transform.position,
                                                     new Vector2(transform.position.x, initialPosition.y),
                                                     verticalSpeed * 0.5f * Time.deltaTime);
        }
        // Move towards start position horizontally
        else if (transform.position.x != initialPosition.x)
        {
            transform.position = Vector2.MoveTowards(transform.position,
                                                     new Vector2(initialPosition.x, initialPosition.y),
                                                     horizontalSpeed * 0.5f * Time.deltaTime);
        }
    }

    // Follow player
    private void TrackPlayer()
    {
        float distanceToPlayer = Mathf.Abs(transform.position.x - player.transform.position.x);
        if (distanceToPlayer <= detectionRange)
        {
            // Move towards player horizontally
            float targetX = Mathf.MoveTowards(transform.position.x,
                                            player.transform.position.x,
                                            horizontalSpeed * Time.deltaTime);

            transform.position = new Vector2(targetX,
                                           initialPosition.y);

            // Check if we're above the player
            if (Mathf.Abs(transform.position.x - player.transform.position.x) < aboveRange &&
                transform.position.y > player.transform.position.y)
            {
                StartCoroutine(SlamAttack());
            }
        }
    }

    private IEnumerator SlamAttack()
    {
        isTracking = false;
        isIdle = false;
        isSlamming = true;

        // Pause before slamming
        yield return new WaitForSeconds(pauseBeforeSlam);

        // Slam down
        while (true)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 100f, groundLayer);
            if (hit.collider != null && Vector2.Distance(transform.position, hit.point) < 0.1f)
            {
                break;
            }

            transform.position = Vector2.MoveTowards(transform.position,
                                                   new Vector2(transform.position.x, hit.point.y),
                                                   verticalSpeed * Time.deltaTime);

            yield return null;
        }

        // Pause at ground
        yield return new WaitForSeconds(groundPauseTime);

        // Reset position
        StartCoroutine(ResetPosition());
    }

    private IEnumerator ResetPosition()
    {
        isSlamming = false;

        // Move back to initial height
        while (Vector2.Distance(transform.position, new Vector2(transform.position.x, initialPosition.y)) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position,
                                                   new Vector2(transform.position.x, initialPosition.y),
                                                   verticalSpeed * 0.5f * Time.deltaTime);

            yield return null;
        }

        isIdle = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

}
