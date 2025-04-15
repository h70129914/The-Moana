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

    public List<float> drumKeyframes = new() {
        0.4596f, 2.5580f, 2.8895f, 3.4343f, 4.8074f, 5.2611f, 8.4830f, 8.9325f, 9.3691f, 12.0119f,
        12.3638f, 13.4353f, 14.5942f, 14.9065f, 15.4695f, 15.7918f, 17.5539f, 17.9681f, 19.0787f,
        19.5915f, 20.2421f, 21.5190f, 22.2128f, 22.9359f, 23.2482f, 23.6972f, 24.1599f, 24.4772f,
        27.0675f, 28.6186f };

    private Coroutine spawnLoopCoroutine;

    private void Start()
    {
        totalGameDuration = GameManager.Instance.gameTime;
    }

    public void StartSpawning()
    {
        spawnLoopCoroutine = StartCoroutine(SpawnLoop());
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
            foreach (float hitTime in drumKeyframes)
            {
                int randomColumn = Random.Range(0, spawnPositions.Length);
                float spawnTime = hitTime - fallDuration + delayBeforeMusic;

                StartCoroutine(SpawnDrumAfterDelay(spawnTime, randomColumn));
            }

            StartCoroutine(StartMusicAfterDelay(delayBeforeMusic));

            float waitTime = loopDuration + delayBeforeMusic;
            yield return new WaitForSeconds(waitTime);

            elapsedTime += waitTime;
        }

        musicSource.Stop();
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

    public void StopSpawningAndClear()
    {
        if (spawnLoopCoroutine != null)
        {
            StopAllCoroutines();
            spawnLoopCoroutine = null;
        }

        Drum[] drums = FindObjectsOfType<Drum>();
        foreach (Drum drum in drums)
        {
            Destroy(drum.gameObject);
        }

        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }

        Debug.Log("Stopped spawning, destroyed all drums, and stopped the music.");
    }
}
