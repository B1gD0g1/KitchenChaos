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
            //桌上没有物品
            if (player.HasKitchenObject())
            {
                //玩家手里拿着物品
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {   //玩家携带了一些可以被切割的东西
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);

                    InteractLogicPlaceObjectOnCounterServerRpc();
                }
            }
            else
            {
                //玩家手里没有物品
            }
        }
        else
        {
            //桌上已经拥有物品
            if (player.HasKitchenObject())
            {
                //玩家手里拿着物品
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //玩家手里拿着的是盘子
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }

                }
            }
            else
            {
                //玩家手里没有物品
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
            //桌上有物品并且可以切割
            CutObjectServerRpc();
            TestCuttingProgressDonServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CutObjectServerRpc()
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            //桌上有物品并且可以切割
            CutObjectClientRpc();
        }
    }

    [ClientRpc]
    private void CutObjectClientRpc()
    {
        //桌上有物品并且可以切割
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
            //桌上有物品并且可以切割
            CuttingRecipSO cuttingRecipSO = GetCuttingRecipSOWithInout(GetKitchenObject().GetKitchenObjectSO());

            if (cuttingProgress >= cuttingRecipSO.cuttingProgressMax)
            {
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

                //销毁原有的对象
                KitchenObject.DestroyKitchenObject(GetKitchenObject());

                //生成新的对象
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
