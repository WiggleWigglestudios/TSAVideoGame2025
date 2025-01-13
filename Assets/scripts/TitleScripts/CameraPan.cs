using UnityEngine;

public class BackgroundCameraPan : MonoBehaviour
{
    public Vector3 startPosition; // Starting position of the camera
    public Vector3 endPosition;   // End position of the camera
    public float speed = 2f;      // Speed of the panning
    private bool movingToEnd = true;

    void Start()
    {
        // Set the camera's initial position
        transform.position = startPosition;
    }

    void Update()
    {
        // Move the camera between startPosition and endPosition
        if (movingToEnd)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPosition, speed * Time.deltaTime);
            if (transform.position == endPosition)
            {
                movingToEnd = false; // Reverse direction
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
            if (transform.position == startPosition)
            {
                movingToEnd = true; // Reverse direction
            }
        }
    }
}
