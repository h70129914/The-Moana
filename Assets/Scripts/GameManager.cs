using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string CurrentPlayerName { get; set; }

    public event Action OnScoreUpdated;
    public Dictionary<string, int> userScores = new();

    public DrumSpawner drumSpawner;
    public UIFlowController main;
    public UIFlowController touch;
    public float gameTime = 60f;
    private float currentTime;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI countdownText;

    private const string ScoresFileName = "userScores.json";

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

        currentTime = gameTime;

        LoadScores();
    }

    void Update()
    {
        if (Input.anyKeyDown && CurrentGameState == GameState.DrumToStart)
        {
            StartCoroutine(StartGame());
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L))
            MoveScoresFile();
        if (CurrentGameState != GameState.Playing) return;

        currentTime -= Time.deltaTime;
        timerText.text = $"{Mathf.CeilToInt(currentTime)}";

        if (currentTime <= 0)
        {
            EndGame();
            ShowLeaderboard();
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
        touch.ShowNext();

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

    public void ResetGame()
    {
        EndGame();

        main.JumpTo(0);
        touch.JumpTo(0);
        CurrentGameState = GameState.Idle;
    }

    public void EndGame()
    {
        if (CurrentGameState != GameState.Playing)
        {
            return;
        }

        CurrentGameState = GameState.End;

        timerText.text = "0";

        if (scoreText != null && userScores.ContainsKey(CurrentPlayerName))
        {
            scoreText.text = $"{userScores[CurrentPlayerName]}";
            SaveScores();
        }

        DrumManager.Instance.ResetDrums();
        FindFirstObjectByType<DrumSpawner>().StopSpawningAndClear();
    }

    private void ShowLeaderboard()
    {
        main.ShowNext();
        FindFirstObjectByType<LeaderboardController>().ShowLeaderboard();
    }

    public bool IsGameStarted()
    {
        return CurrentGameState == GameState.Playing;
    }

    public int GetFinalScore() => userScores.ContainsKey(CurrentPlayerName) ? userScores[CurrentPlayerName] : 0;

    private void SaveScores()
    {
        string json = JsonConvert.SerializeObject(userScores);
        Debug.Log(Path.Combine(Application.persistentDataPath, ScoresFileName));
        File.WriteAllText(Path.Combine(Application.persistentDataPath, ScoresFileName), json);
    }

    private void LoadScores()
    {
        string filePath = Path.Combine(Application.persistentDataPath, ScoresFileName);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            userScores = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
        }
    }

    private void MoveScoresFile()
    {
        Debug.Log(Application.persistentDataPath);
        string filePath = Path.Combine(Application.persistentDataPath, ScoresFileName);
        if (File.Exists(filePath))
        {
            string backupFilePath = filePath + ".bak_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
            File.Move(filePath, backupFilePath);
            Debug.Log($"Scores file moved to {backupFilePath}");
            userScores.Clear();
            OnScoreUpdated?.Invoke();
        }
    }
}
