using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class VolumeBarSegmented : MonoBehaviour
{
    [SerializeField] private int totalSegments = 10;
    [SerializeField] private List<Image> segments;

    public void SetVolumeLevel(float normalizedLoudness)
    {
        int activeCount = Mathf.RoundToInt(normalizedLoudness * totalSegments);

        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].enabled = i < activeCount;
        }
    }
}
