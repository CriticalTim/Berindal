using UnityEngine;

public class UndeadCorps : Enemy
{
    protected override void Start()
    {
        maxHealth = 15;
        maxStamina = 5;
        attackDamage = 3;
        moveSpeed = 1f;
        attackRange = 1.2f;
        detectionRange = 8f;
        attackCooldown = 2f;
        
        base.Start();
        
        SetupVisuals();
    }
    
    private void SetupVisuals()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = gameObject.AddComponent<SpriteRenderer>();
        
        sr.color = new Color(0.3f, 0.3f, 0.2f);
        sr.sprite = CreateEnemySprite(new Color(0.3f, 0.3f, 0.2f));
        sr.size = Vector2.one * 2f;
        sr.sortingOrder = 2;
        
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
        }
    }
    
    private Sprite CreateEnemySprite(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);
    }
    
    protected override void MoveTowardPlayer()
    {
        if (currentStamina <= 0)
        {
            RestoreStamina();
            return;
        }
        
        base.MoveTowardPlayer();
    }
    
    private void RestoreStamina()
    {
        if (Random.Range(0f, 1f) < 0.1f)
        {
            currentStamina = Mathf.Min(currentStamina + 1, maxStamina);
        }
    }
}