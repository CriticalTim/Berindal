using UnityEngine;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    private Canvas titleCanvas;
    private GameObject titlePanel;
    private GameObject registrationPanel;
    private GameObject loginPanel;

    private RegistrationPanel registrationPanelScript;
    private LoginPanel loginPanelScript;

    private void Start()
    {
        // Ensure APIClient and SessionManager exist
        if (APIClient.Instance == null)
        {
            GameObject apiClient = new GameObject("API Client");
            apiClient.AddComponent<APIClient>();
        }

        if (SessionManager.Instance == null)
        {
            GameObject sessionManager = new GameObject("Session Manager");
            sessionManager.AddComponent<SessionManager>();
        }

        CreateTitleScreenUI();
        ShowTitlePanel();
    }

    private void CreateTitleScreenUI()
    {
        // Create canvas
        GameObject canvasObj = new GameObject("Title Screen Canvas");
        titleCanvas = canvasObj.AddComponent<Canvas>();
        titleCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        titleCanvas.sortingOrder = 1000;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        // Create main title panel
        titlePanel = CreateTitlePanel();

        // Create registration panel (initially hidden)
        registrationPanel = CreateRegistrationPanelObject();

        // Create login panel (initially hidden)
        loginPanel = CreateLoginPanelObject();
    }

    private GameObject CreateTitlePanel()
    {
        GameObject panel = CreatePanel("Title Panel", titleCanvas.transform);

        // Title text
        CreateText("Title Text", panel.transform, "BERINDAL RPG", new Vector2(0, 200), 60, Color.white);

        // Register button
        GameObject registerBtn = CreateButton("Register Button", panel.transform, "Register", new Vector2(0, 50));
        registerBtn.GetComponent<Button>().onClick.AddListener(OnRegisterButtonClick);

        // Login button
        GameObject loginBtn = CreateButton("Login Button", panel.transform, "Login", new Vector2(0, -50));
        loginBtn.GetComponent<Button>().onClick.AddListener(OnLoginButtonClick);

        // Exit button
        GameObject exitBtn = CreateButton("Exit Button", panel.transform, "Exit", new Vector2(0, -150));
        exitBtn.GetComponent<Button>().onClick.AddListener(OnExitButtonClick);

        return panel;
    }

    private GameObject CreateRegistrationPanelObject()
    {
        GameObject panel = CreatePanel("Registration Panel", titleCanvas.transform);
        registrationPanelScript = panel.AddComponent<RegistrationPanel>();
        registrationPanelScript.Initialize(this);
        panel.SetActive(false);
        return panel;
    }

    private GameObject CreateLoginPanelObject()
    {
        GameObject panel = CreatePanel("Login Panel", titleCanvas.transform);
        loginPanelScript = panel.AddComponent<LoginPanel>();
        loginPanelScript.Initialize(this);
        panel.SetActive(false);
        return panel;
    }

    private GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent);

        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);

        return panel;
    }

    private GameObject CreateText(string name, Transform parent, string text, Vector2 position, int fontSize, Color color)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent);

        RectTransform rt = textObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(800, fontSize + 20);

        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = fontSize;
        textComponent.color = color;
        textComponent.alignment = TextAnchor.MiddleCenter;

        return textObj;
    }

    public GameObject CreateButton(string name, Transform parent, string text, Vector2 position)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent);

        RectTransform rt = btnObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(300, 60);

        Image image = btnObj.AddComponent<Image>();
        image.color = new Color(0.3f, 0.3f, 0.3f, 1f);

        Button button = btnObj.AddComponent<Button>();
        button.targetGraphic = image;

        // Button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform);

        RectTransform textRT = textObj.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;

        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 24;
        textComponent.color = Color.white;
        textComponent.alignment = TextAnchor.MiddleCenter;

        return btnObj;
    }

    private void OnRegisterButtonClick()
    {
        ShowRegistrationPanel();
    }

    private void OnLoginButtonClick()
    {
        ShowLoginPanel();
    }

    private void OnExitButtonClick()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void ShowTitlePanel()
    {
        titlePanel.SetActive(true);
        registrationPanel.SetActive(false);
        loginPanel.SetActive(false);
    }

    private void ShowRegistrationPanel()
    {
        titlePanel.SetActive(false);
        registrationPanel.SetActive(true);
        loginPanel.SetActive(false);
    }

    private void ShowLoginPanel()
    {
        titlePanel.SetActive(false);
        registrationPanel.SetActive(false);
        loginPanel.SetActive(true);
    }

    public void OnSuccessfulLogin(CharacterDataDTO characterData)
    {
        Debug.Log($"Login successful for {characterData.username}, starting game...");

        // Destroy title screen
        Destroy(titleCanvas.gameObject);
        Destroy(gameObject);

        // Find RPGGame and start the game
        RPGGame rpgGame = FindObjectOfType<RPGGame>();
        if (rpgGame != null)
        {
            rpgGame.StartGameWithCharacter(characterData);
        }
        else
        {
            Debug.LogError("RPGGame not found!");
        }
    }
}
