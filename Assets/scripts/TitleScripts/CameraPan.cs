using UnityEngine;

public class BackgroundCameraPan : MonoBehaviour
{
    public Vector3 startPosition;
    public Vector3 endPosition;  
    public float speed = 2f;     
    private bool movingToEnd = true;

    void Start()
    {
        transform.position = startPosition;
    }

    void Update()
    {
        if (movingToEnd)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPosition, speed * Time.deltaTime);
            if (transform.position == endPosition)
            {
                movingToEnd = false; 
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
            if (transform.position == startPosition)
            {
                movingToEnd = true; 
            }
        }
    }
}
