using System;
using System.Collections;
using System.Collections.Generic;
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
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    cuttingProgress = 0;

                    CuttingRecipSO cuttingRecipSO = GetCuttingRecipSOWithInout(GetKitchenObject().GetKitchenObjectSO());

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = (float)cuttingProgress / cuttingRecipSO.cuttingProgressMax
                    });

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
                        GetKitchenObject().DestroySelf();
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

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
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

            if (cuttingProgress >= cuttingRecipSO.cuttingProgressMax)
            {
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
                //����ԭ�еĶ���
                GetKitchenObject().DestroySelf();

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
