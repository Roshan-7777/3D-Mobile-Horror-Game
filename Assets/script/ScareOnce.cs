using UnityEngine;

public class ScareOnce : MonoBehaviour
{
    public AudioSource scareSound;
    private bool hasPlayed = false; // This remembers if it played or not

    void OnTriggerEnter(Collider other)
    {
        // Check if the Player entered AND if we haven't played the sound yet
        if (other.CompareTag("Player") && !hasPlayed)
        {
            scareSound.Play();   // Play the sound
            hasPlayed = true;    // Set it to true so it never plays again
            Debug.Log("Scare Triggered!");
        }
    }
}