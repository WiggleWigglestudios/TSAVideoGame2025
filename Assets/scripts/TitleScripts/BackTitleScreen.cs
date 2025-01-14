using UnityEngine;
using UnityEngine.SceneManagement;

public class toTitle : MonoBehaviour
{
    public void BacktoMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("TitleScreen"); 
    }
}