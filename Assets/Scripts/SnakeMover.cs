using UnityEngine;

public class SnakeMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;

    [Header("Lifetime Settings")]
    public float lifetime = 5f; // כמה זמן הנחש חי

    private Vector3 moveDirection;

    public void Initialize(Vector3 direction)
    {
        moveDirection = direction;

        // הפניית הנחש בהתאם לכיוון התנועה
        if (direction == Vector3.left)
        {
            transform.localScale = new Vector3(1, 1, 1); // פונה שמאלה
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1); // פונה ימינה
        }

        // השמדה לאחר זמן
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }
}
