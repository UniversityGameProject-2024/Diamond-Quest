using UnityEngine;

public class RotateDiamond : MonoBehaviour
{
    [SerializeField] public float rotationSpeed = 50f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);  

     //   transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
    }
}
