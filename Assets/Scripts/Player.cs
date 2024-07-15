using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.XR.OpenVR;
using UnityEngine;

public class Player : NetworkBehaviour ,IKitchenObjectParent
{

    public static event EventHandler OnAnyPlayerSpawn;
    public static event EventHandler OnAnyPickedSomething;



    public static void ResetStaticData()
    {
        OnAnyPlayerSpawn = null;
    }

    public static Player LocalInstance {  get; private set; }


    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;


    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;



    private void Start()
    {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }


        OnAnyPlayerSpawn?.Invoke(this,EventArgs.Empty);
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        //如果不是游戏状态，玩家无法与其他物体交互
        if (!GameManager.Instance.IsGamePlaying()) return;
 
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        //如果不是游戏状态，玩家无法与其他物体交互
        if (!GameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }

    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        //客户端身份验证
        //HandleMovementServerAuth();

        HandleMovement();
        HandleInteractions();
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        if ( moveDir != Vector3.zero )
        {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        RaycastHit raycastHit;
        if (Physics.Raycast(transform.position , lastInteractDir, out raycastHit, interactDistance , counterLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                //获得一个具体的橱柜
                //clearCounter.Interact();
                if(baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);

                }
                else
                {
                    SetSelectedCounter(null);

                }
            }
            else
            {
                SetSelectedCounter(null);

            }

        }

    }

    public bool IsWalking()
    {
        return isWalking;
    }


    ////服务器身份验证
    //private void HandleMovementServerAuth()
    //{
    //    Vector2 inputVector = GameInput.Instance.GetMovementNormalized();
    //    HandleMovementServerRpc(inputVector);
    //}

    ////客户端向服务器发送请求
    //[ServerRpc(RequireOwnership = false)]
    //private void HandleMovementServerRpc(Vector2 inputVector)
    //{

    //    Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

    //    //碰撞检测
    //    float moveDistance = moveSpeed * Time.deltaTime;
    //    float playerRadius = .7f;
    //    float playerHeight = 2f;

    //    bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

    //    if (!canMove)
    //    {
    //        //不能往这个方向移动

    //        //只有x方向上移动
    //        Vector3 moveDirX = new Vector3(moveDir.x, 0, 0);
    //        canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

    //        if (canMove)
    //        {
    //            //只能在x轴上移动
    //            moveDir = moveDirX;
    //        }
    //        else
    //        {
    //            //只能在z轴上移动
    //            Vector3 moveDirZ = new Vector3(0, 0, moveDir.z);
    //            canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

    //            if (canMove)
    //            {
    //                //只能在z轴上移动
    //                moveDir = moveDirZ;
    //            }
    //            else
    //            {

    //            }
    //        }
    //    }

    //    if (canMove)
    //    {
    //        transform.position += moveDir * moveDistance;
    //    }



    //    //判断是否行走
    //    isWalking = moveDir != Vector3.zero;

    //    //平滑游戏角色旋转
    //    float rotateSpeed = 10f;
    //    transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    //    //Debug.Log(Time.deltaTime);
    //}



    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        //碰撞检测
        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;

        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove)
        {
            //不能往这个方向移动

            //只有x方向上移动
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0);
            canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                //只能在x轴上移动
                moveDir = moveDirX;
            }
            else
            {
                //只能在z轴上移动
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z);
                canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    //只能在z轴上移动
                    moveDir = moveDirZ;
                }
                else
                {

                }
            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }



        //判断是否行走
        isWalking = moveDir != Vector3.zero;

        //平滑游戏角色旋转
        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
        //Debug.Log(Time.deltaTime);
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
            OnAnyPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

}
