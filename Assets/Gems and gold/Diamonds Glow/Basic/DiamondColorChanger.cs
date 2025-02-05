using UnityEngine;

public class DiamondColorChanger : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // גישה ל-Sprite Renderer

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red; // משנה את הצבע לכחול בזמן ריצה
        }
    }
}
