using UnityEngine;

public class JumpScare : MonoBehaviour {
    public AudioSource scareSound;
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) { // Make sure your Player has the tag "Player"
            scareSound.Play();
        }
    }
}