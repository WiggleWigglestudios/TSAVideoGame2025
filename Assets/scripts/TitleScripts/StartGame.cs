using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Brecklevel"); 
    }
    public void toTutorial()
    {
        SceneManager.LoadScene("Tutorial"); 
    }
}
