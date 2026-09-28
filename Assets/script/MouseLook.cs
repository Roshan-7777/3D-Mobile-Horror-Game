using UnityEngine;

public class MouseLook : MonoBehaviour
{
    // --- VARIABLES ---
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    float xRotation = 0f;

    [Header("Mobile Settings")]
    public float touchSensitivity = 0.5f;

    [Header("Attack Lock-On")]
    public bool isLockedOn = false;    // MonsterAI will set this to true when it catches you
    public Transform monsterTarget;    // The Monster's transform
    public float lockOnSpeed = 10f;    // How fast the camera snaps to the monster

    void Start()
    {
        // Cursor logic
        if (Time.timeScale > 0) 
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else 
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        // If we are caught, run the Lock-On logic and ignore mouse/touch input
        if (isLockedOn && monsterTarget != null)
        {
            HandleMonsterLockOn();
            return; 
        }

        if (Time.timeScale == 0) return; 

        float mouseX = 0;
        float mouseY = 0;

        // --- 1. TOUCH INPUT (MOBILE) ---
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.position.x > Screen.width / 2)
                {
                    if (touch.phase == TouchPhase.Moved)
                    {
                        mouseX = touch.deltaPosition.x * touchSensitivity;
                        mouseY = touch.deltaPosition.y * touchSensitivity;
                    }
                }
            }
        }
        // --- 2. MOUSE INPUT (PC) ---
        else
        {
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        }

        xRotation -= mouseY; 
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); 

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    // This function makes the camera stare at the monster during the jumpscare
    void HandleMonsterLockOn()
    {
        // Calculate the direction from camera to monster's head
        Vector3 targetDir = (monsterTarget.position + Vector3.up * 1.5f) - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(targetDir);

        // Smoothly rotate the camera toward the monster
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lockOnSpeed);
        
        // Also rotate the player body so the whole character faces the monster
        Vector3 bodyRotation = lookRotation.eulerAngles;
        playerBody.rotation = Quaternion.Euler(0, bodyRotation.y, 0);
    }
}