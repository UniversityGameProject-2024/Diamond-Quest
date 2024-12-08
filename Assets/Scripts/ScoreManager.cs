using TMPro;
using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText; // Reference to the TextMeshProUGUI component for displaying the score
    private int score; // The player's current score

    private Blade blade; // Reference to the Blade component in the scene
    private Spawner spawner; // Reference to the Spawner component in the scene
    private TargetFruits targetFruits; // Reference to the TargetFruits script
    [SerializeField]
    private TextMeshProUGUI gameOverText;

    private void Awake()
    {
        // Find the Blade and Spawner objects in the scene when the game starts
        blade = FindFirstObjectByType<Blade>();
        spawner = FindFirstObjectByType<Spawner>();
        targetFruits = FindFirstObjectByType<TargetFruits>();
    }

    private void Start()
    {
        // Initialize the game and reset the score at the start
        NewGame();
    }

    private void NewGame()
    {
        score = 0; // Reset the score to 0
        UpdateScoreText(); // Update the UI to show the initial score
        gameOverText.gameObject.SetActive(false); // Hide the Game Over text
        blade.enabled = true; // Re-enable the blade
        spawner.enabled = true; // Re-enable the spawner
        // Generate new target fruits
        if (targetFruits != null)
        {
            targetFruits.GenerateNewTargets();
        }
    }
    public void IncreasingScore()
    {
        score++; // Increment the player's score
        UpdateScoreText(); // Update the UI with the new score
    }

    private void UpdateScoreText()
    {
        // Update the TextMeshProUGUI component to show "Score: <current score>"
        scoreText.text = $"Score: {score}";
    }
    //when slice a bomb object
    public void Explode()
    {
        // Disable the blade and spawner, effectively stopping the game
        blade.enabled = false;
        spawner.enabled = false;
        // Make the TextMeshProUGUI object visible
        gameOverText.gameObject.SetActive(true);

        // Start the coroutine to restart the game after a delay
        StartCoroutine(RestartAfterDelay(3f));
    }

    // Coroutine to restart the game after a specified delay
    private IEnumerator RestartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        NewGame(); // Restart the game
    }
}
