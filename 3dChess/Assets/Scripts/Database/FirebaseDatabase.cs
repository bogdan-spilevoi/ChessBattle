using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public sealed class FirebaseDatabase
{
    private static FirebaseDatabase _defaultInstance;

    public static FirebaseDatabase DefaultInstance =>
        _defaultInstance ??= new FirebaseDatabase(FirebaseConfigLoader.DatabaseUrl());

    public string DatabaseUrl { get; }
    public string AuthToken { get; set; }

    private FirebaseDatabase(string databaseUrl)
    {
        DatabaseUrl = databaseUrl.TrimEnd('/');
    }

    public DatabaseReference GetReference(string path = "")
    {
        return new DatabaseReference(this, Normalize(path));
    }

    internal string BuildUrl(string path)
    {
        var url = $"{DatabaseUrl}";
        if (!string.IsNullOrEmpty(path))
            url += "/" + path.Trim('/');

        url += ".json";

        if (!string.IsNullOrEmpty(AuthToken))
            url += "?auth=" + UnityWebRequest.EscapeURL(AuthToken);

        return url;
    }

    private static string Normalize(string p)
    {
        if (string.IsNullOrWhiteSpace(p)) return "";
        return p.Trim().Trim('/');
    }
}