using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrumSpawner : MonoBehaviour
{
    public Drum drumPrefab;
    public Transform[] spawnPositions;
    public AudioSource musicSource;
    
    public float spawnY = 10f;
    public float fallSpeed = 2;
    public float perfectY;
    public float totalGameDuration { get; set; }

    public List<float> drumKeyframes;

    private void Start()
    {
        totalGameDuration = GameManager.Instance.gameTime;
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        float fallDuration = CalculateFallDuration();
        float earliestHitTime = drumKeyframes.Min();
        var delayBeforeMusic = Mathf.Max(0f, fallDuration - earliestHitTime);

        float loopDuration = musicSource.clip.length;
        float elapsedTime = 0f;

        while (elapsedTime < totalGameDuration)
        {
            float cycleStartTime = Time.time;

            Debug.Log($"Starting loop at {elapsedTime}s, music starts in {delayBeforeMusic}s");

            foreach (float hitTime in drumKeyframes)
            {
                int randomColumn = Random.Range(0, spawnPositions.Length);
                float spawnTime = hitTime - fallDuration + delayBeforeMusic;

                StartCoroutine(SpawnDrumAfterDelay(spawnTime, randomColumn));
            }

            // Start music after delay
            StartCoroutine(StartMusicAfterDelay(delayBeforeMusic));

            // Wait until loop is done
            float waitTime = loopDuration + delayBeforeMusic;
            yield return new WaitForSeconds(waitTime);

            elapsedTime += waitTime;
        }

        Debug.Log("Game time is over. Stopping spawning.");
    }


    private float CalculateFallDuration()
    {
        float fallDistance = Mathf.Abs(spawnY - perfectY);
        float duration = fallDistance / fallSpeed;

        Debug.Log($"Calculated fall duration: {duration} seconds");
        return duration;
    }

    private IEnumerator SpawnDrumAfterDelay(float delay, int column)
    {
        Debug.Log($"Waiting {delay} seconds to spawn drum in column {column}.");
        yield return new WaitForSeconds(delay);
        SpawnDrum(column, spawnY);
    }

    private IEnumerator StartMusicAfterDelay(float delay)
    {
        Debug.Log($"Waiting {delay} seconds to start music.");
        yield return new WaitForSeconds(delay);
        musicSource.Play();
        Debug.Log("Music started.");
    }

    public void SpawnDrum(int columnIndex, float startY)
    {
        if (columnIndex < 0 || columnIndex >= spawnPositions.Length)
        {
            Debug.LogError("Invalid column index");
            return;
        }

        Vector3 spawnPosition = new(spawnPositions[columnIndex].position.x, startY, 0);
        Drum drum = Instantiate(drumPrefab, spawnPosition, Quaternion.identity);
        drum.Initialize(fallSpeed, perfectY, columnIndex);
        Debug.Log($"Drum spawned in column {columnIndex} at position {spawnPosition}.");
    }
}
