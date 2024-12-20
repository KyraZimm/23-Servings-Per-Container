using UnityEngine;

public class SoundTriggerZone : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if it's the player and sound hasn't played yet
        if (other.CompareTag("Player") && !hasPlayed)
        {
            audioSource.Play();
            hasPlayed = true;
        }
    }
}
