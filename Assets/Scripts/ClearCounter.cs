using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ClearCounter : BaseCounter
{

    [SerializeField] private KitchenObjectSO kitchenObjectSO;



    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            //桌上没有物品
            if (player.HasKitchenObject() )
            {
                //玩家手里拿着物品
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                //玩家手里没有物品

            }

        }else
        {
            //橱柜上已经拥有物品
            if( player.HasKitchenObject())
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
                else
                {
                    //玩家手里拿的不是盘子，而是其他食物
                    if (GetKitchenObject().TryGetPlate(out  plateKitchenObject))
                    {
                        //橱柜上有盘子
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
                        }
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


}
