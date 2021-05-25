using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController), typeof(PlayerInputHandler), typeof(AudioSource))]
public class PlayerCharacterController : MonoBehaviour
{
    [Header("References")] [Tooltip("Reference to the main camera used for the player")]
    public Camera playerCamera;

    [Tooltip("Audio source for footsteps, jump, etc...")]
    public AudioSource audioSource;

    [Header("General")] [Tooltip("Force applied downward when in the air")]
    public float gravityDownForce = 20f;

    [Tooltip("Physic layers checked to consider the player grounded")]
    public LayerMask groundCheckLayers = -1;

    [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
    public float groundCheckDistance = 0.05f;

    [Header("Movement")] [Tooltip("Max movement speed when grounded (when not sprinting)")]
    public float maxSpeedOnGround = 10f;

    [Tooltip(
        "Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
    public float movementSharpnessOnGround = 15;

    [Tooltip("Max movement speed when crouching")] [Range(0, 1)]
    public float maxSpeedCrouchedRatio = 0.5f;

    [Tooltip("Max movement speed when not grounded")]
    public float maxSpeedInAir = 10f;

    [Tooltip("Acceleration speed when in the air")]
    public float accelerationSpeedInAir = 25f;

    [Tooltip("Multiplicator for the sprint speed (based on grounded speed)")]
    public float sprintSpeedModifier = 2f;

    [Tooltip("Height at which the player dies instantly when falling off the map")]
    public float killHeight = -50f;

    [Header("Rotation")] [Tooltip("Rotation speed for moving the camera")]
    public float rotationSpeed = 200f;

    [Range(0.1f, 1f)] [Tooltip("Rotation speed multiplier when aiming")]
    public float aimingRotationMultiplier = 0.4f;

    [Header("Jump")] [Tooltip("Force applied upward when jumping")]
    public float jumpForce = 9f;

    [Header("Stance")] [Tooltip("Ratio (0-1) of the character height where the camera will be at")]
    public float cameraHeightRatio = 0.9f;

    [Tooltip("Height of character when standing")]
    public float capsuleHeightStanding = 1.8f;

    [Tooltip("Height of character when crouching")]
    public float capsuleHeightCrouching = 0.9f;

    [Tooltip("Speed of crouching transitions")]
    public float crouchingSharpness = 1;

    public UnityAction<bool> OnStanceChanged;

    public Vector3 CharacterVelocity { get; set; }
    public bool IsGrounded { get; private set; }
    public bool HasJumpedThisFrame { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsCrouching { get; private set; }

    private PlayerInputHandler _inputHandler;
    private CharacterController _characterController;
    private WallRun _wallRunComponent;

    private Vector3 _groundNormal;
    private float _lastTimeJumped;
    private float _cameraVerticalAngle;
    private float _targetCharacterHeight;
    private bool _jumpOnAir;

    private const float JumpGroundingPreventionTime = 0.2f;
    private const float KGroundCheckDistanceInAir = 0.07f;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _inputHandler = GetComponent<PlayerInputHandler>();
        _wallRunComponent = GetComponent<WallRun>();

        _characterController.enableOverlapRecovery = true;

        // Отключаем приседание
        SetCrouchingState(false, true);
        UpdateCharacterHeight(true);
    }

    private void Update()
    {
        HasJumpedThisFrame = false;

        bool wasGrounded = IsGrounded;
        GroundCheck();

        if (_inputHandler.GetCrouchInputDown())
        {
            SetCrouchingState(!IsCrouching, false);
        }

        UpdateCharacterHeight(false);

        HandleCharacterMovement();
    }

    private void GroundCheck()
    {
        float chosenGroundCheckDistance =
            IsGrounded ? (_characterController.skinWidth + groundCheckDistance) : KGroundCheckDistanceInAir;

        IsGrounded = false;
        _groundNormal = Vector3.up;

        // Проверка через время после прыжка
        if (Time.time >= _lastTimeJumped + JumpGroundingPreventionTime)
        {
            // Если на земле - получаем ее нормаль
            if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(_characterController.height),
                _characterController.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance,
                groundCheckLayers, QueryTriggerInteraction.Ignore))
            {
                _groundNormal = hit.normal;

                // Проверка на то - сонаправлены ли нормаль земли и перса
                if (Vector3.Dot(hit.normal, transform.up) > 0f && IsNormalUnderSlopeLimit(_groundNormal))
                {
                    IsGrounded = true;

                    if (hit.distance > _characterController.skinWidth)
                    {
                        _characterController.Move(Vector3.down * hit.distance);
                    }
                }
            }
        }
    }

    private void HandleCharacterMovement()
    {
        // Вращение камеры по горизонтали
        transform.Rotate(new Vector3(0f, (_inputHandler.GetLookInputsHorizontal() * rotationSpeed), 0f), Space.Self);

        // Вращение камеры по вертикали
        _cameraVerticalAngle += _inputHandler.GetLookInputsVertical() * rotationSpeed;
        _cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, -89f, 89f);
        
        // Наклон камеры, когда бежим по стене
        if (_wallRunComponent != null)
        {
            playerCamera.transform.localEulerAngles =
                new Vector3(_cameraVerticalAngle, 0, _wallRunComponent.GetCameraRoll());
        }
        else
        {
            playerCamera.transform.localEulerAngles = new Vector3(_cameraVerticalAngle, 0, 0);
        }


        // Передвижение персонажа
        bool isSprinting = _inputHandler.GetSprintInputHeld();
        {
            if (isSprinting)
            {
                isSprinting = SetCrouchingState(false, false);
            }

            float speedModifier = isSprinting ? sprintSpeedModifier : 1f;
            Vector3 worldSpaceMoveInput = transform.TransformVector(_inputHandler.GetMoveInput());

            // По земле
            if (IsGrounded || (_wallRunComponent != null && _wallRunComponent.IsWallRunning()))
            {
                if (IsGrounded)
                {
                    _jumpOnAir = true;
                    Vector3 targetVelocity = worldSpaceMoveInput * maxSpeedOnGround * speedModifier;
                    if (IsCrouching)
                        targetVelocity *= maxSpeedCrouchedRatio;
                    
                    targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, _groundNormal) *
                                     targetVelocity.magnitude;
                    CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVelocity,
                        movementSharpnessOnGround * Time.deltaTime);
                }

                // Прыжок
                if ((IsGrounded || (_wallRunComponent != null && _wallRunComponent.IsWallRunning())) &&
                    _inputHandler.GetJumpInputDown())
                {
                    if (SetCrouchingState(false, false))
                    {
                        if (IsGrounded)
                        {
                            CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);
                            CharacterVelocity += Vector3.up * jumpForce;
                        }
                        else
                        {
                            CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);
                            CharacterVelocity += _wallRunComponent.GetWallJumpDirection() * jumpForce;
                        }
                        
                        _lastTimeJumped = Time.time;
                        HasJumpedThisFrame = true;
                        
                        IsGrounded = false;
                        _groundNormal = Vector3.up;
                    }
                }
            }
            // По воздуху
            else
            {
                if (_inputHandler.GetJumpInputDown() && _jumpOnAir)
                {
                    CharacterVelocity = new Vector3(CharacterVelocity.x, 0f, CharacterVelocity.z);
                    CharacterVelocity += Vector3.up * jumpForce;
                    _lastTimeJumped = Time.time;
                    _jumpOnAir = false;
                }
                
                if (_wallRunComponent == null || (_wallRunComponent != null && !_wallRunComponent.IsWallRunning()))
                {
                    CharacterVelocity += worldSpaceMoveInput * accelerationSpeedInAir * Time.deltaTime;
                    
                    float verticalVelocity = CharacterVelocity.y;
                    Vector3 horizontalVelocity = Vector3.ProjectOnPlane(CharacterVelocity, Vector3.up);
                    horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxSpeedInAir * speedModifier);
                    CharacterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);
                    
                    CharacterVelocity += Vector3.down * gravityDownForce * Time.deltaTime;
                }
            }
        }

        // Финалочка - применяем все скорости
        Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
        Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere(_characterController.height);
        _characterController.Move(CharacterVelocity * Time.deltaTime);

        if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, _characterController.radius,
            CharacterVelocity.normalized, out RaycastHit hit, CharacterVelocity.magnitude * Time.deltaTime, -1,
            QueryTriggerInteraction.Ignore))
        {
            CharacterVelocity = Vector3.ProjectOnPlane(CharacterVelocity, hit.normal);
        }
    }

    // Возвращает true, если угол наклона, представленный данной нормалью, ниже предела угла наклона контроллера персонажа
    private bool IsNormalUnderSlopeLimit(Vector3 normal)
    {
        return Vector3.Angle(transform.up, normal) <= _characterController.slopeLimit;
    }

    // Получает центральную точку нижней полусферы капсулы контроллера персонажа.  
    private Vector3 GetCapsuleBottomHemisphere()
    {
        return transform.position + (transform.up * _characterController.radius);
    }

    // Получает центральную точку верхней полусферы капсулы контроллера персонажа.    
    private Vector3 GetCapsuleTopHemisphere(float atHeight)
    {
        return transform.position + (transform.up * (atHeight - _characterController.radius));
    }

    // Получает переориентированное направление, касательное к заданному уклону.
    private Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
    {
        Vector3 directionRight = Vector3.Cross(direction, transform.up);
        return Vector3.Cross(slopeNormal, directionRight).normalized;
    }

    private void UpdateCharacterHeight(bool force)
    {
        if (force)
        {
            _characterController.height = _targetCharacterHeight;
            _characterController.center = Vector3.up * _characterController.height * 0.5f;
            playerCamera.transform.localPosition = Vector3.up * _targetCharacterHeight * cameraHeightRatio;
        }
        else if (Math.Abs(_characterController.height - _targetCharacterHeight) > .0001f)
        {
            // Изменить размер капсулы и отрегулировать положение камеры
            _characterController.height = Mathf.Lerp(_characterController.height, _targetCharacterHeight,
                crouchingSharpness * Time.deltaTime);
            _characterController.center = Vector3.up * _characterController.height * 0.5f;
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition,
                Vector3.up * _targetCharacterHeight * cameraHeightRatio, crouchingSharpness * Time.deltaTime);
        }
    }

    // Возвращает false, если нашлось препятствие
    private bool SetCrouchingState(bool crouched, bool ignoreObstructions)
    {
        if (crouched)
        {
            _targetCharacterHeight = capsuleHeightCrouching;
        }
        else
        {
            // Обнаружение препятствий
            if (!ignoreObstructions)
            {
                Collider[] standingOverlaps = Physics.OverlapCapsule(
                    GetCapsuleBottomHemisphere(),
                    GetCapsuleTopHemisphere(capsuleHeightStanding),
                    _characterController.radius,
                    -1,
                    QueryTriggerInteraction.Ignore);
                foreach (Collider c in standingOverlaps)
                {
                    if (c != _characterController)
                    {
                        return false;
                    }
                }
            }

            _targetCharacterHeight = capsuleHeightStanding;
        }

        if (OnStanceChanged != null)
        {
            OnStanceChanged.Invoke(crouched);
        }

        IsCrouching = crouched;
        return true;
    }
}