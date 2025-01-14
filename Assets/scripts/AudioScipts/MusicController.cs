using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioClip song1; 
    public AudioClip song2;     private AudioSource audioSource;
    private bool playFirstSong;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        playFirstSong = Random.Range(0, 2) == 0;

        PlayNextSong();
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            playFirstSong = !playFirstSong; // Alternate 
            PlayNextSong();
        }
    }

    void PlayNextSong()
    {
        audioSource.clip = playFirstSong ? song1 : song2;
        audioSource.Play();
    }
}
