using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class RegistrationPanel : MonoBehaviour
{
    private TitleScreenManager titleScreenManager;
    private InputField usernameInput;
    private InputField passwordInput;
    private InputField confirmPasswordInput;
    private Dropdown classDropdown;
    private Text errorText;
    private Button registerButton;
    private Button backButton;

    public void Initialize(TitleScreenManager manager)
    {
        titleScreenManager = manager;
        CreateRegistrationUI();
    }

    private void CreateRegistrationUI()
    {
        // Title
        CreateText("Registration Title", transform, "Create Account", new Vector2(0, 300), 48, Color.white);

        // Username field
        CreateLabel("Username Label", transform, "Username:", new Vector2(0, 200));
        usernameInput = CreateInputField("Username Input", transform, new Vector2(0, 170), "Enter username (3-20 characters)");

        // Password field
        CreateLabel("Password Label", transform, "Password:", new Vector2(0, 100));
        passwordInput = CreateInputField("Password Input", transform, new Vector2(0, 70), "Enter password (6+ characters)");
        passwordInput.contentType = InputField.ContentType.Password;

        // Confirm password field
        CreateLabel("Confirm Password Label", transform, "Confirm Password:", new Vector2(0, 0));
        confirmPasswordInput = CreateInputField("Confirm Password Input", transform, new Vector2(0, -30), "Confirm password");
        confirmPasswordInput.contentType = InputField.ContentType.Password;

        // Class dropdown
        CreateLabel("Class Label", transform, "Select Class:", new Vector2(0, -100));
        classDropdown = CreateDropdown("Class Dropdown", transform, new Vector2(0, -130));

        // Error text
        errorText = CreateText("Error Text", transform, "", new Vector2(0, -200), 20, Color.red);

        // Register button
        GameObject registerBtn = titleScreenManager.CreateButton("Register Button", transform, "Register", new Vector2(-100, -270));
        registerButton = registerBtn.GetComponent<Button>();
        registerButton.onClick.AddListener(OnRegisterClick);

        // Back button
        GameObject backBtn = titleScreenManager.CreateButton("Back Button", transform, "Back", new Vector2(100, -270));
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

    private Dropdown CreateDropdown(string name, Transform parent, Vector2 position)
    {
        GameObject dropdownObj = new GameObject(name);
        dropdownObj.transform.SetParent(parent);

        RectTransform rt = dropdownObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(400, 40);

        Image image = dropdownObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        Dropdown dropdown = dropdownObj.AddComponent<Dropdown>();

        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(dropdownObj.transform);
        RectTransform labelRT = labelObj.AddComponent<RectTransform>();
        labelRT.anchorMin = Vector2.zero;
        labelRT.anchorMax = Vector2.one;
        labelRT.offsetMin = new Vector2(10, 0);
        labelRT.offsetMax = new Vector2(-25, 0);

        Text labelText = labelObj.AddComponent<Text>();
        labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        labelText.fontSize = 18;
        labelText.color = Color.white;

        dropdown.captionText = labelText;

        // Add dropdown options
        dropdown.options.Clear();
        dropdown.options.Add(new Dropdown.OptionData("Warrior - High HP & Stamina"));
        dropdown.options.Add(new Dropdown.OptionData("Healer - High Magic"));
        dropdown.options.Add(new Dropdown.OptionData("Mage - Very High Magic"));

        // Template (required but not fully implemented for simplicity)
        GameObject templateObj = new GameObject("Template");
        templateObj.transform.SetParent(dropdownObj.transform);
        RectTransform templateRT = templateObj.AddComponent<RectTransform>();
        templateRT.anchorMin = new Vector2(0, 0);
        templateRT.anchorMax = new Vector2(1, 0);
        templateRT.pivot = new Vector2(0.5f, 1);
        templateRT.anchoredPosition = new Vector2(0, 2);
        templateRT.sizeDelta = new Vector2(0, 150);
        templateObj.SetActive(false);

        dropdown.template = templateRT;

        return dropdown;
    }

    private void OnRegisterClick()
    {
        errorText.text = "";

        if (!ValidateInputs(out string error))
        {
            errorText.text = error;
            return;
        }

        // Disable buttons during registration
        registerButton.interactable = false;
        backButton.interactable = false;
        errorText.text = "Registering...";
        errorText.color = Color.yellow;

        string playerClass = GetSelectedClass();

        APIClient.Instance.RegisterUser(
            usernameInput.text,
            passwordInput.text,
            playerClass,
            OnRegistrationComplete
        );
    }

    private void OnRegistrationComplete(bool success, LoginResponse response)
    {
        registerButton.interactable = true;
        backButton.interactable = true;

        if (success && response.success)
        {
            Debug.Log("Registration successful!");

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

        if (usernameInput.text.Length < 3 || usernameInput.text.Length > 20)
        {
            error = "Username must be 3-20 characters";
            return false;
        }

        if (!Regex.IsMatch(usernameInput.text, "^[a-zA-Z0-9]+$"))
        {
            error = "Username must be alphanumeric";
            return false;
        }

        if (string.IsNullOrWhiteSpace(passwordInput.text))
        {
            error = "Password is required";
            return false;
        }

        if (passwordInput.text.Length < 6)
        {
            error = "Password must be at least 6 characters";
            return false;
        }

        if (passwordInput.text != confirmPasswordInput.text)
        {
            error = "Passwords do not match";
            return false;
        }

        return true;
    }

    private string GetSelectedClass()
    {
        switch (classDropdown.value)
        {
            case 0: return "Warrior";
            case 1: return "Healer";
            case 2: return "Mage";
            default: return "Warrior";
        }
    }

    private void OnBackClick()
    {
        titleScreenManager.ShowTitlePanel();
    }
}
