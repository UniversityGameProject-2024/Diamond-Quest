using UnityEngine;
using TMPro;

public class DiamondLabel : MonoBehaviour
{
    public string diamondColorName; // ✅ שם הצבע של היהלום
    private TextMeshPro label; // ✅ רכיב טקסט מעל היהלום

    private void Start()
    {
        CreateLabel();
    }
    private void CreateLabel()
    {
        GameObject textObj = new GameObject("DiamondLabel");
        textObj.transform.SetParent(transform);
        textObj.transform.localPosition = new Vector3(0, 1.2f, 0);
        label = textObj.AddComponent<TextMeshPro>();
        diamondColorName = ExtractColorName(gameObject.name);
        label.text = diamondColorName;
        label.alignment = TextAlignmentOptions.Center;
        label.fontSize = 6;
        label.color = Color.black;
        }
    private string ExtractColorName(string fullName)
    {
        fullName = fullName.Replace("(Clone)", "").Trim();
        string[] parts = fullName.Split('_');
        if (parts.Length > 1)
        {
            return parts[parts.Length - 1];
        }
        return fullName;
    }
}
