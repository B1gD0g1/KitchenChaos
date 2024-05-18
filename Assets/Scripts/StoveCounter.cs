using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter,IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStartChangedEventArgs> OnStartChanged;
    public class OnStartChangedEventArgs : EventArgs
    {
        public State state;

    }

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }


    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    private State state;
    private float fryingTimer;
    private FryingRecipeSO fryingRecipeSO;
    private float burningTimer;
    private BurningRecipeSO burningRecipeSO;

    private void Start()
    {
        state = State.Idle;
    }


    private void Update()
    {
        if (HasKitchenObject())
        {

            switch (state)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
                    });


                    if (fryingTimer > fryingRecipeSO.fryingTimerMax)
                    {
                        //������
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);


                        state = State.Fried;
                        burningTimer = 0f;
                        burningRecipeSO = GetBurningRecipSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                        OnStartChanged?.Invoke(this,new OnStartChangedEventArgs
                        {
                            state = state
                        });
                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = burningTimer / burningRecipeSO.BurningTimerMax
                    });


                    if (burningTimer > burningRecipeSO.BurningTimerMax)
                    {
                        //������
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                        state = State.Burned;

                        OnStartChanged?.Invoke(this, new OnStartChangedEventArgs
                        {
                            state = state
                        });


                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });

                    }
                    break;
                case State.Burned:
                    break;
            }
        }
    }


    public override void Interact(Player player)
    {

        if (!HasKitchenObject())
        {
            //����û����Ʒ
            if (player.HasKitchenObject())
            {
                //�������������Ʒ
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {   //���Я����һЩ���Ա�����Ķ���
                    player.GetKitchenObject().SetKitchenObjectParent(this);



                    fryingRecipeSO = GetFryingRecipSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    state = State.Frying;
                    fryingTimer = 0f;

                    OnStartChanged?.Invoke(this, new OnStartChangedEventArgs
                    {
                        state = state
                    });

                    OnProgressChanged?.Invoke(this,new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
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


                        state = State.Idle;

                        OnStartChanged?.Invoke(this, new OnStartChangedEventArgs
                        {
                            state = state
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });

                    }

                }
            }
            else
            {
                //�������û����Ʒ
                GetKitchenObject().SetKitchenObjectParent(player);


                state = State.Idle;

                OnStartChanged?.Invoke(this, new OnStartChangedEventArgs
                {
                    state = state
                });

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                });

            }
        }
    }


    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipSOWithInput(inputKitchenObjectSO);
        return fryingRecipeSO != null;

    }


    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipSOWithInput(inputKitchenObjectSO);
        if (fryingRecipeSO != null)
        {
            return fryingRecipeSO.output;
        }
        else
        {
            return null;
        }

    }

    private FryingRecipeSO GetFryingRecipSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return fryingRecipeSO;
            }
        }
        return null;
    }


    private BurningRecipeSO GetBurningRecipSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            if (burningRecipeSO.input == inputKitchenObjectSO)
            {
                return burningRecipeSO;
            }
        }
        return null;
    }

    public bool IsFried()
    {
        return state == State.Fried;
    }
}
