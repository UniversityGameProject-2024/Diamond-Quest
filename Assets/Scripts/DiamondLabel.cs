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
        textObj.transform.SetParent(transform); // ✅ מחבר את הטקסט ליהלום
        textObj.transform.localPosition = new Vector3(0, 1.2f, 0); // ✅ מיקום מעל היהלום
        
        label = textObj.AddComponent<TextMeshPro>();

        // ✅ קובע את שם הצבע בלבד (החלק האחרון בשם)
        diamondColorName = ExtractColorName(gameObject.name);
        label.text = diamondColorName;
        
        label.alignment = TextAlignmentOptions.Center;
        label.fontSize = 6; // ✅ מתאים את הגודל
        label.color = Color.black; // ✅ צבע טקסט לבן

        Debug.Log($"✅ Label created for {gameObject.name} with text: {diamondColorName}");
    }

    // ✅ פונקציה שמחלצת את הצבע מתוך שם האובייקט
    private string ExtractColorName(string fullName)
    {
        fullName = fullName.Replace("(Clone)", "").Trim(); // ✅ מסיר "(Clone)"
        string[] parts = fullName.Split('_'); // ✅ מחלק את השם לפי "_"

        if (parts.Length > 1)
        {
            return parts[parts.Length - 1]; // ✅ מחזיר רק את החלק האחרון (הצבע)
        }
        return fullName; // אם אין "_" מחזיר את השם כמו שהוא
    }
}
