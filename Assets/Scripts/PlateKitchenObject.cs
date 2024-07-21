using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO kitchenObjectSO;
    }

    [SerializeField] private List<KitchenObjectSO> vaildKitchenObjectSOList;

    private List<KitchenObjectSO> kitchenObjectSOList;


    protected override void Awake()
    {
        base.Awake();
        kitchenObjectSOList = new List<KitchenObjectSO>();    
    }

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        //������ֻ��װ�ӹ����ʳ��
        if (!vaildKitchenObjectSOList.Contains(kitchenObjectSO))
        {
            //������Ч��ʳ��
            return false;
        }
        if (kitchenObjectSOList.Contains(kitchenObjectSO))
        {
            //�Ѿ�ӵ���������͵�
            return false;
        }
        else
        {
            AddIngredientServerRpc(
                KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO)
            );

            

            return true;
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int kitchenObjectSOIndex)
    {
        AddIngredientCLientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void AddIngredientCLientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

        kitchenObjectSOList.Add(kitchenObjectSO);

        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
        {
            kitchenObjectSO = kitchenObjectSO
        });
    }


    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return kitchenObjectSOList;
    }
}
