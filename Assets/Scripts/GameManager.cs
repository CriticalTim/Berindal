using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI Setup")]
    public Canvas uiCanvas;
    public GameObject uiBarPrefab;
    
    [Header("Player Setup")]
    public GameObject playerPrefab;
    public PlayerClass startingClass = PlayerClass.Warrior;
    
    [Header("World Setup")]
    public GameObject worldManagerPrefab;
    public GameObject enemySpawnerPrefab;
    
    private void Start()
    {
        SetupScene();
    }
    
    private void SetupScene()
    {
        SetupCamera();
        SetupPlayer();
        SetupUI();
        SetupWorld();
        SetupEnemySpawning();
    }

    public void RestoreCharacterState(CharacterStateDTO state)
    {
        // Wait a frame for player to be fully initialized
        StartCoroutine(RestoreStateNextFrame(state));
    }

    private System.Collections.IEnumerator RestoreStateNextFrame(CharacterStateDTO state)
    {
        yield return null;

        if (PlayerCharacter.Instance != null)
        {
            PlayerCharacter.Instance.currentHealth = state.currentHealth;
            PlayerCharacter.Instance.currentStamina = state.currentStamina;
            PlayerCharacter.Instance.currentMagic = state.currentMagic;
            PlayerCharacter.Instance.transform.position = new Vector3(state.positionX, state.positionY, 0);

            // Update UI to reflect restored stats
            UIManager uiManager = FindObjectOfType<UIManager>();
            uiManager?.UpdateUI();

            Debug.Log($"Character state restored: HP={state.currentHealth}, Stamina={state.currentStamina}, Magic={state.currentMagic}");
        }
    }
    
    private void SetupCamera()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            mainCam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
        }
        
        mainCam.transform.position = new Vector3(0, 0, -10);
        mainCam.orthographic = true;
        mainCam.orthographicSize = 25f;
    }
    
    private void SetupPlayer()
    {
        if (PlayerCharacter.Instance == null)
        {
            GameObject player = new GameObject("Player");
            PlayerCharacter pc = player.AddComponent<PlayerCharacter>();
            pc.playerClass = startingClass;
            
            SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
            sr.sprite = CreatePlayerSprite();
            sr.color = GetPlayerColor(startingClass);
            sr.size = Vector2.one * 2f;
            sr.sortingOrder = 10;
            
            Collider2D col = player.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            ((CircleCollider2D)col).radius = 0.6f;
            
            player.transform.position = Vector3.zero;
        }
    }
    
    private void SetupUI()
    {
        if (uiCanvas == null)
        {
            GameObject canvasObj = new GameObject("UI Canvas");
            uiCanvas = canvasObj.AddComponent<Canvas>();
            uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            uiCanvas.sortingOrder = 100;
            
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        GameObject uiManagerObj = new GameObject("UI Manager");
        UIManager uiManager = uiManagerObj.AddComponent<UIManager>();
        
        CreateHealthBar(uiManager);
        CreateStaminaBar(uiManager);
        CreateMagicBar(uiManager);
    }
    
    private void CreateHealthBar(UIManager uiManager)
    {
        GameObject healthBarObj = CreateUIBar("Health Bar", new Vector2(-120, -50), Color.red);
        uiManager.healthBar = healthBarObj.GetComponent<Slider>();
    }
    
    private void CreateStaminaBar(UIManager uiManager)
    {
        GameObject staminaBarObj = CreateUIBar("Stamina Bar", new Vector2(-120, -90), Color.yellow);
        uiManager.staminaBar = staminaBarObj.GetComponent<Slider>();
    }
    
    private void CreateMagicBar(UIManager uiManager)
    {
        GameObject magicBarObj = CreateUIBar("Magic Bar", new Vector2(-120, -130), Color.blue);
        uiManager.magicBar = magicBarObj.GetComponent<Slider>();
    }
    
    private GameObject CreateUIBar(string name, Vector2 position, Color color)
    {
        GameObject barObj = new GameObject(name);
        barObj.transform.SetParent(uiCanvas.transform);
        
        RectTransform rt = barObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(1, 1);
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(200, 20);
        
        Slider slider = barObj.AddComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 10;
        slider.value = 10;
        
        GameObject background = new GameObject("Background");
        background.transform.SetParent(barObj.transform);
        RectTransform bgRT = background.AddComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = Color.gray;
        slider.targetGraphic = bgImage;
        
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(barObj.transform);
        RectTransform fillRT = fillArea.AddComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.offsetMin = Vector2.zero;
        fillRT.offsetMax = Vector2.zero;
        
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform);
        RectTransform fillImageRT = fill.AddComponent<RectTransform>();
        fillImageRT.anchorMin = Vector2.zero;
        fillImageRT.anchorMax = Vector2.one;
        fillImageRT.offsetMin = Vector2.zero;
        fillImageRT.offsetMax = Vector2.zero;
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = color;
        
        slider.fillRect = fillImageRT;
        
        return barObj;
    }
    
    private void SetupWorld()
    {
        GameObject worldManager = new GameObject("World Manager");
        worldManager.AddComponent<WorldManager>();
    }
    
    private void SetupEnemySpawning()
    {
        GameObject enemySpawner = new GameObject("Enemy Spawner");
        enemySpawner.AddComponent<EnemySpawner>();
    }
    
    private Sprite CreatePlayerSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);
    }
    
    private Color GetPlayerColor(PlayerClass playerClass)
    {
        switch (playerClass)
        {
            case PlayerClass.Warrior:
                return Color.red;
            case PlayerClass.Healer:
                return Color.green;
            case PlayerClass.Mage:
                return Color.blue;
            default:
                return Color.white;
        }
    }
}