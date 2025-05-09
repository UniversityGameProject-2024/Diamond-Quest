using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class FirebaseWebGL
{
    private const string DatabaseUrl = "https://diamond-quest-59676-default-rtdb.firebaseio.com/";
    private const string ApiKey = "AIzaSyBbCK_47ajHXvR12RirvvtvDBEPodEOJ6w";

    public static async Task SaveScoreAsync(string username, Dictionary<string, object> data)
    {
        string json = Serialize(data);
        string url = DatabaseUrl + "users/" +
                     UnityWebRequest.EscapeURL(username) +
                     "/gameResults.json?auth=" + ApiKey;

        using (var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var op = request.SendWebRequest();
            while (!op.isDone) await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
                Debug.LogError("SaveScoreAsync error: " + request.error);
            else
                Debug.Log("SaveScoreAsync success");
        }
    }

    public static async Task<string> GetPasswordAsync(string username)
    {
        string url = DatabaseUrl + "users/" +
                     UnityWebRequest.EscapeURL(username) +
                     "/password.json?auth=" + ApiKey;

        using (var request = UnityWebRequest.Get(url))
        {
            var op = request.SendWebRequest();
            while (!op.isDone) await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("GetPasswordAsync error: " + request.error);
                return null;
            }

            var text = request.downloadHandler.text;
            if (string.IsNullOrEmpty(text) || text == "null")
                return null;
            return text.Trim('\"');
        }
    }

    public static async Task<bool> CheckUserExistsAsync(string username)
    {
        string url = DatabaseUrl + "users/" +
                     UnityWebRequest.EscapeURL(username) +
                     ".json?auth=" + ApiKey;

        using (var request = UnityWebRequest.Get(url))
        {
            var op = request.SendWebRequest();
            while (!op.isDone) await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("CheckUserExistsAsync error: " + request.error);
                return false;
            }

            var text = request.downloadHandler.text;
            return !string.IsNullOrEmpty(text) && text != "null";
        }
    }

    public static async Task<bool> RegisterUserAsync(string username, string password)
    {
        string url = DatabaseUrl + "users/" +
                     UnityWebRequest.EscapeURL(username) +
                     "/password.json?auth=" + ApiKey;
        // מכניסים את הסיסמה כמחרוזת JSON
        string json = "\"" + Escape(password) + "\"";

        using (var request = UnityWebRequest.Put(url, json))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            var op = request.SendWebRequest();
            while (!op.isDone) await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("RegisterUserAsync error: " + request.error);
                return false;
            }
            return true;
        }
    }

    // המרה ידנית של Dictionary ל־JSON
    private static string Serialize(Dictionary<string, object> dict)
    {
        var sb = new StringBuilder();
        sb.Append("{");
        bool first = true;

        foreach (var kv in dict)
        {
            if (!first) sb.Append(",");
            first = false;

            // מפתח במרכאות
            sb.Append("\"").Append(Escape(kv.Key)).Append("\":");

            // ערך – אם מחרוזת, במרכאות; אם בוליאני, lower; אחרת כפי שהוא
            if (kv.Value is string)
                sb.Append("\"").Append(Escape(kv.Value.ToString())).Append("\"");
            else if (kv.Value is bool)
                sb.Append(kv.Value.ToString().ToLower());
            else
                sb.Append(kv.Value.ToString());
        }

        sb.Append("}");
        return sb.ToString();
    }

    // בריחת תווי backslash ו־quote
    private static string Escape(string s)
    {
        return s
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"");
    }
}
