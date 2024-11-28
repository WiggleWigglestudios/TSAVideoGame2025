using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject[] followObjects;
    Camera cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 center=new Vector3();
        Vector2 min = new Vector2(100000, 100000);
        Vector2 max = new Vector2(-100000, -100000);
        for (int i = 0; i < followObjects.Length; i++)
        {
            center += followObjects[i].transform.position;
            min.x = Mathf.Min(followObjects[i].transform.position.x, min.x);
            min.y = Mathf.Min(followObjects[i].transform.position.y, min.y);

            max.x = Mathf.Max(followObjects[i].transform.position.x, max.x);
            max.y = Mathf.Max(followObjects[i].transform.position.y, max.y);
        }
        center /= followObjects.Length;
        Vector3 dir = center - transform.position;
        dir.z = 0;
        float distance = dir.magnitude;
        dir.Normalize();


        transform.position += dir*distance*Mathf.Pow(0.5f,Time.deltaTime)*Time.deltaTime;
        cam.orthographicSize +=(Mathf.Max(10, Vector2.Distance(min, max)*1.05f)- cam.orthographicSize)*Mathf.Pow(0.5f,Time.deltaTime)*Time.deltaTime;
    }
}
