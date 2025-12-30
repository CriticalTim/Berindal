using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Bars")]
    public Slider healthBar;
    public Slider staminaBar;
    public Slider magicBar;
    
    [Header("Bar Colors")]
    public Color healthColor = Color.red;
    public Color staminaColor = Color.yellow;
    public Color magicColor = Color.blue;
    
    private PlayerCharacter player;
    
    private void Start()
    {
        player = PlayerCharacter.Instance;
        SetupBars();
        UpdateUI();
    }
    
    private void SetupBars()
    {
        if (healthBar != null)
        {
            healthBar.fillRect.GetComponent<Image>().color = healthColor;
        }
        
        if (staminaBar != null)
        {
            staminaBar.fillRect.GetComponent<Image>().color = staminaColor;
        }
        
        if (magicBar != null)
        {
            magicBar.fillRect.GetComponent<Image>().color = magicColor;
        }
    }
    
    public void UpdateUI()
    {
        if (player == null) return;
        
        if (healthBar != null)
        {
            healthBar.maxValue = player.maxHealth;
            healthBar.value = player.currentHealth;
        }
        
        if (staminaBar != null)
        {
            staminaBar.maxValue = player.maxStamina;
            staminaBar.value = player.currentStamina;
        }
        
        if (magicBar != null)
        {
            magicBar.maxValue = player.maxMagic;
            magicBar.value = player.currentMagic;
        }
    }
}