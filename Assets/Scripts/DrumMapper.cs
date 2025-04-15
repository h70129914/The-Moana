using UnityEngine;

public class DrumMapper : MonoBehaviour
{
    public GameObject drumPrefab;
    public Transform guide;
    public Transform parentContainer;

    public float[] columnX = new float[4] { -3f, -1f, 1f, 3f }; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) SpawnDrum(0);
        if (Input.GetKeyDown(KeyCode.B)) SpawnDrum(1);
        if (Input.GetKeyDown(KeyCode.C)) SpawnDrum(2);
        if (Input.GetKeyDown(KeyCode.D)) SpawnDrum(3);
    }

    void SpawnDrum(int columnIndex)
    {
        Vector3 spawnPosition = new(columnX[columnIndex], guide.position.y, 0);
        GameObject drum = Instantiate(drumPrefab, spawnPosition, Quaternion.identity, parentContainer);
        drum.name = $"Drum_Col{columnIndex}_Y{spawnPosition.y:F2}";
        Debug.Log($"Spawned drum at column {columnIndex}, y={spawnPosition.y:F2}");
    }
}
