using UnityEngine;
using EzySlice; 

public class SliceableObject : MonoBehaviour
{

    public Material crossSectionMaterial; // חומר שיופיע על פני השטח של החתך

   public void Slice(Vector3 position, Vector3 normal)
{
    Debug.Log("🔪 Slice function activated for: " + gameObject.name);

    // יצירת החלקים החדשים
    GameObject[] slices = gameObject.SliceInstantiate(position, normal, crossSectionMaterial);

    if (slices == null || slices.Length == 0)
    {
        Debug.LogError("Slicing failed: No slices created.");
        return;
    }

    Debug.Log("Slicing successful! Created " + slices.Length + " pieces.");

    // הוספת Rigidbody לכל חתיכה והפעלת כוח
    foreach (GameObject slice in slices)
    {
        slice.isStatic = false; // וודא שהחתיכות לא סטטיות

        // הוסף Rigidbody
        Rigidbody rb = slice.AddComponent<Rigidbody>();
        rb.mass = 0.5f; // מסה נמוכה לחתיכות קטנות
        rb.linearDamping = 0.1f; // חיכוך קל
        rb.angularDamping = 0.05f; // חיכוך סיבובי קל

        if (!slice.GetComponent<Collider>())
        {
            slice.AddComponent<BoxCollider>();
        }

        // הוסף כוח (Force) כדי להפריד את החלקים
        rb.AddForce(Random.insideUnitSphere * 3f, ForceMode.Impulse); // כוח קל
        rb.AddTorque(Random.insideUnitSphere * 3f, ForceMode.Impulse); // סיבוב קל
    }

    // השמד את האובייקט המקורי
    Destroy(gameObject);
}

}
