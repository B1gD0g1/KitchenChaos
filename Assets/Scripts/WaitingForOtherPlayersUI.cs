using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForOtherPlayersUI : MonoBehaviour
{




    private void Start()
    {
        GameManager.Instance.OnLocalPlayerReadyChanged += GameManager_OnLocalPlayerReadyChanged;
        GameManager.Instance.OnStartChanged += GameManager_OnStartChanged;

        Hide();
    }

    private void GameManager_OnStartChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsCountdownToStartActive())
        {
            Hide();
        }
    }

    private void GameManager_OnLocalPlayerReadyChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsLocalPlayerReady())
        {
            Show();
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
