using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ShowLevelCamera : MonoBehaviour
{
    public Camera thisCam;
    public GameObject[] otherCams;
    public Player[] players;
    public Transform[] travelPositions;
    public Vector2 velocity;
    public float drag;
    public float[] camSizes;
    public int travelIndex;
    public float travelSpeed;
    public float maxTravelSpeed;
    public float zoomSpeed;
    float countDown;
    bool finished;
    public TextMeshProUGUI countDownText;
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
        countDown = 3;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector2.Distance(transform.position, travelPositions[travelIndex].position)>0.05f)
        {
            velocity += (Vector2)(travelPositions[travelIndex].position - transform.position).normalized*Time.deltaTime*travelSpeed;
            velocity = velocity.normalized * Mathf.Min(velocity.magnitude,maxTravelSpeed)* Mathf.Pow(drag, Time.deltaTime);
            transform.Translate(velocity*Time.deltaTime);
           // float speed=Mathf.Min((travelPositions[travelIndex].position - transform.position).magnitude * Mathf.Pow(travelSpeed, Time.deltaTime),maxTravelSpeed);
            //transform.Translate((travelPositions[travelIndex].position - transform.position).normalized * Time.deltaTime* speed);  
        }
        if (Mathf.Abs(thisCam.orthographicSize-camSizes[travelIndex]) > 0.05f)
        {
            thisCam.orthographicSize += (camSizes[travelIndex] - thisCam.orthographicSize) * Mathf.Pow(zoomSpeed, Time.deltaTime)* Time.deltaTime;
        }


        if (Input.GetKeyDown(KeyCode.Return))
        {
         
                finished = true;
                travelIndex = 0;
        }

        if (Vector2.Distance(transform.position, travelPositions[travelIndex].position) <= 8f &&
            Mathf.Abs(thisCam.orthographicSize - camSizes[travelIndex]) <= 2f)
        {

          
            if (finished)
            {

                countDown -= Time.deltaTime;
                countDownText.text = "" + Mathf.Ceil(countDown);
                if(countDown<=0.0f)
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
                    countDownText.text = "";
                }

            }
            else
            {


                travelIndex++;
                if (travelIndex >= camSizes.Length)
                {
                finished = true;
                travelIndex = 0;
                }
            }
        }
        
    }
}
