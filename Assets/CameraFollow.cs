using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Camera cam; //stashing attached camera for performance
    [SerializeField] private bool followTarget;
    [Header("Stationary Camera")]
    [SerializeField] private Vector3 stationaryPos;
    [SerializeField] private float stationarySize;
    [Header("Follow Target Camera")]
    [SerializeField] private float followSize;

    [SerializeField] 
    private Transform target;  // Transform to follow (usually the player)

    [SerializeField]
    private Vector3 offset = new Vector2(0, 5);  // Offset from target position (x=right, y=up, z=forward)

    [SerializeField] 
    private float smoothSpeed = 12;  // Higher values = faster camera movement (less smoothing)

    private void FixedUpdate()
    {
        if (target == null)
            return;

        // Calculate desired position
        Vector3 desiredPosition = followTarget ? target.position + offset : stationaryPos;
       
        // Smoothly move camera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.fixedDeltaTime);
        transform.position = smoothedPosition;

        float targetSize = followTarget ? followSize : stationarySize;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, smoothSpeed * Time.fixedDeltaTime); 
    }

    // Method to change target at runtime (useful for cutscenes or switching characters)
    public void SetTarget(Transform newTarget)
    {
       target = newTarget;
    }

    public void SetToStationary(bool makeCamStationary) {
        followTarget = !makeCamStationary;
    }
}
