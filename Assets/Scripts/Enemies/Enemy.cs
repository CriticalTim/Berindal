using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int maxHealth;
    public int currentHealth;
    public int maxStamina;
    public int currentStamina;
    public int attackDamage;
    public float moveSpeed;
    public float attackRange = 1f;
    
    [Header("AI Settings")]
    public float detectionRange = 10f;
    public float attackCooldown = 1f;
    
    protected PlayerCharacter player;
    protected float lastAttackTime;
    protected bool isAttacking = false;
    
    protected virtual void Start()
    {
        player = PlayerCharacter.Instance;
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }
    
    protected virtual void Update()
    {
        if (player == null || currentHealth <= 0) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        
        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer <= attackRange)
            {
                TryAttackPlayer();
            }
            else
            {
                MoveTowardPlayer();
            }
        }
    }
    
    protected virtual void MoveTowardPlayer()
    {
        if (currentStamina <= 0) return;
        
        Vector2 direction = (player.transform.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }
    
    protected virtual void TryAttackPlayer()
    {
        if (Time.time >= lastAttackTime + attackCooldown && currentStamina > 0)
        {
            AttackPlayer();
            lastAttackTime = Time.time;
            currentStamina--;
        }
    }
    
    protected virtual void AttackPlayer()
    {
        float playerEvasionChance = player.GetEvasionChance();
        
        if (Random.Range(0f, 1f) > playerEvasionChance)
        {
            player.TakeDamage(attackDamage);
            Debug.Log($"{gameObject.name} hit player for {attackDamage} damage!");
        }
        else
        {
            Debug.Log($"Player evaded {gameObject.name}'s attack!");
        }
    }
    
    public virtual void TakeDamage(int damage)
    {
        float evasionChance = GetEvasionChance();
        
        if (Random.Range(0f, 1f) > evasionChance)
        {
            currentHealth -= damage;
            Debug.Log($"{gameObject.name} took {damage} damage! Health: {currentHealth}/{maxHealth}");
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        else
        {
            Debug.Log($"{gameObject.name} evaded the attack!");
        }
    }
    
    protected virtual float GetEvasionChance()
    {
        float staminaRatio = (float)currentStamina / maxStamina;
        return Mathf.Lerp(0.05f, 0.5f, staminaRatio);
    }
    
    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        Destroy(gameObject);
    }
    
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}