using TMPro;
using UnityEngine;

public class TargetFruits : MonoBehaviour
{
    [Tooltip("Text to display the target fruits")]
    [SerializeField] private TextMeshProUGUI targetFruitsText;
    [Tooltip("Fruits colors")]
    [SerializeField] private string[] fruitColors;
    [Tooltip("How many colors desplay")]
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
        // Replace already sliced fruits with "X" in the displayed text
        string[] displayTargets = new string[currentTargets.Length];
        for (int i = 0; i < currentTargets.Length; i++)
        {
            displayTargets[i] = i < currentTargetIndex ? "X" : currentTargets[i];
        }

        targetFruitsText.text = $"Order: {string.Join(", ", displayTargets)}";
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
