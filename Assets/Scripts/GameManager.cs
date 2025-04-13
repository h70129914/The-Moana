using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string CurrentPlayerName { get; set; }

    public event Action OnScoreUpdated;
    public Dictionary<string, int> userScores = new();

    public DrumSpawner drumSpawner;
    public UIFlowController main;
    public float gameTime = 60f;
    private float currentTime;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI countdownText;

    [SerializeField] private float endGameWaitTime = 5f; 

    public enum GameState
    {
        Idle,
        DrumToStart,
        GetReady,
        Playing,
        End
    }

    public GameState CurrentGameState { get; private set; } = GameState.Idle;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentTime = gameTime;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && CurrentGameState == GameState.DrumToStart)
        {
            StartCoroutine(StartGame());
        }
        if (CurrentGameState != GameState.Playing) return;

        currentTime -= Time.deltaTime;
        timerText.text = $"{Mathf.CeilToInt(currentTime)}";

        if (currentTime <= 0)
        {
            EndGame();
        }
    }

    public void NameSubmit()
    {
        if (CurrentGameState != GameState.Idle)
        {
            return;
        }
        if (string.IsNullOrEmpty(CurrentPlayerName))
            return;

        main.ShowNext();
        CurrentGameState = GameState.DrumToStart;
    }

    public IEnumerator StartGame()
    {
        CurrentGameState = GameState.GetReady;
        main.ShowNext();

        yield return StartCoroutine(Countdown());

        CurrentGameState = GameState.Playing;

        currentTime = gameTime;

        drumSpawner.StartSpawning();
    }

    private IEnumerator Countdown()
    {
        float countdown = 3f;

        while (countdown > 0f)
        {
            countdownText.text = Mathf.CeilToInt(countdown).ToString();

            yield return new WaitForSeconds(1f);
            countdown -= 1f;
        }

        countdownText.text = ""; // Clear text
        main.ShowNext();
    }

    public void RegisterPlayer(string playerName)
    {
        CurrentPlayerName = playerName;

        if (!userScores.ContainsKey(playerName))
        {
            userScores.Add(playerName, 0);
        }

        UpdateScore(0); // Trigger UI update
    }

    public void UpdateScore(int scoreToAdd)
    {
        if (string.IsNullOrEmpty(CurrentPlayerName)) return;

        if (!userScores.ContainsKey(CurrentPlayerName))
        {
            userScores.Add(CurrentPlayerName, 0);
        }

        userScores[CurrentPlayerName] += scoreToAdd;

        if (scoreText != null)
        {
            scoreText.text = $"{userScores[CurrentPlayerName]}";
        }

        OnScoreUpdated?.Invoke();
    }

    void EndGame()
    {
        CurrentGameState = GameState.End;

        timerText.text = "0";

        if (scoreText != null && userScores.ContainsKey(CurrentPlayerName))
        {
            scoreText.text = $"{userScores[CurrentPlayerName]}";
        }

        main.ShowNext();
        FindFirstObjectByType<DrumSpawner>().StopSpawningAndClear();
        FindFirstObjectByType<LeaderboardController>().ShowLeaderboard();

        StartCoroutine(EndGameRoutine());
    }

    private IEnumerator EndGameRoutine()
    {
        yield return new WaitForSeconds(endGameWaitTime);
        main.JumpTo(0);
        CurrentGameState = GameState.Idle;
    }

    public bool IsGameStarted()
    {
        return CurrentGameState == GameState.Playing;
    }

    public int GetFinalScore() => userScores[CurrentPlayerName];
}
