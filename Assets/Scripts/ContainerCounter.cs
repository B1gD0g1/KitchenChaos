using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;


    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            //玩家没有拿着物品
            //Debug.Log("Interact!");
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);

            //Debug.Log( kitchenObjectTransform.GetComponent<KitchenObject>().GetKitchenObjectSO().objectName );

            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
    }

}
