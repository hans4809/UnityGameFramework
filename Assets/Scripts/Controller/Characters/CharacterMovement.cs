using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EViewType
{
    FirstPerson,
    TopView,
    SideView,
    QuarterView,
    ShoulderView,
}

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private EViewType _viewType;
    public EViewType ViewType { get => _viewType; }

    [SerializeField] private float _moveSpeed;
    public float MoveSpeed { get => _moveSpeed; private set => _moveSpeed = value; }

    [SerializeField] private float _rotationSpeed;
    public float RotationSpeed { get => _rotationSpeed; private set => _rotationSpeed = value; }

    [SerializeField] private Animator _anim;
    public Animator Anim { get => _anim; private set => _anim = value; }

    [SerializeField] private Vector2 _moveInput;
    public Vector2 MoveInput { get => _moveInput; private set => _moveInput = value; }

#region 3D
    [SerializeField] private Rigidbody _rb;
    public Rigidbody Rb { get => _rb; private set => _rb = value; }

    [SerializeField] private Vector3 _dirVec3;
    public Vector3 DirVec3 { get => _dirVec3; private set => _dirVec3 = value; }

    [SerializeField] private Vector3 _lookVec3;
    public Vector3 LookVec3 { get => _lookVec3; private set => _lookVec3 = value; }

    [SerializeField] private Camera _followCamera;
    public Camera FollowCamera { get => _followCamera; private set => _followCamera = value; }
#endregion

#region 2D
    [SerializeField] private Rigidbody2D _rb2D;
    public Rigidbody2D Rb2D { get => _rb2D; private set => _rb2D = value; }

    [SerializeField] private Vector2 _dirVec2;
    public Vector2 DirVec2 { get => _dirVec2; private set => _dirVec2 = value; }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if (Rb == null)
            Rb = GetComponent<Rigidbody>();

        if (Rb2D == null)
            Rb2D = GetComponent<Rigidbody2D>();

        if (FollowCamera == null)
            FollowCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        MoveAndRotate();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        MoveInput = new Vector2(horizontalInput, verticalInput);

        if (MoveInput.sqrMagnitude > 1)
            MoveInput = MoveInput.normalized;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        DirVec2 = new Vector2(horizontalInput, verticalInput).normalized;
        DirVec3 = new Vector3(horizontalInput, 0, verticalInput).normalized;

    }

    void MoveAndRotate()
    {
        switch (ViewType)
        {
            case EViewType.FirstPerson:
                break;
            case EViewType.TopView:
                break;
            case EViewType.SideView:
                break;
            case EViewType.QuarterView:
                QuarterViewMoveAndRotate();
                break;
            case EViewType.ShoulderView:
                ShoulderViewMoveAndRotate();
                break;
        }
    }

    void FirstPersonMoveAndRotate()
    {
        
    }

    void TopViewMoveAndRotate()
    {

    }

    void SlideViewMoveAndRotate()
    {

    }

    void QuarterViewMoveAndRotate()
    {
        if (DirVec3.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(DirVec3);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);

            Rb.velocity = transform.forward * MoveSpeed;
        }
        else
            Rb.velocity = Vector3.zero;
    }

    void ShoulderViewMoveAndRotate()
    {
        var targetSpeed = MoveSpeed * MoveInput.magnitude;
        var moveDirection = Vector3.Normalize(transform.forward * MoveInput.y + transform.right * MoveInput.x);

        Rb.velocity = new Vector3(moveDirection.x * targetSpeed, Rb.velocity.y, moveDirection.z * targetSpeed);

        var targetRotation = FollowCamera.transform.eulerAngles.y;

        transform.eulerAngles = Vector3.up * targetRotation;
    }
}
