using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;


    [SerializeField] private KitchenObjectSO platesKitchenObjectSO;

    private float spawnPlatesTimer;
    private float spawnPlatesTimerMax = 4f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;




    private void Update()
    {
        spawnPlatesTimer += Time.deltaTime;
        if (spawnPlatesTimer > spawnPlatesTimerMax)
        {
            spawnPlatesTimer = 0f;

            if (GameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnedAmountMax)
            {
                platesSpawnedAmount++;

                OnPlateSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            //������ֿտ�
            if (platesSpawnedAmount > 0)
            {
                //������һ�������ڳ�����
                platesSpawnedAmount--;

                KitchenObject.SpawnKitchenObject(platesKitchenObjectSO,player);

                OnPlateRemoved?.Invoke(this, EventArgs.Empty);
            }
        }
    }


}
