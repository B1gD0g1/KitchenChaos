using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;


    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            //���û��������Ʒ
            //Debug.Log("Interact!");
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);

            //Debug.Log( kitchenObjectTransform.GetComponent<KitchenObject>().GetKitchenObjectSO().objectName );

            InteractLogicServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
    }
}
