using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class UserDatabase
{
    private static string filePath = Path.Combine(Application.persistentDataPath, "users.json");

    public static List<UserData> LoadUsers()
    {
        if (!File.Exists(filePath))
            return new List<UserData>();

        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<UsersWrapper>(json).users;
    }

    public static void SaveUsers(List<UserData> users)
    {
        UsersWrapper wrapper = new UsersWrapper { users = users };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, json);
    }

    [System.Serializable]
    private class UsersWrapper
    {
        public List<UserData> users;
    }
}
