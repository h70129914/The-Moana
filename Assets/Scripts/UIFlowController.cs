using UnityEngine;
using System.Collections.Generic;

public class UIFlowController : MonoBehaviour
{
    [Header("Ordered UI Windows")]
    [SerializeField] private List<GameObject> windows;

    private int currentIndex = 0;

    void Start()
    {
        ShowOnly(currentIndex);
    }

    public void ShowNext()
    {
        if (currentIndex < windows.Count - 1)
        {
            currentIndex++;
            ShowOnly(currentIndex);
        }
    }

    public void ShowPrevious()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowOnly(currentIndex);
        }
    }

    private void ShowOnly(int index)
    {
        for (int i = 0; i < windows.Count; i++)
            windows[i].SetActive(i == index);
    }

    public void JumpTo(int index)
    {
        if (index >= 0 && index < windows.Count)
        {
            currentIndex = index;
            ShowOnly(currentIndex);
        }
    }

    public int GetCurrentIndex() => currentIndex;
}
