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

        float mouseX = Input.GetAxis("MouseX");
        float mouseY = Input.GetAxis("MouseY");

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
        if (DirVec3.magnitude > 0)
        {
            float targetAngle = Mathf.Atan2(DirVec3.x, DirVec3.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _rotationSpeed, 0.1f);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward * MoveSpeed;
            Rb.velocity = new Vector3(moveDirection.x, Rb.velocity.y, moveDirection.z);
        }
        else
            Rb.velocity = new Vector3(0, Rb.velocity.y, 0);
    }
}
