using System;

[Serializable]
public class CharacterStateDTO
{
    public int currentHealth;
    public int currentStamina;
    public int currentMagic;
    public float positionX;
    public float positionY;
    public float worldOffsetX;
    public float worldOffsetY;
}

[Serializable]
public class CharacterDataDTO
{
    public int userId;
    public string username;
    public string playerClass;
    public CharacterStateDTO state;
}

[Serializable]
public class RegisterRequest
{
    public string username;
    public string password;
    public string playerClass;
}

[Serializable]
public class LoginRequest
{
    public string username;
    public string password;
}

[Serializable]
public class LoginResponse
{
    public bool success;
    public string message;
    public string token;
    public CharacterDataDTO characterData;
}

[Serializable]
public class ApiResponse
{
    public bool success;
    public string message;
}
