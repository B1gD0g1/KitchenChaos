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
        //���������Ϸ״̬������޷����������彻��
        if (!GameManager.Instance.IsGamePlaying()) return;
 
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        //���������Ϸ״̬������޷����������彻��
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

        //�ͻ��������֤
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
                //���һ������ĳ���
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


    ////�����������֤
    //private void HandleMovementServerAuth()
    //{
    //    Vector2 inputVector = GameInput.Instance.GetMovementNormalized();
    //    HandleMovementServerRpc(inputVector);
    //}

    ////�ͻ������������������
    //[ServerRpc(RequireOwnership = false)]
    //private void HandleMovementServerRpc(Vector2 inputVector)
    //{

    //    Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

    //    //��ײ���
    //    float moveDistance = moveSpeed * Time.deltaTime;
    //    float playerRadius = .7f;
    //    float playerHeight = 2f;

    //    bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

    //    if (!canMove)
    //    {
    //        //��������������ƶ�

    //        //ֻ��x�������ƶ�
    //        Vector3 moveDirX = new Vector3(moveDir.x, 0, 0);
    //        canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

    //        if (canMove)
    //        {
    //            //ֻ����x�����ƶ�
    //            moveDir = moveDirX;
    //        }
    //        else
    //        {
    //            //ֻ����z�����ƶ�
    //            Vector3 moveDirZ = new Vector3(0, 0, moveDir.z);
    //            canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

    //            if (canMove)
    //            {
    //                //ֻ����z�����ƶ�
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



    //    //�ж��Ƿ�����
    //    isWalking = moveDir != Vector3.zero;

    //    //ƽ����Ϸ��ɫ��ת
    //    float rotateSpeed = 10f;
    //    transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    //    //Debug.Log(Time.deltaTime);
    //}



    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        //��ײ���
        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;

        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove)
        {
            //��������������ƶ�

            //ֻ��x�������ƶ�
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0);
            canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                //ֻ����x�����ƶ�
                moveDir = moveDirX;
            }
            else
            {
                //ֻ����z�����ƶ�
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z);
                canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    //ֻ����z�����ƶ�
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



        //�ж��Ƿ�����
        isWalking = moveDir != Vector3.zero;

        //ƽ����Ϸ��ɫ��ת
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
