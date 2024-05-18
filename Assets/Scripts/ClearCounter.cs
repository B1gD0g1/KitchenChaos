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
            //����û����Ʒ
            if (player.HasKitchenObject() )
            {
                //�������������Ʒ
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                //�������û����Ʒ

            }

        }else
        {
            //�������Ѿ�ӵ����Ʒ
            if( player.HasKitchenObject())
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
                else
                {
                    //��������õĲ������ӣ���������ʳ��
                    if (GetKitchenObject().TryGetPlate(out  plateKitchenObject))
                    {
                        //������������
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            player.GetKitchenObject().DestroySelf();
                        }
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


}
