using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI player1Text;
    public TextMeshProUGUI player2Text;

    public Button nextButton;
    public Button skipButton;
    public AudioSource rumbleSound;
    public CameraShake cameraShake;
    private bool skipTyping = false;


    private (string speaker, string text)[] dialogues = {
    ("Player1", "Waddles! Humanity has been bunkered underground for a millennia, but it looks like the world is finally collapsing!"),
    ("Player2", "Wiggles... Our time has finally come. Except... it has come for one of us..."),
    ("Player2", "That last spaceship is mine!"),
    ("Player1", "Not if I beat you first!")
    };

    private int dialogueIndex = 0;
    private bool isTyping = false;

    void Start()
    {
        nextButton.onClick.AddListener(DisplayNextDialogue);
        skipButton.onClick.AddListener(SkipCutscene);

        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        rumbleSound.Play();
        yield return StartCoroutine(cameraShake.Shake(1f, 0.5f));

        DisplayNextDialogue();
    }

    void DisplayNextDialogue()
    {
        if (isTyping)
        {
            skipTyping = true;
            return;
        }

        if (dialogueIndex < dialogues.Length)
        {
            StartCoroutine(ShakeAndPlayDialogue());
        }
        else
        {
            HideNextButton();
        }
    }


    private IEnumerator ShakeAndPlayDialogue()
    {
        rumbleSound.Play();

        yield return StartCoroutine(cameraShake.Shake(0.5f, 0.3f)); 

        string speaker = dialogues[dialogueIndex].speaker;
        string text = dialogues[dialogueIndex].text;

        StartCoroutine(TypeDialogue(speaker, text));

        dialogueIndex++;
    }


    private IEnumerator TypeDialogue(string speaker, string dialogue)
    {
        isTyping = true;
        skipTyping = false;

        // Clear text
        player1Text.text = "";
        player2Text.text = "";

        TextMeshProUGUI targetText = speaker == "Player1" ? player1Text : player2Text;

        foreach (char c in dialogue)
        {
            if (skipTyping)
            {
                targetText.text = dialogue;
                break;
            }

            targetText.text += c;

            if (c == '.')
                yield return new WaitForSeconds(0.2f);
            else
                yield return new WaitForSeconds(0.05f);
        }

        isTyping = false;
    }


    void SkipCutscene()
    {
        StopAllCoroutines();
    }

    private void HideNextButton()
    {
        nextButton.gameObject.SetActive(false);
    }   
}
