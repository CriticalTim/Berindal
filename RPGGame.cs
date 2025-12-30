using UnityEngine;

public class RPGGame : MonoBehaviour
{
    [Header("Game Settings")]
    [Tooltip("Choose the player class for this game session")]
    public PlayerClass selectedClass = PlayerClass.Warrior;
    
    private void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
    }
    
    private void Start()
    {
        Debug.Log("=== 2D Pixelart RPG Starting ===");
        Debug.Log($"Player Class: {selectedClass}");
        Debug.Log("Click anywhere to move your character!");
        Debug.Log("Walk into enemies to attack them (requires stamina)");
        Debug.Log("Watch your health, stamina, and magic bars in the top right corner");
        
        GameObject gameManager = new GameObject("Game Manager");
        GameManager gm = gameManager.AddComponent<GameManager>();
        gm.startingClass = selectedClass;
    }
}

[System.Serializable]
public class GameInstructions
{
    [Header("Controls")]
    [TextArea(3, 5)]
    public string controls = 
        "• Click anywhere on screen to move\n" +
        "• Character stays centered, world moves around you\n" +
        "• Walk into enemies to attack (needs stamina)\n" +
        "• Move to quadrant edges to transition to new areas";
    
    [Header("Classes")]
    [TextArea(3, 5)]
    public string classes = 
        "• Warrior: High health & stamina, medium magic, strong attacks\n" +
        "• Healer: Medium health & stamina, high magic, weak attacks\n" +
        "• Mage: Low health & stamina, very high magic, medium attacks";
    
    [Header("Combat")]
    [TextArea(3, 5)]
    public string combat = 
        "• Evasion chance: 5% (low stamina) to 50% (full stamina)\n" +
        "• Undead Corps: 15 HP, 5 stamina, slow, 3 damage per hit\n" +
        "• Skeleton: 2 HP, 10 stamina, fast, 1 damage per hit";
}