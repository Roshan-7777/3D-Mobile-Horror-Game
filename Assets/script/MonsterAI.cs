using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MonsterAI : MonoBehaviour
{
    [Header("Movement & Detection")]
    public NavMeshAgent agent;
    public Transform point1, point2, point3, player; 
    public float detectionRange = 20f;
    private int currentPointIndex = 1; 

    [Header("Animation & Sound")]
    public Animator anim;
    public AudioSource monsterAudioSource; 
    public AudioSource chaseMusicSource;   
    public AudioClip rageSound;            
    public string rageTrigger = "PlayRage";

    [Header("Death Camera & UI")]
    public GameObject gameOverPanel;
    public Camera playerCamera;
    public float cameraFallSpeed = 4f; 
    public float lockOnSpeed = 15f; 
    private bool isAttacking = false;
    private bool isRaging = false;
    private bool isChasing = false;
    private bool cameraIsFalling = false; 
    public GameObject gameOverUI;

    void Start()
    {
        if (chaseMusicSource != null) chaseMusicSource.Stop();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        
        agent.SetDestination(point1.position);
    }

    void Update()
    {
        if (isAttacking) {
            HandleDeathCamera(); 
            if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        if (isRaging) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool canSee = CanSeePlayer();

        if (distance < detectionRange && canSee) {
            if (!isChasing) {
                StartCoroutine(PlayRageRoutine());
            } else {
                agent.isStopped = false;
                agent.speed = 5f;
                agent.SetDestination(player.position);
            }
        } 
        else {
            if (distance > detectionRange + 5f && isChasing) { 
                isChasing = false; 
                if (chaseMusicSource != null && chaseMusicSource.isPlaying) chaseMusicSource.Stop();
            }
            HandlePatrol();
        }
    }

    IEnumerator PlayRageRoutine() {
        isChasing = true; 
        isRaging = true;
        agent.isStopped = true;

        if (monsterAudioSource != null && rageSound != null) monsterAudioSource.PlayOneShot(rageSound);
        if (chaseMusicSource != null && !chaseMusicSource.isPlaying) chaseMusicSource.Play();

        anim.SetTrigger(rageTrigger);
        yield return new WaitForSeconds(2.0f); 
        
        isRaging = false;
        agent.isStopped = false;
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && !isAttacking) {
            StartCoroutine(GrannyStyleDeathRoutine());
        }
    }

    IEnumerator GrannyStyleDeathRoutine() {
        isAttacking = true;
        agent.isStopped = true;

        // --- CAMERA LOCK-ON START ---
        MouseLook ml = playerCamera.GetComponent<MouseLook>();
        if (ml != null) {
            ml.monsterTarget = this.transform; 
            ml.isLockedOn = true;              
        }

        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        if (pm != null) pm.canMove = false;

        anim.SetTrigger("Attack");
        if (chaseMusicSource != null) chaseMusicSource.Stop();

        yield return new WaitForSeconds(0.4f); 
        cameraIsFalling = true; 

        yield return new WaitForSeconds(1.6f);
        
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Object.FindFirstObjectByType<MainMenuManager>().ShowGameOver();
    }

    void HandleDeathCamera() {
        if (playerCamera == null) return;

        Vector3 headPosition = transform.position + Vector3.up * 1.7f; 
        Vector3 directionToMonster = headPosition - playerCamera.transform.position;
        
        Quaternion targetRotation = Quaternion.LookRotation(directionToMonster);
        
        playerCamera.transform.rotation = Quaternion.Slerp(
            playerCamera.transform.rotation, 
            targetRotation, 
            Time.deltaTime * lockOnSpeed
        );

        if (cameraIsFalling) {
            playerCamera.transform.Rotate(0, 0, 40f * Time.deltaTime);
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition, 
                new Vector3(0, 0.3f, 0), 
                Time.deltaTime * cameraFallSpeed
            );
        }
    }

    void HandlePatrol() {
        agent.speed = 2.5f;
        if (!agent.pathPending && agent.remainingDistance < 0.5f) {
            currentPointIndex++;
            if (currentPointIndex > 3) currentPointIndex = 1;

            if (currentPointIndex == 1) agent.SetDestination(point1.position);
            else if (currentPointIndex == 2) agent.SetDestination(point2.position);
            else if (currentPointIndex == 3) agent.SetDestination(point3.position);
        }
    }

    bool CanSeePlayer() {
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up;
        Vector3 dir = (player.position - rayStart).normalized;
        if (Physics.Raycast(rayStart, dir, out hit, detectionRange)) {
            return hit.transform.CompareTag("Player");
        }
        return false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CatchPlayer();
        }
    }

    void CatchPlayer()
    {
        // --- CAMERA LOCK-ON START ---
        MouseLook ml = playerCamera.GetComponent<MouseLook>();
        if (ml != null) {
            ml.monsterTarget = this.transform;
            ml.isLockedOn = true;
        }

        Time.timeScale = 0;
        if (gameOverUI != null) gameOverUI.SetActive(true);
        
        Object.FindFirstObjectByType<MainMenuManager>().ShowGameOver();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Player Caught!");
    }
}