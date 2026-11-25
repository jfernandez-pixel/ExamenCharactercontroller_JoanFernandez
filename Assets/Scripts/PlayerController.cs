using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    CharacterController _characterController;
    Animator _animator;

    InputAction _moveAction;
    InputAction _jumpAction;
    Vector2 _moveInputValue;
    Transform _cameraTransform;

    float _movementSpeed = 10f;
    public float _jumpForce = 20f;
    float _gravity = -9.81f;
    Vector3 _playerGravity;
    float _smothTime = 0.5f;
    [SerializeField]float _turnSmoothVelocity;

    [SerializeField]public Transform _sensorPosition ;
    [SerializeField]float _sensorRadius = 0.5f;
    [SerializeField]LayerMask _groundLayer;
    
    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();

        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];

        _gravity = Physics.gravity.y;
        _cameraTransform = Camera.main.transform;
    }
    
    void Update()
    {
        _moveInputValue = _moveAction.ReadValue<Vector2>();
        Movement();
        if(_jumpAction.WasPressedThisFrame())
        {
            Jump();
        }
        Gravity();
        _animator.SetBool("IsJumping", !IsGrounded());
    }
    void Movement()
    {
        Vector3 direction = new Vector3(_moveInputValue.x, 0,_moveInputValue.y);
        _animator.SetFloat("Horizontal", _moveInputValue.x, 0.2f, Time.deltaTime);
        _animator.SetFloat("Vertical", _moveInputValue.y, 0.2f, Time.deltaTime);

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, _cameraTransform.eulerAngles.y, ref _turnSmoothVelocity, _smothTime);
        transform.rotation = Quaternion.Euler(0, smoothAngle,0);

        if(direction != Vector3.zero)
        {
            Vector3 moveDirection = Quaternion.Euler(0, targetAngle,0) * Vector3.forward;
            _characterController.Move(moveDirection * _movementSpeed * Time.deltaTime);
        }
    }
    void Jump()
    {
        _playerGravity.y = _jumpForce;
    }
    bool IsGrounded()
    {
        return Physics.CheckSphere(_sensorPosition.position, _sensorRadius, _groundLayer);
    }
    void Gravity()
    {
        if(!IsGrounded())
        {
            _playerGravity.y += _gravity * Time.deltaTime;
        }
        if(IsGrounded() && _playerGravity.y < 0)
        {
            _playerGravity.y = _gravity;
        }
        _characterController.Move(_playerGravity * Time.deltaTime);
    }
}

