using System.Collections;
using UnityEngine;

public class thwomp : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float horizontalSpeed = 5f;
    [SerializeField] private float verticalSpeed = 15f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float aboveRange = 0.5f;
    [SerializeField] private float hoverHeight = 5f;
    [SerializeField] private float pauseBeforeSlam = 0.5f;
    [SerializeField] private float groundPauseTime = 1f;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask groundLayer;

    // State variables
    private Vector2 initialPosition;
    private bool isTracking = true;
    private bool isSlamming = false;

    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTracking)
        {
            TrackPlayer();
        }
    }

    public void SetPlayer(Transform player){ this.player = player; }

    private void TrackPlayer()
    {
        float distanceToPlayer = Mathf.Abs(transform.position.x - player.position.x);
        if (distanceToPlayer <= detectionRange)
        {
            // Move towards player horizontally
            float targetX = Mathf.MoveTowards(transform.position.x,
                                            player.position.x,
                                            horizontalSpeed * Time.deltaTime);

            transform.position = new Vector2(targetX,
                                           initialPosition.y);

            // Check if we're above the player
            if (Mathf.Abs(transform.position.x - player.position.x) < aboveRange)
            {
                StartCoroutine(SlamAttack());
            }
        }
    }

    private IEnumerator SlamAttack()
    {
        isTracking = false;
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

        isTracking = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
