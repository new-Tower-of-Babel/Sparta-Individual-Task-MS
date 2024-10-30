using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;  // �̵� �ӵ�
    [SerializeField] private float boostSpeed = 10f; //�ν�Ʈ �ӵ�
    [SerializeField] private float boostDuration = 10f; //�ν�Ʈ ���ӽð�
    [SerializeField] private float jumpPower = 5f;  // ���� �Ŀ�
    [SerializeField] private LayerMask groundLayerMask;  // �ٴ� ������ ���� ���̾� ����ũ

    [Header("Look")]
    [SerializeField] private Transform cameraContainer;  // ī�޶��� ��ġ ����
    [SerializeField] private float minXLook = -90f;   // �ü� ȸ�� ������ �ּҰ�
    [SerializeField] private float maxXLook = 90f;    // �ü� ȸ�� ������ �ִ밪
    [SerializeField] private float lookSensitivity = 2f;  // ȸ�� �ΰ���
    [SerializeField] private bool canLook = true;    // �ü� ���� ����

    private Vector2 currentMovementInput;  // ��ǲ �ý��۰� ����Ǿ� ���� �޾ƿ��� ����
    private Vector2 mouseDelta;  // ���콺 ��Ÿ ��
    private float camCurXRot;  // ī�޶��� ���� X ȸ�� ��
    private float currentSpeed; // �ν�Ʈ���� �⺻������ ���� �ӵ����Է�
    private bool isBoosted = false; //�ν�Ʈ�������� �ƴ��� �Ǵ�
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
        Cursor.lockState = CursorLockMode.Locked;  // ���콺�� ���� �߾� ��ǥ�� ������Ű�� ���콺 Ŀ���� ������ ����
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
        velocity.y = _rigidbody.velocity.y;  // Y�� �ӵ� ����
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
