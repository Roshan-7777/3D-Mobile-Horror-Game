using UnityEngine;
using UnityEngine.SceneManagement; // <-- ADD THIS for Retry/Restart logic

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject startMenuPanel;
    public GameObject gameOverPanel; // <-- ADD THIS slot

    void Awake()
    {
        // Ensure the game starts frozen for the menu
        ShowStartMenu();
    }

    public void ShowStartMenu()
    {
        Time.timeScale = 0f;
        if (startMenuPanel != null) startMenuPanel.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;
        if (startMenuPanel != null) startMenuPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // Lock cursor for the "Look" script to work
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
    }

    // --- ADDED FOR THE RETRY BUTTON ---
    public void RetryGame()
    {
        Time.timeScale = 1f; // IMPORTANT: Unfreeze time before reloading
        
        // This reloads whatever scene you are currently in
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- CALL THIS WHEN MONSTER HITS PLAYER ---
    public void ShowGameOver()
    {
        Time.timeScale = 0f;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit(); 
    }
}