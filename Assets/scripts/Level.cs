using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System;


public class Level : MonoBehaviour
{

    public Texture2D tileSet;
    public int[,] levelData;
    public int[] translationTable;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public Vector2[] boundsOfLevel;
    public string levelDataFilePath;
    public string translationDataFilePath;

    public GameObject collapseTilePrefab;

    public bool collapsed = false;

    public int checkPointHits = 0;

    public Level lastLevel;
    // Audio
    public AudioSource audioSource;
    public AudioClip collapseNoise;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        recreateMesh();
    }

    private void Update()
    {
        if (checkPointHits==2&&lastLevel!=null)
        {
            lastLevel.collapse();
            // Wait three seconds
            StartCoroutine(PlayCollapseSoundWithDelay()); // Start the coroutine
            checkPointHits++; // Modifed for audio to play once
        }
    }
    private System.Collections.IEnumerator PlayCollapseSoundWithDelay()
    {
        yield return new WaitForSeconds(3f); // Wait for 3 seconds
        audioSource.PlayOneShot(collapseNoise); // Play the collapse noise
    }


    public void loadData() 
    {
        levelData = new int[32, 32];
        
        if (File.Exists(levelDataFilePath))
        {

            byte[] data = File.ReadAllBytes(levelDataFilePath);
            int indexX = 0;
            int indexY = 0;
            string currentStringNumber = "";
            for (int i = 0; i < data.Length; i++)
            {

                if (data[i] < 32 || data[i] == 44)
                {
                    if (currentStringNumber.Length > 0)
                    {
                        levelData[indexX,31-indexY]=(int.Parse(currentStringNumber));



                        indexX++;
                        if (indexX >= 32)
                        {
                            indexX = 0;
                            indexY++;
                            if (indexY >= 32)
                            {
                                i = data.Length;
                            }
                        }
                    }
                    currentStringNumber = "";
                }
                else
                {
                    currentStringNumber += (char)data[i];
                }
            }
            



        }
        else {
            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    levelData[x, y] = UnityEngine.Random.Range(0, 32);
                }
            }
        }


        if (File.Exists(translationDataFilePath))
        {
            byte[] data = File.ReadAllBytes(translationDataFilePath);
            string currentStringNumber = "";
            List<int> newTranslationData = new List<int>();
            for (int i = 0; i < data.Length; i++)
            {

                if (data[i] < 32 || data[i] == 44)
                {
                    if (currentStringNumber.Length > 0)
                    {
                        newTranslationData.Add(int.Parse(currentStringNumber));
                    }
                    currentStringNumber = "";
                }
                else
                {
                    currentStringNumber += (char)data[i];
                }
            }
            translationTable = newTranslationData.ToArray();
        }
        else {
            translationTable = new int[256];
            
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
                int numberOfTilesX = (int)((tileSet.width - initialPadding * 2) / (pixelsPerTile + pixelsOfPadding * 2));
                int numberOfTilesY = (int)((tileSet.height - initialPadding * 2) / (pixelsPerTile + pixelsOfPadding * 2));
                float tileX = levelData[x, y] % numberOfTilesX;
                float tileY = (int)(levelData[x, y] / numberOfTilesX);
               
                tileX = tileX * pixelsPerTile + initialPadding + pixelsOfPadding * tileX + pixelsOfPadding * (tileX+1);
                tileY = tileY * pixelsPerTile + initialPadding + pixelsOfPadding * tileY + pixelsOfPadding * (tileY + 1);
                tileX /= tileSet.width;
                tileY /= tileSet.height;

                uvs.Add(new Vector2((tileX), 1.0f-(tileY + pixelsPerTile / tileSet.height)));
                uvs.Add(new Vector2((tileX + pixelsPerTile / tileSet.width), 1.0f -(tileY + pixelsPerTile / tileSet.height)));
                uvs.Add(new Vector2((tileX), 1.0f - (tileY)));
                uvs.Add(new Vector2((tileX + pixelsPerTile / tileSet.width), 1.0f - (tileY)));

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

        boundsOfLevel = new Vector2[2];
        boundsOfLevel[0] = new Vector2(transform.position.x, transform.position.y);
        boundsOfLevel[1] = new Vector2(transform.position.x+32, transform.position.y+32);

    }


    public void collapse() 
    {
        if (collapsed != true)
        {
            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    GameObject newCollapsedTile = Instantiate(collapseTilePrefab, transform.position + new Vector3(x, y) + new Vector3(0.5f, 0.5f), Quaternion.identity);

                    newCollapsedTile.GetComponent<CollapseTile>().initializeCollapseTile(tileSet, levelData[x, y], 2 + y / 32.0f);
                }
            }

            Camera[] allCameras = Camera.allCameras;
            for (int i = 0; i < allCameras.Length; i++)
            {
                allCameras[i].GetComponent<CameraFollow>().shakeTimer = 4.0f;
            }
            meshRenderer.enabled = false;
        }
        
        collapsed = true;
    }

}
