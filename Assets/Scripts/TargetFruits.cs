using TMPro;
using UnityEngine;

public class TargetFruits : MonoBehaviour
{
    [Tooltip("Text to display the target fruits")]
    [SerializeField] private TextMeshProUGUI targetFruitsText;

    [Tooltip("Available fruit names (matching prefab names)")]
    [SerializeField] private string[] fruitColors;
    [SerializeField] private int FruitOrderAmount = 3;
    private string[] currentTargets; // The current sequence of target fruits
    private int currentTargetIndex;  // The current fruit in the sequence

    private void Start()
    {
        GenerateNewTargets();
    }

    // Generate a new sequence of target fruits
    public void GenerateNewTargets()
    {
        currentTargetIndex = 0;
        currentTargets = new string[FruitOrderAmount]; // Adjust size for your sequence length

        for (int i = 0; i < currentTargets.Length; i++)
        {
            currentTargets[i] = fruitColors[Random.Range(0, fruitColors.Length)];
        }

        UpdateTargetFruitsText();
    }

    // Update the displayed target fruits text
    private void UpdateTargetFruitsText()
    {
        targetFruitsText.text = $"Order: {string.Join(", ", currentTargets)}";
    }

    // Check if the fruit sliced matches the current target
    public bool CheckTarget(string fruitName)
    {
        if (fruitName == currentTargets[currentTargetIndex])
        {
            currentTargetIndex++;
            if (currentTargetIndex >= currentTargets.Length)
            {
                GenerateNewTargets(); // Reset targets when all are cleared
            }
            else
            {
                UpdateTargetFruitsText(); // Update text to show the remaining order
            }
            return true; // Correct fruit sliced
        }
        return false; // Incorrect fruit sliced
    }
}