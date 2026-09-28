using UnityEngine;

public class FlickeringLight : MonoBehaviour {
    public Light lightSource;
    void Update() {
        if (Random.value > 0.9f) { // Flickers 10% of the time
            lightSource.enabled = !lightSource.enabled;
        }
    }
}
