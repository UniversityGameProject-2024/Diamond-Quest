using UnityEngine;
using System.Collections;

public class BigDiamond : MonoBehaviour
{
    private Renderer diamondRenderer;
    private SliceableObject sliceableObject;
    public static bool hasSpawned = false; //  משתנה למניעת יצירה כפולה
    public static float scaleBigDiamond = 3f;


    public static BigDiamond Create(GameObject diamondPrefab, Vector3 position)
    {
        if (hasSpawned)
        {
            Debug.Log("⚠️ Big Diamond already exists! Skipping...");
            return null;
        }

        hasSpawned = true; //  מסמן שכבר נוצר יהלום גדול
        Quaternion fixedRotation = Quaternion.Euler(-30f, 1f, 1f);

        GameObject bigDiamondObj = Instantiate(diamondPrefab, position, fixedRotation);
        bigDiamondObj.tag = "Big Diamond";
        bigDiamondObj.transform.localScale = new Vector3(scaleBigDiamond, scaleBigDiamond, scaleBigDiamond); // ✅ גודל גדול
         //  הוספת סיבוב ליהלום הגדול
        RotateDiamond rotateScript = bigDiamondObj.AddComponent<RotateDiamond>();
        rotateScript.rotationSpeed = 50f; // ניתן לשנות מהירות


        BigDiamond bigDiamond = bigDiamondObj.AddComponent<BigDiamond>();

        return bigDiamond;
    }
    

    private void Start()
    {
        diamondRenderer = GetComponent<Renderer>();
        sliceableObject = GetComponent<SliceableObject>();

        //  מניעת חיתוך של היהלום הגדול
        if (sliceableObject != null)
        {
            sliceableObject.enabled = false;
        }

        //  שליחת הצבע של היהלום הגדול ללהב
        if (diamondRenderer != null)
        {
            Color bigDiamondColor = diamondRenderer.material.color;
            GameManager.Instance.SetBigDiamondColor(bigDiamondColor);
            Blade bladeScript = FindObjectOfType<Blade>();
            if (bladeScript != null)
            {
                bladeScript.SetBigDiamondColor(bigDiamondColor);
            }
        }

        StartCoroutine(DestroyAfterTime(3f)); //  הורס את היהלום לאחר 3 שניות
    }

    private IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        //  אפקט התכווצות לפני ההשמדה
        float shrinkDuration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < shrinkDuration)
        {
            float scale = Mathf.Lerp(1f, 0f, elapsedTime / shrinkDuration);
            transform.localScale = new Vector3(scale, scale, scale);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
        hasSpawned = false; //  מאפשר יצירה מחדש בסיבוב הבא
    }
}
