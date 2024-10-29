using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("MoverMent")]  // ��Ʈ����Ʈ
    public float moveSpeed;  //�̵��ӵ�
    public float jumpPower;  //�����Ŀ�
    private Vector2 curMovementInput;  //��ǲ�ý��۰� ����Ǿ� ���� �޾ƿ��°�
    public LayerMask groundLayerMask;  //������ �ٴ������� ���� ����

    [Header("Look")]
    public Transform cameraContainer;  //ī�޶��� ��ġ����
    public float minXLook;   // �ü�ȸ�������� �ּҰ�
    public float maxXLook;   // �ü�ȸ�������� �ִ밪
    private float camCurXRot;  //��ǲ�ý��ۿ��� �޾ƿ��� ��Ÿ��
    public float lookSensitivity;  //ȸ���ΰ���(ȸ���ӵ�)
    private Vector2 mouseDelta;    //�޾ƿ� ��Ÿ���� �־��� ����
    public bool canLook = true;    //�κ��丮���� ���¿����� ȸ��������

    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // ���콺�� ���� �߾� ��ǥ�� ������Ű�� ���콺Ŀ���� �Ⱥ���
    }
    private void FixedUpdate()  // �����ۿ��� FixedUpdate�� �־��ִ°��� ����
    {
        Move();
    }
    private void LateUpdate()  //��� Update �Լ��� ȣ��� �� ���������� ȣ��Ǳ⿡ �ַ� ������Ʈ�� ���󰡰� ������ ī�޶� ����
    {
        if (canLook) //�κ��丮���� �ü� ������ �ʿ��� ���� ���� ����
        {
            Look();
        }
    }
    void Move()     //OnMove���� �޾ƿ� curMovementInput���� �̵���Ű�� �޼���
    {
        Vector3 dir = transform.forward*curMovementInput.y + transform.right*curMovementInput.x;  // �����¿� �̵�
        dir*=moveSpeed;  
        dir.y = rigidbody.velocity.y;  //y���� �ٽù޾ƿ�
        rigidbody.velocity = dir; //������ �� ����
    }
    void Look()     //OnLook���� �޾ƿ� ������ ī�޶��� ȸ�� �� �ü� ����
    {
        camCurXRot += mouseDelta.y * lookSensitivity;   //ī�޶��� ���Ʒ� ȸ��ȿ��
        camCurXRot = Mathf.Clamp(camCurXRot,minXLook,maxXLook);  //�������������� ȸ���ϵ��� ����
        cameraContainer.localEulerAngles=new Vector3(-camCurXRot, 0, 0);  //ī�޶��� x��ȸ�� ����

        transform.eulerAngles +=new Vector3(0, mouseDelta.x*lookSensitivity, 0);  //x�� y�� ĳ���͸� ȸ����Ŵ
    }
    public void OnMove(InputAction.CallbackContext context)      //inputsystem�� �̵� ��ư��
    {
        Debug.Log("OnMove");
        if (context.phase == InputActionPhase.Performed)  //������������
        {
            curMovementInput = context.ReadValue<Vector2>();
        }else if(context.phase == InputActionPhase.Canceled)  //������
        {
            curMovementInput = Vector2.zero;
        }
    }
    public void OnLook(InputAction.CallbackContext context)     //inputsystem�� ī�޶�ȸ�� ��ư��
    {
        Debug.Log("OnLook");
        mouseDelta = context.ReadValue<Vector2>();
    }
}
