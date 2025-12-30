using UnityEngine;

public class Skeleton : Enemy
{
    protected override void Start()
    {
        maxHealth = 2;
        maxStamina = 10;
        attackDamage = 1;
        moveSpeed = 3f;
        attackRange = 1f;
        detectionRange = 12f;
        attackCooldown = 0.8f;
        
        base.Start();
        
        SetupVisuals();
    }
    
    private void SetupVisuals()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = gameObject.AddComponent<SpriteRenderer>();
        
        sr.color = Color.white;
        sr.sprite = CreateEnemySprite(Color.white);
        sr.size = Vector2.one * 1.5f;
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
        base.MoveTowardPlayer();
        
        if (Random.Range(0f, 1f) < 0.05f)
        {
            currentStamina = Mathf.Min(currentStamina + 1, maxStamina);
        }
    }
    
    protected override void TryAttackPlayer()
    {
        base.TryAttackPlayer();
        
        if (currentStamina <= 2)
        {
            currentStamina = Mathf.Min(currentStamina + 2, maxStamina);
        }
    }
}