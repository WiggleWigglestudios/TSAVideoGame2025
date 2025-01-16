using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour
{
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public AudioSource audioSource;

    private void Start()
    {
        //audioSource = FindObjectOfType<AudioSource>(); 
    }

    public void PlayHoverSound()
    {
        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void PlayClickSound()
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
