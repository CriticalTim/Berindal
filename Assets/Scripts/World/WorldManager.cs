using UnityEngine;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{
    [Header("World Settings")]
    public float quadrantSize = 100f;
    public Vector2 currentQuadrant = Vector2.zero;
    
    [Header("World Objects")]
    public Transform worldContainer;
    public GameObject grassTilePrefab;
    public GameObject stonePrefab;
    public GameObject firePrefab;
    public GameObject treePrefab;
    
    private Dictionary<Vector2, GameObject> loadedQuadrants = new Dictionary<Vector2, GameObject>();
    private Vector2 worldOffset = Vector2.zero;
    
    public static WorldManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        if (worldContainer == null)
        {
            GameObject container = new GameObject("WorldContainer");
            worldContainer = container.transform;
        }
        
        LoadQuadrant(currentQuadrant);
    }
    
    public void MoveWorld(Vector2 offset)
    {
        worldOffset += offset;
        worldContainer.position += (Vector3)offset;
        
        CheckQuadrantTransition();
    }
    
    private void CheckQuadrantTransition()
    {
        Vector2 playerWorldPos = (Vector2)PlayerCharacter.Instance.transform.position + worldOffset;
        Vector2 newQuadrant = new Vector2(
            Mathf.Floor(playerWorldPos.x / quadrantSize),
            Mathf.Floor(playerWorldPos.y / quadrantSize)
        );
        
        if (newQuadrant != currentQuadrant)
        {
            TransitionToQuadrant(newQuadrant);
        }
    }
    
    private void TransitionToQuadrant(Vector2 newQuadrant)
    {
        UnloadQuadrant(currentQuadrant);
        currentQuadrant = newQuadrant;
        LoadQuadrant(currentQuadrant);
        
        Vector2 quadrantCenter = currentQuadrant * quadrantSize + Vector2.one * (quadrantSize * 0.5f);
        worldOffset = -quadrantCenter;
        worldContainer.position = (Vector3)worldOffset;
        
        PlayerCharacter.Instance.transform.position = Vector3.zero;
    }
    
    private void LoadQuadrant(Vector2 quadrant)
    {
        if (loadedQuadrants.ContainsKey(quadrant))
            return;
        
        GameObject quadrantObj = new GameObject($"Quadrant_{quadrant.x}_{quadrant.y}");
        quadrantObj.transform.parent = worldContainer;
        
        GenerateQuadrant(quadrantObj.transform, quadrant);
        loadedQuadrants[quadrant] = quadrantObj;
    }
    
    private void UnloadQuadrant(Vector2 quadrant)
    {
        if (loadedQuadrants.ContainsKey(quadrant))
        {
            Destroy(loadedQuadrants[quadrant]);
            loadedQuadrants.Remove(quadrant);
        }
    }
    
    private void GenerateQuadrant(Transform parent, Vector2 quadrant)
    {
        GenerateGrassBackground(parent);
        
        if (quadrant == Vector2.zero)
        {
            GenerateFirstQuadrant(parent);
        }
        else
        {
            GenerateRandomQuadrant(parent);
        }
    }
    
    private void GenerateGrassBackground(Transform parent)
    {
        GameObject background = new GameObject("GrassBackground");
        background.transform.parent = parent;
        
        SpriteRenderer sr = background.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.2f, 0.8f, 0.2f, 1f);
        sr.sprite = CreateColorSprite(Color.green);
        sr.size = new Vector2(quadrantSize, quadrantSize);
        sr.sortingOrder = -10;
    }
    
    private void GenerateFirstQuadrant(Transform parent)
    {
        Vector2 center = Vector2.zero;
        
        CreateStoneCircle(parent, center, 15f);
        CreateFireplace(parent, center);
        
        for (int i = 0; i < 8; i++)
        {
            Vector2 treePos = new Vector2(
                Random.Range(-40f, 40f),
                Random.Range(-40f, 40f)
            );
            
            if (Vector2.Distance(treePos, center) > 20f)
            {
                CreateTree(parent, treePos);
            }
        }
    }
    
    private void GenerateRandomQuadrant(Transform parent)
    {
        for (int i = 0; i < Random.Range(5, 15); i++)
        {
            Vector2 treePos = new Vector2(
                Random.Range(-45f, 45f),
                Random.Range(-45f, 45f)
            );
            CreateTree(parent, treePos);
        }
    }
    
    private void CreateStoneCircle(Transform parent, Vector2 center, float radius)
    {
        int stoneCount = 16;
        for (int i = 0; i < stoneCount; i++)
        {
            float angle = (float)i / stoneCount * Mathf.PI * 2f;
            Vector2 stonePos = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            
            GameObject stone = new GameObject("Stone");
            stone.transform.parent = parent;
            stone.transform.position = stonePos;
            
            SpriteRenderer sr = stone.AddComponent<SpriteRenderer>();
            sr.color = Color.gray;
            sr.sprite = CreateColorSprite(Color.gray);
            sr.size = Vector2.one * 2f;
            
            Collider2D col = stone.AddComponent<BoxCollider2D>();
            col.isTrigger = false;
        }
    }
    
    private void CreateFireplace(Transform parent, Vector2 position)
    {
        GameObject fire = new GameObject("Fireplace");
        fire.transform.parent = parent;
        fire.transform.position = position;
        
        SpriteRenderer sr = fire.AddComponent<SpriteRenderer>();
        sr.color = Color.red;
        sr.sprite = CreateColorSprite(Color.red);
        sr.size = Vector2.one * 1.5f;
        sr.sortingOrder = 1;
    }
    
    private void CreateTree(Transform parent, Vector2 position)
    {
        GameObject tree = new GameObject("Tree");
        tree.transform.parent = parent;
        tree.transform.position = position;
        
        SpriteRenderer sr = tree.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.4f, 0.2f, 0.1f);
        sr.sprite = CreateColorSprite(new Color(0.4f, 0.2f, 0.1f));
        sr.size = Vector2.one * 3f;
        sr.sortingOrder = 1;
        
        Collider2D col = tree.AddComponent<BoxCollider2D>();
        col.isTrigger = false;
    }
    
    private Sprite CreateColorSprite(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);
    }
}