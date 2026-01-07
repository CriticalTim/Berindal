using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    private TitleScreenManager titleScreenManager;
    private InputField usernameInput;
    private InputField passwordInput;
    private Text errorText;
    private Button loginButton;
    private Button backButton;

    public void Initialize(TitleScreenManager manager)
    {
        titleScreenManager = manager;
        CreateLoginUI();
    }

    private void CreateLoginUI()
    {
        // Title
        CreateText("Login Title", transform, "Login", new Vector2(0, 250), 48, Color.white);

        // Username field
        CreateLabel("Username Label", transform, "Username:", new Vector2(0, 150));
        usernameInput = CreateInputField("Username Input", transform, new Vector2(0, 120), "Enter username");

        // Password field
        CreateLabel("Password Label", transform, "Password:", new Vector2(0, 50));
        passwordInput = CreateInputField("Password Input", transform, new Vector2(0, 20), "Enter password");
        passwordInput.contentType = InputField.ContentType.Password;

        // Error text
        errorText = CreateText("Error Text", transform, "", new Vector2(0, -50), 20, Color.red);

        // Login button
        GameObject loginBtn = titleScreenManager.CreateButton("Login Button", transform, "Login", new Vector2(-100, -150));
        loginButton = loginBtn.GetComponent<Button>();
        loginButton.onClick.AddListener(OnLoginClick);

        // Back button
        GameObject backBtn = titleScreenManager.CreateButton("Back Button", transform, "Back", new Vector2(100, -150));
        backButton = backBtn.GetComponent<Button>();
        backButton.onClick.AddListener(OnBackClick);
    }

    private void CreateLabel(string name, Transform parent, string text, Vector2 position)
    {
        GameObject labelObj = new GameObject(name);
        labelObj.transform.SetParent(parent);

        RectTransform rt = labelObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(400, 30);

        Text textComponent = labelObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 20;
        textComponent.color = Color.white;
        textComponent.alignment = TextAnchor.MiddleLeft;
    }

    private Text CreateText(string name, Transform parent, string text, Vector2 position, int fontSize, Color color)
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

        return textComponent;
    }

    private InputField CreateInputField(string name, Transform parent, Vector2 position, string placeholder)
    {
        GameObject inputObj = new GameObject(name);
        inputObj.transform.SetParent(parent);

        RectTransform rt = inputObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(400, 40);

        Image image = inputObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        InputField inputField = inputObj.AddComponent<InputField>();

        // Text component
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(inputObj.transform);
        RectTransform textRT = textObj.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = new Vector2(10, 0);
        textRT.offsetMax = new Vector2(-10, 0);

        Text text = textObj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 18;
        text.color = Color.white;
        text.supportRichText = false;

        // Placeholder
        GameObject placeholderObj = new GameObject("Placeholder");
        placeholderObj.transform.SetParent(inputObj.transform);
        RectTransform placeholderRT = placeholderObj.AddComponent<RectTransform>();
        placeholderRT.anchorMin = Vector2.zero;
        placeholderRT.anchorMax = Vector2.one;
        placeholderRT.offsetMin = new Vector2(10, 0);
        placeholderRT.offsetMax = new Vector2(-10, 0);

        Text placeholderText = placeholderObj.AddComponent<Text>();
        placeholderText.text = placeholder;
        placeholderText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        placeholderText.fontSize = 18;
        placeholderText.color = new Color(0.7f, 0.7f, 0.7f, 0.5f);
        placeholderText.fontStyle = FontStyle.Italic;

        inputField.textComponent = text;
        inputField.placeholder = placeholderText;

        return inputField;
    }

    private void OnLoginClick()
    {
        errorText.text = "";

        if (!ValidateInputs(out string error))
        {
            errorText.text = error;
            return;
        }

        // Disable buttons during login
        loginButton.interactable = false;
        backButton.interactable = false;
        errorText.text = "Logging in...";
        errorText.color = Color.yellow;

        APIClient.Instance.LoginUser(
            usernameInput.text,
            passwordInput.text,
            OnLoginComplete
        );
    }

    private void OnLoginComplete(bool success, LoginResponse response)
    {
        loginButton.interactable = true;
        backButton.interactable = true;

        if (success && response.success)
        {
            Debug.Log("Login successful!");

            // Store session
            SessionManager.Instance.SetSession(response.token, response.characterData);

            // Start game
            titleScreenManager.OnSuccessfulLogin(response.characterData);
        }
        else
        {
            errorText.text = response.message;
            errorText.color = Color.red;
        }
    }

    private bool ValidateInputs(out string error)
    {
        error = "";

        if (string.IsNullOrWhiteSpace(usernameInput.text))
        {
            error = "Username is required";
            return false;
        }

        if (string.IsNullOrWhiteSpace(passwordInput.text))
        {
            error = "Password is required";
            return false;
        }

        return true;
    }

    private void OnBackClick()
    {
        titleScreenManager.ShowTitlePanel();
    }
}
