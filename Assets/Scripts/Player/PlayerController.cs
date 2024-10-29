using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("MoverMent")]  // 어트리뷰트
    public float moveSpeed;  //이동속도
    public float jumpPower;  //점프파워
    private Vector2 curMovementInput;  //인풋시스템과 연결되어 값을 받아오는것
    public LayerMask groundLayerMask;  //점프시 바닥판정을 위한 변수

    [Header("Look")]
    public Transform cameraContainer;  //카메라의 위치저장
    public float minXLook;   // 시선회전범위의 최소값
    public float maxXLook;   // 시선회전범위의 최대값
    private float camCurXRot;  //인풋시스템에서 받아오는 델타값
    public float lookSensitivity;  //회전민감도(회전속도)
    private Vector2 mouseDelta;    //받아온 델타값을 넣어줄 변수
    public bool canLook = true;    //인벤토리등의 상태에따라 회전고정용

    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // 마우스를 게임 중앙 좌표에 고정시키고 마우스커서가 안보임
    }
    private void FixedUpdate()  // 물리작용은 FixedUpdate에 넣어주는것이 좋음
    {
        Move();
    }
    private void LateUpdate()  //모든 Update 함수가 호출된 후 마지막으로 호출되기에 주로 오브젝트를 따라가게 설정한 카메라에 적합
    {
        if (canLook) //인벤토리등의 시선 고정이 필요할 때를 위해 지정
        {
            Look();
        }
    }
    void Move()     //OnMove에서 받아온 curMovementInput으로 이동시키는 메서드
    {
        Vector3 dir = transform.forward*curMovementInput.y + transform.right*curMovementInput.x;  // 전후좌우 이동
        dir*=moveSpeed;  
        dir.y = rigidbody.velocity.y;  //y값은 다시받아옴
        rigidbody.velocity = dir; //변경한 값 적용
    }
    void Look()     //OnLook에서 받아온 값으로 카메라의 회전 및 시선 관리
    {
        camCurXRot += mouseDelta.y * lookSensitivity;   //카메라의 위아래 회전효과
        camCurXRot = Mathf.Clamp(camCurXRot,minXLook,maxXLook);  //지정범위내에서 회전하도록 조절
        cameraContainer.localEulerAngles=new Vector3(-camCurXRot, 0, 0);  //카메라의 x축회전 조절

        transform.eulerAngles +=new Vector3(0, mouseDelta.x*lookSensitivity, 0);  //x와 y로 캐릭터를 회전시킴
    }
    public void OnMove(InputAction.CallbackContext context)      //inputsystem의 이동 버튼용
    {
        Debug.Log("OnMove");
        if (context.phase == InputActionPhase.Performed)  //누르고있을때
        {
            curMovementInput = context.ReadValue<Vector2>();
        }else if(context.phase == InputActionPhase.Canceled)  //땟을때
        {
            curMovementInput = Vector2.zero;
        }
    }
    public void OnLook(InputAction.CallbackContext context)     //inputsystem의 카메라회전 버튼용
    {
        Debug.Log("OnLook");
        mouseDelta = context.ReadValue<Vector2>();
    }
}
