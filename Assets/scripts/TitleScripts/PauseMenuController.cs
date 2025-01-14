using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenu; 

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    void TogglePauseMenu()
    {
        isPaused = !isPaused; 
        pauseMenu.SetActive(isPaused); 

        if (isPaused)
        {
            Time.timeScale = 0f; 
        }
        else
        {
            Time.timeScale = 1f; 
        }
    }
}
