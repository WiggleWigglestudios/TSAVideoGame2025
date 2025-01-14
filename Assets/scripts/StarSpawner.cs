using UnityEngine;

public class StarSpawner : MonoBehaviour
{
    public GameObject starPrefab; 
    public float spawnRate = 0.1f; 
    public float fallSpeed = 1f; 
    public float screenWidth = 10f; 

    void Start()
    {
        InvokeRepeating(nameof(SpawnStar), 0f, spawnRate);
    }

    void SpawnStar()
    {
        float randomX = Random.Range(-screenWidth / 2, screenWidth / 2);
        Vector3 spawnPosition = new Vector3(randomX, transform.position.y, 0);
        
        GameObject star = Instantiate(starPrefab, spawnPosition, Quaternion.identity);
        
        StarMover starMover = star.AddComponent<StarMover>();
        starMover.fallSpeed = fallSpeed; 
        
        Destroy(star, 5f);
    }
}
