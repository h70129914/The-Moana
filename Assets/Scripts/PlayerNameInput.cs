using TMPro;
using UnityEngine;

public class PlayerNameInput : MonoBehaviour
{
    public TMP_InputField nameInput;

    public void OnSubmitName()
    {
        string playerName = nameInput.text.Trim();
        if (string.IsNullOrEmpty(playerName)) return;

        GameManager.Instance.RegisterPlayer(playerName);
    }
}
