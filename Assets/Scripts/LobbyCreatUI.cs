using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreatUI : MonoBehaviour
{

    [SerializeField] private Button closeButton;
    [SerializeField] private Button creatPublicButton;
    [SerializeField] private Button creatPrivateButton;
    [SerializeField] private TMP_InputField lobbyNameInputField;



    private void Awake()
    {
        creatPublicButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.CreatLobby(lobbyNameInputField.text, false);
        });

        creatPrivateButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.CreatLobby(lobbyNameInputField.text, true);
        });

        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

}
