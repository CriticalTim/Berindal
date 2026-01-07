using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class APIClient : MonoBehaviour
{
    // Replace this with your Railway URL after deployment
    private const string BASE_URL = "http://localhost:5000/api";
    // Production example: "https://your-app.up.railway.app/api"

    public static APIClient Instance { get; private set; }

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

    public void RegisterUser(string username, string password, string playerClass, Action<bool, LoginResponse> callback)
    {
        StartCoroutine(RegisterUserCoroutine(username, password, playerClass, callback));
    }

    public void LoginUser(string username, string password, Action<bool, LoginResponse> callback)
    {
        StartCoroutine(LoginUserCoroutine(username, password, callback));
    }

    public void LogoutUser(string token, Action<bool, string> callback)
    {
        StartCoroutine(LogoutUserCoroutine(token, callback));
    }

    public void SaveCharacterState(string token, CharacterStateDTO state, Action<bool, string> callback)
    {
        StartCoroutine(SaveCharacterStateCoroutine(token, state, callback));
    }

    public void LoadCharacterData(string token, Action<bool, CharacterDataDTO> callback)
    {
        StartCoroutine(LoadCharacterDataCoroutine(token, callback));
    }

    private IEnumerator RegisterUserCoroutine(string username, string password, string playerClass, Action<bool, LoginResponse> callback)
    {
        RegisterRequest request = new RegisterRequest
        {
            username = username,
            password = password,
            playerClass = playerClass
        };

        string json = JsonUtility.ToJson(request);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest www = new UnityWebRequest($"{BASE_URL}/auth/register", "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.timeout = 30;

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    LoginResponse response = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);
                    callback?.Invoke(response.success, response);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to parse registration response: {ex.Message}");
                    callback?.Invoke(false, new LoginResponse { success = false, message = "Failed to parse server response" });
                }
            }
            else
            {
                HandleError(www, callback);
            }
        }
    }

    private IEnumerator LoginUserCoroutine(string username, string password, Action<bool, LoginResponse> callback)
    {
        LoginRequest request = new LoginRequest
        {
            username = username,
            password = password
        };

        string json = JsonUtility.ToJson(request);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest www = new UnityWebRequest($"{BASE_URL}/auth/login", "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.timeout = 30;

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    LoginResponse response = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);
                    callback?.Invoke(response.success, response);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to parse login response: {ex.Message}");
                    callback?.Invoke(false, new LoginResponse { success = false, message = "Failed to parse server response" });
                }
            }
            else
            {
                HandleError(www, callback);
            }
        }
    }

    private IEnumerator LogoutUserCoroutine(string token, Action<bool, string> callback)
    {
        using (UnityWebRequest www = new UnityWebRequest($"{BASE_URL}/auth/logout", "POST"))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Authorization", $"Bearer {token}");
            www.timeout = 30;

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    ApiResponse response = JsonUtility.FromJson<ApiResponse>(www.downloadHandler.text);
                    callback?.Invoke(response.success, response.message);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to parse logout response: {ex.Message}");
                    callback?.Invoke(false, "Failed to parse server response");
                }
            }
            else
            {
                string errorMessage = GetErrorMessage(www);
                callback?.Invoke(false, errorMessage);
            }
        }
    }

    private IEnumerator SaveCharacterStateCoroutine(string token, CharacterStateDTO state, Action<bool, string> callback)
    {
        string json = JsonUtility.ToJson(state);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest www = new UnityWebRequest($"{BASE_URL}/character/save", "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", $"Bearer {token}");
            www.timeout = 30;

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    ApiResponse response = JsonUtility.FromJson<ApiResponse>(www.downloadHandler.text);
                    callback?.Invoke(response.success, response.message);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to parse save response: {ex.Message}");
                    callback?.Invoke(false, "Failed to parse server response");
                }
            }
            else
            {
                string errorMessage = GetErrorMessage(www);
                callback?.Invoke(false, errorMessage);
            }
        }
    }

    private IEnumerator LoadCharacterDataCoroutine(string token, Action<bool, CharacterDataDTO> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"{BASE_URL}/character/load"))
        {
            www.SetRequestHeader("Authorization", $"Bearer {token}");
            www.timeout = 30;

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string responseText = www.downloadHandler.text;
                    // The response is wrapped in {success: true, characterData: {...}}
                    // We need to extract just the characterData part
                    int startIndex = responseText.IndexOf("\"characterData\":");
                    if (startIndex != -1)
                    {
                        startIndex += "\"characterData\":".Length;
                        int endIndex = responseText.LastIndexOf('}');
                        string characterDataJson = responseText.Substring(startIndex, endIndex - startIndex + 1);
                        CharacterDataDTO characterData = JsonUtility.FromJson<CharacterDataDTO>(characterDataJson);
                        callback?.Invoke(true, characterData);
                    }
                    else
                    {
                        callback?.Invoke(false, null);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to parse load response: {ex.Message}");
                    callback?.Invoke(false, null);
                }
            }
            else
            {
                Debug.LogError($"Failed to load character data: {www.error}");
                callback?.Invoke(false, null);
            }
        }
    }

    private void HandleError(UnityWebRequest www, Action<bool, LoginResponse> callback)
    {
        string errorMessage = GetErrorMessage(www);

        // Try to parse error response from server
        if (!string.IsNullOrEmpty(www.downloadHandler.text))
        {
            try
            {
                LoginResponse errorResponse = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);
                callback?.Invoke(false, errorResponse);
                return;
            }
            catch
            {
                // If parsing fails, use generic error message
            }
        }

        callback?.Invoke(false, new LoginResponse { success = false, message = errorMessage });
    }

    private string GetErrorMessage(UnityWebRequest www)
    {
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            return "Cannot connect to server. Please check your connection.";
        }
        else if (www.result == UnityWebRequest.Result.ProtocolError)
        {
            if (www.responseCode == 401)
            {
                return "Invalid or expired session. Please log in again.";
            }
            return $"Server error: {www.error}";
        }
        else
        {
            return "An unexpected error occurred. Please try again.";
        }
    }
}
