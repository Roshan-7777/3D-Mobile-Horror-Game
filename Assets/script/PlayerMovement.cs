using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // --- VARIABLES (The "Settings" for your player) ---
    public float walkSpeed = 5f;        // How many meters the player moves per second while walking
    public float runSpeed = 10f;       // How many meters the player moves per second while running
    public float gravity = -25f;       // The strength of gravity pulling the player down
    public bool canMove = true;        // A switch to turn movement ON or OFF

    [Header("Jumping Settings")]
    public float jumpHeight = 2.5f;     // How high the player can jump in meters
    private bool shouldJump = false;    // A mobile switch to track if the UI Jump button was pressed

    [Header("Joystick Running")]
    [Range(0, 1)] 
    public float runThreshold = 0.9f;  // If the joystick is pushed 90% of the way out, start running

    private CharacterController controller; // The "Body" component that handles collisions
    private Vector3 velocity;               // Stores how fast the player is falling or jumping
    public bl_Joystick Joystick;            // A reference to your on-screen mobile joystick script

    // Start is called once at the very beginning
    void Start() => controller = GetComponent<CharacterController>(); 

    // Update is called once per frame (around 60 times a second)
    void Update() 
    {
        // If movement is disabled (monster caught you), exit this function immediately
        if (!canMove) return;

        // 1. GROUND CHECK (Prevents gravity from building up infinite downward speed)
        bool isGrounded = controller.isGrounded; 
        if (isGrounded && velocity.y < 0) 
        {
            velocity.y = -2f; // Press the player slightly into the ground to keep things stable
        }

        // 2. GET INPUT VALUES (Reading what the user is doing)
        float x = Input.GetAxis("Horizontal"); // Reads 'A'/'D' keys on PC
        float z = Input.GetAxis("Vertical");   // Reads 'W'/'S' keys on PC
        float joystickMagnitude = 0f;          // Stores how far the joystick is pushed

        // If the joystick object exists in the scene, add its mobile values
        if (Joystick != null) 
        {
            x += Joystick.Horizontal; 
            z += Joystick.Vertical;   

            // Calculate distance from center (0 to 1) using Vector2 math
            joystickMagnitude = new Vector2(Joystick.Horizontal, Joystick.Vertical).magnitude;
        }

        // 3. DETERMINE SPEED (Choosing between Walk or Run)
        float currentSpeed = walkSpeed; 

        // If Shift is held (PC) OR the joystick is pushed almost to the edge (Mobile)
        if (Input.GetKey(KeyCode.LeftShift) || joystickMagnitude >= runThreshold)
        {
            currentSpeed = runSpeed; 
        }

        // 4. APPLY MOVEMENT (Calculating direction in 3D space)
        Vector3 move = transform.right * x + transform.forward * z;
        
        // Prevent moving faster diagonally
        if (move.magnitude > 1) move.Normalize();

        // Physically move the player body forward/back/left/right
        controller.Move(move * currentSpeed * Time.deltaTime);

        // --- NEW: JUMPING CALCULATION ---
        // Checks if Spacebar is pressed (PC) OR if the Mobile UI Jump button was tapped
        if ((Input.GetButtonDown("Jump") || shouldJump) && isGrounded)
        {
            // Standard Physics Formula for clean jumps: v = sqrt(h * -2 * g)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            shouldJump = false; // Reset the mobile jump switch immediately
        }

        // 5. APPLY GRAVITY (Pulling the player down)
        velocity.y += gravity * Time.deltaTime; 
        
        // Physically move the player body downward/upward
        controller.Move(velocity * Time.deltaTime);
    }

    // --- NEW: PUBLIC MOBILE TRIGGER FUNCTION ---
    // Link this function directly to your mobile Canvas Jump Button UI elements
    public void MobileJump()
    {
        // Only allow a jump trigger if the player is safely standing on the ground
        if (controller.isGrounded)
        {
            shouldJump = true;
        }
    }
}