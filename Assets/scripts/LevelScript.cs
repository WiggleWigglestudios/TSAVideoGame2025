using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEditor;
public class LevelScript : MonoBehaviour
{

    public Texture2D tileSet;
    public int[,] levelData;
    public Tilemap tileMap;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public Vector2Int[] boundsOfLevel;

    void Start()
    {
      /*  levelData = new int[(int)Mathf.Abs(boundsOfLevel[0].x - boundsOfLevel[1].x), 
        (int)Mathf.Abs(boundsOfLevel[0].y - boundsOfLevel[1].y)];
        
        for (int i = 0; i < levelData.GetLength(0); i++)
        {
            for (int c = 0; c < levelData.GetLength(1); c++)
            {
                Debug.Log(tileMap.GetTile(new Vector3Int(0, 0, 0)));
                levelData[i,c] = (int)UnityEngine.Random.Range(0,250);
                //Debug.Log(i + " " + c + " " + levelData[i, c]);
            }
        }

        createMesh();*/
    }

    public void createMesh()
    {
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

        List<Vector3> verts =new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        int index = 0;
        /*for (int x = 0; x < levelData.GetLength(0) + 1; x++)
        {
            for (int y = 0; y < levelData.GetLength(1) + 1; y++)
            {
                verts.Add(new Vector3(x, y));
            }
        }*/
        for (int x = 0; x < levelData.GetLength(0); x++)
        {
            for (int y = 0; y < levelData.GetLength(1); y++)
            {
                verts.Add(new Vector3(x, y));
                verts.Add(new Vector3(x+1, y));
                verts.Add(new Vector3(x, y + 1));
                verts.Add(new Vector3(x+1, y + 1));

                tris.Add(index);// x + (y + 1) * levelData.GetLength(0));
                tris.Add(index+1);//x + y * levelData.GetLength(0));
                tris.Add(index+2);//x + y * levelData.GetLength(0) + 1);

                tris.Add(index+1);//(x + (y + 1) * levelData.GetLength(0) + 1);
                tris.Add(index+3);//(x + 1 + y * levelData.GetLength(0));
                tris.Add(index+2);//(x + (y + 1) * levelData.GetLength(0));
                

                float tilePixelWidth = 8;
                float scaleX = tilePixelWidth / tileSet.width;
                float scaleY = tilePixelWidth / tileSet.height;
                int numberOfTilesX =(int)(tileSet.width / tilePixelWidth);
                int numberOfTilesY = (int)(tileSet.height / tilePixelWidth);
                float tileX = levelData[x,y] % numberOfTilesX;
                float tileY = (int)(levelData[x,y] / numberOfTilesY);
                uvs.Add(new Vector2(tileX * scaleX, tileY * scaleY));
                uvs.Add(new Vector2(tileX * scaleX+ scaleX, tileY * scaleY));
                uvs.Add(new Vector2(tileX * scaleX, tileY * scaleY+ scaleY));
                uvs.Add(new Vector2(tileX * scaleX + scaleX, tileY * scaleY + scaleY));
                
                index += 4;
            }
        }

        mesh.vertices = verts.ToArray();
        mesh.triangles=tris.ToArray();
        mesh.uv = uvs.ToArray();
        // mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        
    }

  
}
