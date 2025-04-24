using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class LeaderboardController : MonoBehaviour
{
    public GameObject leaderboardTextPrefab;
    public Transform container;
    public TextMeshProUGUI finalScoreText;

    private void Start()
    {
        GameManager.Instance.OnScoreUpdated += UpdateLeaderboard;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnScoreUpdated -= UpdateLeaderboard;
    }

    public void ShowLeaderboard()
    {
        UpdateLeaderboard();
        gameObject.SetActive(true);
    }

    private void UpdateLeaderboard()
    {
        Dictionary<string, int> scores = GameManager.Instance.userScores;
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        var orderedScores = scores.OrderByDescending(s => s.Value).Take(10);
        int index = 1;

        for (int i = 0; i < 10; i++)
        {
            GameObject leaderboardEntry = Instantiate(leaderboardTextPrefab, container);
            TextMeshProUGUI textComponent = leaderboardEntry.GetComponent<TextMeshProUGUI>();
            if (i < orderedScores.Count())
            {
                var score = orderedScores.ElementAt(i);
                textComponent.text = $"{i + 1}. {score.Key}";
            }
            else
            {
                textComponent.text = $"{i + 1}.";
            }
        }
        finalScoreText.text = $"{GameManager.Instance.GetFinalScore()}";
    }
}
