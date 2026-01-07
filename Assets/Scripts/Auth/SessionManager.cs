using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }

    private string currentToken;
    private CharacterDataDTO currentUser;

    public bool IsLoggedIn => !string.IsNullOrEmpty(currentToken);
    public string CurrentToken => currentToken;
    public CharacterDataDTO CurrentUser => currentUser;

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

    public void SetSession(string token, CharacterDataDTO userData)
    {
        currentToken = token;
        currentUser = userData;
        Debug.Log($"Session set for user: {userData.username}");
    }

    public void ClearSession()
    {
        currentToken = null;
        currentUser = null;
        Debug.Log("Session cleared");
    }

    public string GetAuthToken()
    {
        return currentToken;
    }
}
