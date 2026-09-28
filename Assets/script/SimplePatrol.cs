using UnityEngine;

public class SimplePatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2.0f;
    private Vector3 target;

    void Start() {
        target = pointB.position; // Start by moving to Point B
    }

    void Update() {
        // Move the monster towards the target
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Check if the monster reached the point
        if (Vector3.Distance(transform.position, target) < 0.1f) {
            // Switch targets
            target = (target == pointA.position) ? pointB.position : pointA.position;
            
            // Make the monster look at the new target
            transform.LookAt(target);
        }
    }
}