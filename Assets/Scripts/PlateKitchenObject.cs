using System;
using System.Collections;
using System.Collections.Generic;
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
            kitchenObjectSOList.Add(kitchenObjectSO);

            OnIngredientAdded?.Invoke(this,new OnIngredientAddedEventArgs
            {
                kitchenObjectSO = kitchenObjectSO
            });

            return true;
        }
        
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return kitchenObjectSOList;
    }
}
