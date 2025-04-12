using UnityEngine;

public class InputManager : MonoBehaviour
{
    void Update()
    {
        if (!GameManager.Instance.IsGameStarted()) return;

        if (Input.GetKeyDown(KeyCode.A)) TryCatch(0);
        if (Input.GetKeyDown(KeyCode.B)) TryCatch(1);
        if (Input.GetKeyDown(KeyCode.C)) TryCatch(2);
        if (Input.GetKeyDown(KeyCode.D)) TryCatch(3);
    }

    void TryCatch(int column)
    {
        Drum nextDrum = DrumManager.Instance.GetNextDrum(column);
        if (nextDrum != null)
        {
            nextDrum.TryCatch();
        }
    }
}
