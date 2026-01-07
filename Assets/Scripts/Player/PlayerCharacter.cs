using UnityEngine;

public enum PlayerClass
{
    Warrior,
    Healer,
    Mage
}

public class PlayerCharacter : MonoBehaviour
{
    [Header("Player Stats")]
    public PlayerClass playerClass = PlayerClass.Warrior;
    public int maxHealth = 10;
    public int maxStamina = 10;
    public int maxMagic = 10;
    
    [Header("Current Stats")]
    public int currentHealth;
    public int currentStamina;
    public int currentMagic;
    
    [Header("Movement")]
    public float moveSpeed = 5f;
    public bool isMoving = false;

    private Vector2 targetPosition;
    private Camera playerCamera;
    private UIManager uiManager;

    // Auto-save
    private float saveTimer = 0f;
    private const float SAVE_INTERVAL = 30f; // Save every 30 seconds

    public static PlayerCharacter Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        InitializePlayer();
        playerCamera = Camera.main;
        uiManager = FindObjectOfType<UIManager>();
        targetPosition = transform.position;
    }
    
    private void Update()
    {
        HandleMovement();
        HandleInput();

        // Auto-save character state every 30 seconds
        if (SessionManager.Instance != null && SessionManager.Instance.IsLoggedIn)
        {
            saveTimer += Time.deltaTime;
            if (saveTimer >= SAVE_INTERVAL)
            {
                SaveCharacterToServer();
                saveTimer = 0f;
            }
        }
    }
    
    private void InitializePlayer()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentMagic = maxMagic;
        
        switch (playerClass)
        {
            case PlayerClass.Warrior:
                maxHealth = 15;
                maxStamina = 12;
                maxMagic = 5;
                break;
            case PlayerClass.Healer:
                maxHealth = 8;
                maxStamina = 8;
                maxMagic = 15;
                break;
            case PlayerClass.Mage:
                maxHealth = 6;
                maxStamina = 6;
                maxMagic = 18;
                break;
        }
        
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentMagic = maxMagic;
    }
    
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = playerCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, playerCamera.nearClipPlane));
            worldPos.z = 0f;
            
            SetTargetPosition(worldPos);
        }
    }
    
    private void SetTargetPosition(Vector2 newTarget)
    {
        targetPosition = newTarget;
        isMoving = true;
    }
    
    private void HandleMovement()
    {
        if (isMoving)
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            float distance = Vector2.Distance(transform.position, targetPosition);
            
            if (distance > 0.1f)
            {
                transform.Translate(direction * moveSpeed * Time.deltaTime);
                
                Vector2 worldOffset = direction * moveSpeed * Time.deltaTime;
                WorldManager.Instance?.MoveWorld(-worldOffset);
            }
            else
            {
                isMoving = false;
            }
        }
    }
    
    public float GetEvasionChance()
    {
        float staminaRatio = (float)currentStamina / maxStamina;
        return Mathf.Lerp(0.05f, 0.5f, staminaRatio);
    }
    
    public bool TryAttack()
    {
        if (currentStamina > 0)
        {
            currentStamina--;
            uiManager?.UpdateUI();
            return true;
        }
        return false;
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        uiManager?.UpdateUI();
    }
    
    public void RestoreStamina(int amount)
    {
        currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
        uiManager?.UpdateUI();
    }
    
    private void Die()
    {
        Debug.Log("Player died!");

        // Save state before death
        if (SessionManager.Instance != null && SessionManager.Instance.IsLoggedIn)
        {
            SaveCharacterToServer();
        }
    }

    public CharacterStateDTO GetCharacterState()
    {
        return new CharacterStateDTO
        {
            currentHealth = this.currentHealth,
            currentStamina = this.currentStamina,
            currentMagic = this.currentMagic,
            positionX = transform.position.x,
            positionY = transform.position.y,
            worldOffsetX = 0f, // WorldManager offset can be added later if needed
            worldOffsetY = 0f
        };
    }

    private void SaveCharacterToServer()
    {
        if (APIClient.Instance == null)
        {
            Debug.LogWarning("APIClient not available for saving");
            return;
        }

        CharacterStateDTO state = GetCharacterState();
        string token = SessionManager.Instance.GetAuthToken();

        APIClient.Instance.SaveCharacterState(token, state, (success, message) =>
        {
            if (success)
            {
                Debug.Log("Character state saved successfully");
            }
            else
            {
                Debug.LogWarning($"Failed to save character state: {message}");
            }
        });
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && TryAttack())
        {
            enemy.TakeDamage(GetAttackDamage());
        }
    }
    
    private int GetAttackDamage()
    {
        switch (playerClass)
        {
            case PlayerClass.Warrior:
                return 4;
            case PlayerClass.Healer:
                return 2;
            case PlayerClass.Mage:
                return 3;
            default:
                return 3;
        }
    }
}