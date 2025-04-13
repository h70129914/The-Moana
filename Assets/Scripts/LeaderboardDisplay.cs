using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class LeaderboardDisplay : MonoBehaviour
{
    public TextMeshProUGUI leaderboardText;
    public int usersToShow = 0;

    void Start()
    {
        GameManager.Instance.OnScoreUpdated += UpdateLeaderboard;
    }

    void OnDestroy()
    {
        GameManager.Instance.OnScoreUpdated -= UpdateLeaderboard;
    }

    void UpdateLeaderboard()
    {
        Dictionary<string, int> scores = GameManager.Instance.userScores;
        leaderboardText.text = string.Empty;
        var orderedScores = scores.OrderByDescending(s => s.Value);

        if (usersToShow > 0)
        {
            orderedScores = orderedScores.Take(usersToShow).ToDictionary(pair => pair.Key, pair => pair.Value).OrderByDescending(s => s.Value);
        }

        foreach (var score in orderedScores)
        {
            leaderboardText.text += $"{score.Key}: {score.Value}\n";
        }
    }
}
