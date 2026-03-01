using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[System.Serializable]
public class FirebaseConfig { public string databaseUrl; }

public static class FirebaseConfigLoader
{
    public static string DatabaseUrl()
    {
        var json = Resources.Load<TextAsset>("firebase_config");
        if (json == null) throw new System.Exception("Missing firebase_config.json in Resources");
        var cfg = JsonUtility.FromJson<FirebaseConfig>(json.text);
        return cfg.databaseUrl;
    }
}