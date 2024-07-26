using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter,IHasProgress
{
    public static event EventHandler OnAnyCut;

    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }


    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;

    [SerializeField] private CuttingRecipSO[] cuttingRecipeSOArray;


    private int cuttingProgress;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            //����û����Ʒ
            if (player.HasKitchenObject())
            {
                //�������������Ʒ
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {   //���Я����һЩ���Ա��и�Ķ���
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);

                    InteractLogicPlaceObjectOnCounterServerRpc();
                }
            }
            else
            {
                //�������û����Ʒ
            }
        }
        else
        {
            //�����Ѿ�ӵ����Ʒ
            if (player.HasKitchenObject())
            {
                //�������������Ʒ
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //����������ŵ�������
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }

                }
            }
            else
            {
                //�������û����Ʒ
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc()
    {
        InteractLogicPlaceObjectOnCounterClientRpc();
    }

    [ClientRpc]
    private void InteractLogicPlaceObjectOnCounterClientRpc()
    {
        cuttingProgress = 0;

        //CuttingRecipSO cuttingRecipSO = GetCuttingRecipSOWithInout(kitchenObject.GetKitchenObjectSO());

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            //progressNormalized = (float)cuttingProgress / cuttingRecipSO.cuttingProgressMax
            progressNormalized = 0f

        });
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            //��������Ʒ���ҿ����и�
            CutObjectServerRpc();
            TestCuttingProgressDonServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CutObjectServerRpc()
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            //��������Ʒ���ҿ����и�
            CutObjectClientRpc();
        }
    }

    [ClientRpc]
    private void CutObjectClientRpc()
    {
        //��������Ʒ���ҿ����и�
        cuttingProgress++;

        OnCut?.Invoke(this, EventArgs.Empty);

        OnAnyCut?.Invoke(this, EventArgs.Empty);

        CuttingRecipSO cuttingRecipSO = GetCuttingRecipSOWithInout(GetKitchenObject().GetKitchenObjectSO());

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = (float)cuttingProgress / cuttingRecipSO.cuttingProgressMax
        });

        
    }

    [ServerRpc(RequireOwnership = false)]
    private void TestCuttingProgressDonServerRpc()
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            //��������Ʒ���ҿ����и�
            CuttingRecipSO cuttingRecipSO = GetCuttingRecipSOWithInout(GetKitchenObject().GetKitchenObjectSO());

            if (cuttingProgress >= cuttingRecipSO.cuttingProgressMax)
            {
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

                //����ԭ�еĶ���
                KitchenObject.DestroyKitchenObject(GetKitchenObject());

                //�����µĶ���
                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipSO cuttingRecipSO = GetCuttingRecipSOWithInout(inputKitchenObjectSO);
        return cuttingRecipSO != null;

    }


    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipSO cuttingRecipSO = GetCuttingRecipSOWithInout(inputKitchenObjectSO);
        if (cuttingRecipSO != null)
        {
            return cuttingRecipSO.output;
        }
        else
        {
            return null;
        }

    }

    private CuttingRecipSO GetCuttingRecipSOWithInout(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipSO cuttingRecipSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipSO;
            }
        }
        return null;
    }

}
