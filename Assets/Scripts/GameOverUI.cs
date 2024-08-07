using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{


    [SerializeField] private TextMeshProUGUI recipeDeliveredText;
    [SerializeField] private Button playAgainButton;




    private void Awake()
    {
        playAgainButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        GameManager.Instance.OnStartChanged += GameManager_OnStartChanged;

        Hide();
    }

    private void GameManager_OnStartChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            Show();


            recipeDeliveredText.text = DeliveryManagar.Instance.GetSuccessfulRecipesAmount().ToString();
        }
        else
        {
            Hide();
        }
    }


    private void Show()
    {
        gameObject.SetActive(true);
        playAgainButton.Select();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
