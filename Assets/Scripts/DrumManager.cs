using System.Collections.Generic;
using UnityEngine;

public class DrumManager : MonoBehaviour
{
    public static DrumManager Instance;
    

    private readonly Dictionary<int, Queue<Drum>> columnDrums = new();
    public void ResetDrums()
    {
        foreach (var column in columnDrums.Values)
        {
            column.Clear();
        }
    }

    void Awake()
    {
        if (Instance == null) Instance = this;

        for (int i = 0; i < 4; i++)
        {
            columnDrums[i] = new Queue<Drum>();
        }
    }

    public void RegisterDrum(Drum drum)
    {
        columnDrums[drum.ColumnIndex].Enqueue(drum);
    }

    public void RemoveDrum(Drum drum)
    {
        if (columnDrums.ContainsKey(drum.ColumnIndex))
        {
            var newQueue = new Queue<Drum>();
            foreach (var d in columnDrums[drum.ColumnIndex])
            {
                if (d != drum) newQueue.Enqueue(d);
            }
            columnDrums[drum.ColumnIndex] = newQueue;
        }
    }

    public Drum GetNextDrum(int column)
    {
        if (columnDrums[column].Count > 0)
        {
            return columnDrums[column].Peek();
        }

        return null;
    }

    public void PopDrum(int column)
    {
        if (columnDrums[column].Count > 0)
        {
            columnDrums[column].Dequeue();
        }
    }
}
