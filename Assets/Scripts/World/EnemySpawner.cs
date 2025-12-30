using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject undeadCorpsPrefab;
    public GameObject skeletonPrefab;
    public int maxEnemiesPerQuadrant = 5;
    public float spawnRadius = 40f;
    public float minDistanceFromPlayer = 15f;
    
    private void Start()
    {
        CreateEnemyPrefabs();
        SpawnInitialEnemies();
    }
    
    private void CreateEnemyPrefabs()
    {
        if (undeadCorpsPrefab == null)
        {
            undeadCorpsPrefab = new GameObject("UndeadCorps");
            undeadCorpsPrefab.AddComponent<UndeadCorps>();
        }
        
        if (skeletonPrefab == null)
        {
            skeletonPrefab = new GameObject("Skeleton");
            skeletonPrefab.AddComponent<Skeleton>();
        }
    }
    
    private void SpawnInitialEnemies()
    {
        for (int i = 0; i < maxEnemiesPerQuadrant; i++)
        {
            Vector2 spawnPosition = GetValidSpawnPosition();
            
            if (Random.Range(0f, 1f) < 0.6f)
            {
                SpawnUndead(spawnPosition);
            }
            else
            {
                SpawnSkeleton(spawnPosition);
            }
        }
    }
    
    private Vector2 GetValidSpawnPosition()
    {
        Vector2 spawnPos;
        int attempts = 0;
        
        do
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(minDistanceFromPlayer, spawnRadius);
            
            spawnPos = new Vector2(
                Mathf.Cos(angle) * distance,
                Mathf.Sin(angle) * distance
            );
            
            attempts++;
        }
        while (attempts < 20 && Vector2.Distance(spawnPos, Vector2.zero) < minDistanceFromPlayer);
        
        return spawnPos;
    }
    
    private void SpawnUndead(Vector2 position)
    {
        GameObject undead = new GameObject("UndeadCorps");
        undead.transform.position = position;
        undead.AddComponent<UndeadCorps>();
    }
    
    private void SpawnSkeleton(Vector2 position)
    {
        GameObject skeleton = new GameObject("Skeleton");
        skeleton.transform.position = position;
        skeleton.AddComponent<Skeleton>();
    }
    
    public void SpawnEnemy(Vector2 position, bool isUndead = true)
    {
        if (isUndead)
        {
            SpawnUndead(position);
        }
        else
        {
            SpawnSkeleton(position);
        }
    }
}