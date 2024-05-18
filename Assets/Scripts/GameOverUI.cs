using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{


    [SerializeField] private TextMeshProUGUI recipeDeliveredText;



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
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
