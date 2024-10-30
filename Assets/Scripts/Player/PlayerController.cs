using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;  // 이동 속도
    [SerializeField] private float boostSpeed = 10f; //부스트 속도
    [SerializeField] private float boostDuration = 10f; //부스트 지속시간
    [SerializeField] private float jumpPower = 5f;  // 점프 파워
    [SerializeField] private LayerMask groundLayerMask;  // 바닥 판정을 위한 레이어 마스크

    [Header("Look")]
    [SerializeField] private Transform cameraContainer;  // 카메라의 위치 저장
    [SerializeField] private float minXLook = -90f;   // 시선 회전 범위의 최소값
    [SerializeField] private float maxXLook = 90f;    // 시선 회전 범위의 최대값
    [SerializeField] private float lookSensitivity = 2f;  // 회전 민감도
    [SerializeField] private bool canLook = true;    // 시선 고정 여부

    private Vector2 currentMovementInput;  // 인풋 시스템과 연결되어 값을 받아오는 변수
    private Vector2 mouseDelta;  // 마우스 델타 값
    private float camCurXRot;  // 카메라의 현재 X 회전 값
    private float currentSpeed; // 부스트인지 기본인지에 따른 속도값입력
    private bool isBoosted = false; //부스트상태인지 아닌지 판단
    public Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError("Rigidbody");
        }

        if (cameraContainer == null)
        {
            Debug.LogWarning("Camera Container");
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // 마우스를 게임 중앙 좌표에 고정시키고 마우스 커서가 보이지 않음
        currentSpeed = moveSpeed;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            Look();
        }
    }

    void Move()
    {
        Vector3 direction = (transform.forward * currentMovementInput.y + transform.right * currentMovementInput.x).normalized;
        Vector3 velocity = direction * currentSpeed;
        velocity.y = _rigidbody.velocity.y;  // Y축 속도 유지
        _rigidbody.velocity = velocity;
    }

    void Look()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);

        if (cameraContainer != null)
        {
            cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
        }

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            currentMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            currentMovementInput = Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && IsGround())
        {
            _rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            CharacterManager.Instance.Player.condition.Subtract(10f);
        }
    }

    bool IsGround()
    {
        float distance = 0.1f;
        Vector3[] origins = new Vector3[]
        {
            transform.position + transform.forward * 0.2f,
            transform.position - transform.forward * 0.2f,
            transform.position + transform.right * 0.2f,
            transform.position - transform.right * 0.2f
        };

        foreach (var origin in origins)
        {
            if (Physics.Raycast(origin + Vector3.up * 0.01f, Vector3.down, distance, groundLayerMask))
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        float distance = 0.1f;
        Vector3[] origins = new Vector3[]
        {
            transform.position + transform.forward * 0.2f,
            transform.position - transform.forward * 0.2f,
            transform.position + transform.right * 0.2f,
            transform.position - transform.right * 0.2f
        };

        foreach (var origin in origins)
        {
            Gizmos.DrawRay(origin + Vector3.up * 0.01f, Vector3.down * distance);
        }
    }
    public void ActiveBoost()
    {
        StartCoroutine(SpeedBoostCoroutine());
    }
    private IEnumerator SpeedBoostCoroutine()
    {
        isBoosted = true;
        currentSpeed = boostSpeed;

        yield return new WaitForSeconds(boostDuration);

        currentSpeed = moveSpeed;
        isBoosted = false;
    }
}
