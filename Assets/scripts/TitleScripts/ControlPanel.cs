using UnityEngine;

public class ControlPanel : MonoBehaviour
{
    public GameObject controlsPanel; 
    public GameObject[] uiElements; 

    public void ToggleControlsPanel()
    {
        bool isPanelActive = !controlsPanel.activeSelf;

        controlsPanel.SetActive(isPanelActive);

        foreach (GameObject element in uiElements)
        {
            element.SetActive(!isPanelActive);
        }
    }

    public void CloseControlsPanel()
    {
        controlsPanel.SetActive(false);

        foreach (GameObject element in uiElements)
        {
            element.SetActive(true);
        }
    }
}
