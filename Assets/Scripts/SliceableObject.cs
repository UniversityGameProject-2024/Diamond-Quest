using UnityEngine;
using EzySlice;

public class SliceableObject : MonoBehaviour
{
    public Material crossSectionMaterial; // חומר שיופיע על פני השטח של החתך
    public void Slice(Vector3 position, Vector3 normal)
    {
        GameObject[] slices = gameObject.SliceInstantiate(position, normal, crossSectionMaterial);
        if (slices == null || slices.Length == 0)
        {
            Debug.Log("Slicing failed: No slices created.");
            return;
        }
        Debug.Log("Slicing successful! Created " + slices.Length + " pieces.");
        foreach (GameObject slice in slices)
        {
            slice.isStatic = false;
            Rigidbody rb = slice.AddComponent<Rigidbody>();
            rb.mass = 0.5f;
            rb.linearDamping = 0.1f;
            rb.angularDamping = 0.05f;
            if (!slice.GetComponent<Collider>())
            {
                slice.AddComponent<BoxCollider>();
            }
            //הוסף כוח (Force) כדי להפריד את החלקים
            rb.AddForce(Random.insideUnitSphere * 3f, ForceMode.Impulse); //כוח קל
            rb.AddTorque(Random.insideUnitSphere * 3f, ForceMode.Impulse); //סיבוב קל
        }
        Destroy(gameObject);
    }
}
