using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Button registerButton;

    private void Start()
    {
        nameInput.onValueChanged.AddListener(OnNameInputValueChanged);
        registerButton.interactable = false;
    }

    public void OnSubmitName()
    {
        string playerName = nameInput.text.Trim();
        if (string.IsNullOrEmpty(playerName)) return;

        GameManager.Instance.RegisterPlayer(playerName);
        nameInput.text = string.Empty;
    }

    private void OnNameInputValueChanged(string input)
    {
        registerButton.interactable = !string.IsNullOrEmpty(input.Trim());
    }
}
