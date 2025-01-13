using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource footstepSource; // Reference to the Audio Source
    public AudioClip leftFootSound;   // Sound for the left foot
    public AudioClip rightFootSound;  // Sound for the right foot
    public float stepInterval = 0.5f; // Time between steps
    private float stepTimer;
    private bool isLeftFoot = true;  // Toggle between left and right foot
    private Player playerScript;     // Reference to the Player script

    void Start()
    {
        // Get the Player script from the same GameObject
        playerScript = GetComponent<Player>();
        if (footstepSource == null)
        {
            footstepSource = GetComponent<AudioSource>();
        }
        stepTimer = 0f;
    }

    void Update()
    {
        // Check player state
        if (playerScript != null)
        {
            // Check if the player is grounded, not in water, and moving
            if (playerScript.grounded && !playerScript.inWater && Mathf.Abs(playerScript.vel.x) > 0.1f)
            {
                stepTimer -= Time.deltaTime;

                if (stepTimer <= 0f)
                {
                    PlayFootstep();
                    stepTimer = stepInterval; // Reset the step timer
                }
            }
            else
            {
                stepTimer = 0f; // Reset timer when not moving
            }
        }
        else
        {
            Debug.LogWarning("Player script reference is missing!");
        }
    }

    void PlayFootstep()
    {
        // Alternate between left and right foot sounds
        footstepSource.clip = isLeftFoot ? leftFootSound : rightFootSound;
        footstepSource.Play();
        isLeftFoot = !isLeftFoot; // Toggle footstep sound
    }
}
