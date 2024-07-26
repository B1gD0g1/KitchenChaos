using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{

    [SerializeField] private Button playMultiplayerButton;
    [SerializeField] private Button playSingleplayerButton;
    [SerializeField] private Button quitButton;



    private void Awake()
    {
        playMultiplayerButton.onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.playMultiplayer = true;
            //Clickµã»÷
            Loader.Load(Loader.Scene.LobbyScene);
        });

        playSingleplayerButton.onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.playMultiplayer = false;
            //Clickµã»÷
            Loader.Load(Loader.Scene.LobbyScene);
        });

        quitButton.onClick.AddListener(() =>
        {
            //Clickµã»÷
            Application.Quit();
        });

        Time.timeScale = 1f;
    }

}
