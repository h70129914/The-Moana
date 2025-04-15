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
        UpdateLeaderboard();
    }

    void OnDestroy()
    {
        GameManager.Instance.OnScoreUpdated -= UpdateLeaderboard;
    }

    internal void UpdateLeaderboard()
    {
        Dictionary<string, int> scores = GameManager.Instance.userScores;
        leaderboardText.text = string.Empty;
        var orderedScores = scores.OrderByDescending(s => s.Value).ToList();

        for (int i = 0; i < usersToShow; i++)
        {
            if (i < orderedScores.Count)
            {
                var score = orderedScores[i];
                leaderboardText.text += $"{i + 1}. {score.Key}\n";
            }
            else
            {
                leaderboardText.text += $"{i + 1}.\n";
            }
        }
    }
}
