using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level : MonoBehaviour
{

    public Texture2D tileSet;
    public int[,] levelData;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public Vector2Int[] boundsOfLevel;

    public void loadData() 
    {
        levelData = new int[32, 32];
        for(int x=0;x<32; x++)
        {
            for (int y = 0; y < 32; y++)
            {
                levelData[x, y] = Random.Range(0,32);
            }
        }
    }


    public void recreateMesh() 
    {

        if (levelData == null)
        {
            loadData();

        }

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
                verts.Add(new Vector3(x + 1, y));
                verts.Add(new Vector3(x, y + 1));
                verts.Add(new Vector3(x + 1, y + 1));

                tris.Add(index);// x + (y + 1) * levelData.GetLength(0));
                tris.Add(index + 1);//x + y * levelData.GetLength(0));
                tris.Add(index + 2);//x + y * levelData.GetLength(0) + 1);

                tris.Add(index + 1);//(x + (y + 1) * levelData.GetLength(0) + 1);
                tris.Add(index + 3);//(x + 1 + y * levelData.GetLength(0));
                tris.Add(index + 2);//(x + (y + 1) * levelData.GetLength(0));


                float pixelsPerTile = 80;
                float pixelsOfPadding = 10;
                float initialPadding = 0;
                int numberOfTilesX = (int)((tileSet.width - initialPadding*2) / (pixelsPerTile+ pixelsOfPadding*2));
                int numberOfTilesY = (int)((tileSet.height - initialPadding * 2) / (pixelsPerTile + pixelsOfPadding*2));
                float tileX = levelData[x, y] % numberOfTilesX;
                float tileY = (int)(levelData[x, y] / numberOfTilesY);
                tileX = tileX * pixelsPerTile + initialPadding + pixelsOfPadding * tileX + pixelsOfPadding * (tileX+1);
                tileY = tileY * pixelsPerTile + initialPadding + pixelsOfPadding * tileY + pixelsOfPadding * (tileY + 1);
                tileX /= tileSet.width;
                tileY /= tileSet.height;

                uvs.Add(new Vector2(tileX, tileY));
                uvs.Add(new Vector2(tileX + pixelsPerTile / tileSet.width, tileY));
                uvs.Add(new Vector2(tileX, tileY + pixelsPerTile / tileSet.height));
                uvs.Add(new Vector2(tileX + pixelsPerTile / tileSet.width, tileY + pixelsPerTile / tileSet.height));

                index += 4;
            }
        }

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();
        // mesh.RecalculateNormals();
        meshFilter.mesh = mesh;


        Material tileMapMat = new Material(Shader.Find("Sprites/Default"));
        tileMapMat.mainTexture = tileSet;

        meshRenderer.material = tileMapMat;
    }
}
