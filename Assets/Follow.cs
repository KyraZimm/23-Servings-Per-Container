using UnityEngine;

public class CameraFollow : MonoBehaviour
{
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
       Vector3 desiredPosition = target.position + offset;
       
       // Smoothly move camera
       Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.fixedDeltaTime);
       transform.position = smoothedPosition;
   }

   // Method to change target at runtime (useful for cutscenes or switching characters)
   public void SetTarget(Transform newTarget)
   {
       target = newTarget;
   }
}
