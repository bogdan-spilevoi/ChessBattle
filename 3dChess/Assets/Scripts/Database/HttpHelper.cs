using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

internal static class HttpHelper
{
    public static async Task<(long code, string body)> SendAsync(string method, string url, string json)
    {
        using var req = new UnityWebRequest(url, method);

        if (json != null)
        {
            var bytes = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bytes);
            req.SetRequestHeader("Content-Type", "application/json");
        }

        req.downloadHandler = new DownloadHandlerBuffer();

        var op = req.SendWebRequest();
        while (!op.isDone)
            await Task.Yield();

#if UNITY_2020_2_OR_NEWER
        if (req.result == UnityWebRequest.Result.ConnectionError ||
            req.result == UnityWebRequest.Result.ProtocolError)
#else
        if (req.isNetworkError || req.isHttpError)
#endif
        {
            return (req.responseCode, req.downloadHandler.text);
        }

        return (req.responseCode, req.downloadHandler.text);
    }
}