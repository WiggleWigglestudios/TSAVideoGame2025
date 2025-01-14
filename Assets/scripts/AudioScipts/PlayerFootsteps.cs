using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource footstepSource; 
    public AudioClip leftFootSound;   
    public AudioClip rightFootSound;  
    public float stepInterval = 0.5f; 
    private float stepTimer;
    private bool isLeftFoot = true;  
    private Player playerScript;     

    void Start()
    {
        playerScript = GetComponent<Player>();
        if (footstepSource == null)
        {
            footstepSource = GetComponent<AudioSource>();
        }
        stepTimer = 0f;
    }

    void Update()
    {
        if (playerScript != null)
        {
            if (playerScript.grounded && !playerScript.inWater && Mathf.Abs(playerScript.vel.x) > 0.1f)
            {
                stepTimer -= Time.deltaTime;

                if (stepTimer <= 0f)
                {
                    PlayFootstep();
                    stepTimer = stepInterval; 
                }
            }
            else
            {
                stepTimer = 0f; 
            }
        }
        else
        {
            Debug.LogWarning("Player script reference is missing!");
        }
    }

    void PlayFootstep()
    {
        footstepSource.clip = isLeftFoot ? leftFootSound : rightFootSound;
        footstepSource.Play();
        isLeftFoot = !isLeftFoot; 
    }
}
