using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; 

public class PlayerInteraction : MonoBehaviour
{
    [Header("Detection Settings")]
    public float interactDistance = 4f;
    public LayerMask interactLayer; 

    [Header("Key System")]
    public int keysRequired = 3;    
    public int keysCollected = 0;   

    // --- ADDED: DOCUMENT MISSION VARIABLES ---
    [Header("Document Mission Settings")]
    public bool hasDocument = false; // Tracks if the secret document is collected

    [Header("UI References")]
    public Image crosshair;
    public TextMeshProUGUI keyCounterText;     
    public GameObject winPanel;      
    public TextMeshProUGUI objectiveTextUI; // UI text component to display current objective messages

    [Header("Mobile Interaction Button")]
    public GameObject interactButton; // The Button UI object
    public TextMeshProUGUI interactText; // The Text inside the button

    void Start()
    {
        UpdateUI(); 
        if(interactButton != null) interactButton.SetActive(false); // Hide at start
        UpdateObjectiveText("Objective: Collect 3 Keys & Find the Secret Document!");
    }

    void Update()
    {
        // 1. Raycast for Visual Feedback & Button Visibility
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactDistance, interactLayer))
        {
            crosshair.color = Color.red;
            ShowInteractionUI(hit.collider.gameObject);
        }
        else
        {
            crosshair.color = Color.white;
            if(interactButton != null) interactButton.SetActive(false); // Hide when looking away
        }

        // 2. PC Input (E Key)
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckForInteraction();
        }
    }

    // --- UPDATED FUNCTION TO SUPPORT THE DOCUMENT PAYLOAD ---
    void ShowInteractionUI(GameObject target)
    {
        if (interactButton == null) return;

        if (target.CompareTag("Key"))
        {
            interactButton.SetActive(true);
            interactText.text = "Pick Up Key";
        }
        // --- ADDED: Document Detection UI Check ---
        else if (target.CompareTag("Document"))
        {
            interactButton.SetActive(true);
            interactText.text = "Pick Up Document";
        }
        else if (target.CompareTag("ExitGate"))
        {
            interactButton.SetActive(true);
            interactText.text = "Escape!";
        }
        else
        {
            interactButton.SetActive(false);
        }
    }

    public void CheckForInteraction()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactDistance, interactLayer))
        {
            DoInteraction(hit.collider.gameObject);
        }
    }

    // --- MAIN LOGIC EXECUTION ---
    public void DoInteraction(GameObject target)
    {
        if (target.CompareTag("Key"))
        {
            keysCollected++;
            UpdateUI();
            Destroy(target); 
            Debug.Log("Key Picked Up! Total: " + keysCollected);
            
            CheckMissionProgress();
        }
        // --- ADDED: Handle picking up the secret document via Raycast ---
        else if (target.CompareTag("Document"))
        {
            hasDocument = true;
            Destroy(target);
            if(interactButton != null) interactButton.SetActive(false); // Hide immediately upon retrieval
            Debug.Log("Document Secured!");
            
            CheckMissionProgress();
        }
        else if (target.CompareTag("ExitGate"))
        {
            // NEW RULES: Requires BOTH all 3 keys AND the secret document to unlock the win condition
            if (keysCollected >= keysRequired && hasDocument)
            {
                WinGame();
            }
            else
            {
                // Give dynamic feedback based on what is missing
                if (keysCollected < keysRequired && !hasDocument) {
                    UpdateObjectiveText("LOCKED! You need all keys AND the document!");
                } else if (keysCollected < keysRequired) {
                    UpdateObjectiveText($"LOCKED! Missing keys ({keysCollected}/{keysRequired})!");
                } else if (!hasDocument) {
                    UpdateObjectiveText("LOCKED! You forgot the Document!");
                }
                Debug.Log("Escape blocked. Requirements incomplete.");
            }
        }
    }

    void UpdateUI()
    {
        if (keyCounterText != null)
        {
            keyCounterText.text = "Keys: " + keysCollected + " / " + keysRequired;
        }
    }

    // Helper to keep mission guidance crisp and responsive
    void CheckMissionProgress()
    {
        if (keysCollected >= keysRequired && hasDocument) {
            UpdateObjectiveText("All objectives met! Reach the Escape Gate!");
        } else if (keysCollected >= keysRequired && !hasDocument) {
            UpdateObjectiveText("Keys collected! Now track down the Document!");
        } else if (keysCollected < keysRequired && hasDocument) {
            UpdateObjectiveText($"Document secured! Gather remaining keys ({keysCollected}/{keysRequired})");
        }
    }

    void UpdateObjectiveText(string textMessage)
    {
        if (objectiveTextUI != null)
        {
            objectiveTextUI.text = textMessage;
        }
    }

    void WinGame()
    {
        winPanel.SetActive(true);
        Time.timeScale = 0; 
        
        // Connect cleanly with your revised MainMenuManager syntax too if required:
        Object.FindFirstObjectByType<MainMenuManager>().ShowGameOver();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}