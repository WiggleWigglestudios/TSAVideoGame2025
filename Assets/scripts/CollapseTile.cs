using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class CollapseTile : MonoBehaviour
{

    public Texture2D tileSet;
    public int tile;
    public float timer;
    public Vector2 pos;
    public Vector2 velocity;
    float rotationVel;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    bool falling = false;

    public void initializeCollapseTile(Texture2D _tileSet,int _tile,float _timer)
    {
        pos=transform.position;
        rotationVel = UnityEngine.Random.Range(-1000, 1000);
        tileSet = _tileSet;
        tile = _tile;
        timer = _timer;
        if (gameObject.GetComponent<MeshRenderer>() == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        else
        {
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
        }

        if (gameObject.GetComponent<MeshFilter>() == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        else
        {
            meshFilter = gameObject.GetComponent<MeshFilter>();
        }

        Mesh mesh = new Mesh();

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        verts.Add(new Vector3(-0.5f, -0.5f));
        verts.Add(new Vector3(0.5f, -0.5f));
        verts.Add(new Vector3(-0.5f, 0.5f));
        verts.Add(new Vector3(0.5f, 0.5f));
        tris.Add(0);// x + (y + 1) * levelData.GetLength(0));
        tris.Add(1);//x + y * levelData.GetLength(0));
        tris.Add(2);//x + y * levelData.GetLength(0) + 1);

        tris.Add(1);//(x + (y + 1) * levelData.GetLength(0) + 1);
        tris.Add(3);//(x + 1 + y * levelData.GetLength(0));
        tris.Add(2);

        float pixelsPerTile = 80;
        float pixelsOfPadding = 10;
        float initialPadding = 0;
        int numberOfTilesX = (int)((tileSet.width - initialPadding * 2) / (pixelsPerTile + pixelsOfPadding * 2));
        int numberOfTilesY = (int)((tileSet.height - initialPadding * 2) / (pixelsPerTile + pixelsOfPadding * 2));
        float tileX = tile % numberOfTilesX;
        float tileY = (int)(tile / numberOfTilesX);
        
        tileX = tileX * pixelsPerTile + initialPadding + pixelsOfPadding * tileX + pixelsOfPadding * (tileX + 1);
        tileY = tileY * pixelsPerTile + initialPadding + pixelsOfPadding * tileY + pixelsOfPadding * (tileY + 1);
        tileX /= tileSet.width;
        tileY /= tileSet.height;

        uvs.Add(new Vector2((tileX), 1.0f - (tileY + pixelsPerTile / tileSet.height)));
        uvs.Add(new Vector2((tileX + pixelsPerTile / tileSet.width), 1.0f - (tileY + pixelsPerTile / tileSet.height)));
        uvs.Add(new Vector2((tileX), 1.0f - (tileY)));
        uvs.Add(new Vector2((tileX + pixelsPerTile / tileSet.width), 1.0f - (tileY)));



        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();
        // mesh.RecalculateNormals();
        meshFilter.mesh = mesh;


        Material tileMapMat = new Material(Shader.Find("Sprites/Default"));
        tileMapMat.mainTexture = tileSet;

        meshRenderer.material = tileMapMat;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        timer-= Time.deltaTime;
        
        //shake
        if (timer < 2.0f&&!falling) 
        {
            transform.position = pos + 0.5f*Mathf.Pow((2-timer)/2.0f,4)*new Vector2(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1));
        }

        //fall
        if (timer < 0&&!falling)
        {
            pos = transform.position;
            falling = true;
            velocity.x = UnityEngine.Random.Range(-5, 5);
            velocity.y = UnityEngine.Random.Range(0, 15);
        }
        if (falling)
        {
            transform.position = pos;
            velocity.y -= 20.0f*Time.deltaTime;
            pos += velocity * Time.deltaTime;
            transform.Rotate(new Vector3(0,0,rotationVel * Time.deltaTime));

        }

        if (transform.position.y < -50)
        { 
            Destroy(gameObject);
        }
    }
}
