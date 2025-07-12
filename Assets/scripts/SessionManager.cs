using UnityEngine;

public static class SessionManager
{
    public static string currentUsername;

    public static void SetCurrentUser(string username)
    {
        currentUsername = username;
        PlayerPrefs.SetString("current_user", username);
        PlayerPrefs.Save();
    }

    public static void LoadCurrentUser()
    {
        currentUsername = PlayerPrefs.GetString("current_user", "");
    }

    public static void ClearUser()
    {
        currentUsername = "";
        PlayerPrefs.DeleteKey("current_user");
    }
}
