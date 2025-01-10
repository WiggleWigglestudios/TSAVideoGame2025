using UnityEngine;

public class ShowLevelCamera : MonoBehaviour
{
    public Camera thisCam;
    public GameObject[] otherCams;
    public Player[] players;
    public Transform[] travelPositions;
    public float[] camSizes;
    public int travelIndex;
    public float travelSpeed;
    public float zoomSpeed;
    bool finished;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].enabled = false;
        }
        for (int i = 0; i < otherCams.Length; i++)
        {
            otherCams[i].SetActive(false);
        }
        transform.position = travelPositions[0].position;
        thisCam.orthographicSize = camSizes[0];
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector2.Distance(transform.position, travelPositions[travelIndex].position)>0.05f)
        {
            float speed=Mathf.Min((travelPositions[travelIndex].position - transform.position).magnitude * Mathf.Pow(0.99f, Time.deltaTime),travelSpeed);
            transform.Translate((travelPositions[travelIndex].position - transform.position).normalized * Time.deltaTime* speed);  
        }
        if (Mathf.Abs(thisCam.orthographicSize-camSizes[travelIndex]) > 0.05f)
        {
            thisCam.orthographicSize += (camSizes[travelIndex] - thisCam.orthographicSize) * Mathf.Pow(0.99f, Time.deltaTime)* Time.deltaTime;
        }

        if (Vector2.Distance(transform.position, travelPositions[travelIndex].position) <= 0.1f &&
            Mathf.Abs(thisCam.orthographicSize - camSizes[travelIndex]) <= 2f)
        {
            if (finished)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    players[i].enabled = true;
                }
                for (int i = 0; i < otherCams.Length; i++)
                {
                    otherCams[i].SetActive(true);
                }
                gameObject.SetActive(false);
            }

            travelIndex++;
            if (travelIndex >= camSizes.Length)
            {
                finished = true;
                travelIndex = 0;
            }
        }
        
    }
}
