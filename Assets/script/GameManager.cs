using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject startMenuPanel;
    public GameObject player; // Drag your Player object here

    void Start()
    {
        // Pause the game and show the menu
        Time.timeScale = 0;
        startMenuPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartGame()
    {
        Time.timeScale = 1; // Unpause everything
        startMenuPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked; // Hide mouse for FPS play
    }
}