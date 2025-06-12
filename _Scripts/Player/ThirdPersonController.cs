using com.absence.attributes;
using com.absence.soundsystem;
using com.absence.soundsystem.internals;
using com.game.player;
using com.game.player.statsystemextensions;
using com.game.utilities;
using UnityEngine;
using Zenject;
[RequireComponent(typeof(CharacterController), typeof(PlayerInputHandler))]
public class ThirdPersonController : MonoBehaviour
{
    public static Matrix4x4 CameraMatrix =
    new(new Vector4(Mathf.Sqrt(2) / 2, 0f, -Mathf.Sqrt(2) / 2),
        new Vector4(Mathf.Sqrt(2) / 2, 0f, Mathf.Sqrt(2) / 2),
        Vector4.zero,
        Vector4.zero);

    public static Matrix4x4 NegativeCameraMatrix =
    new(new Vector4(Mathf.Sqrt(2) / 2, 0f, -Mathf.Sqrt(2) / 2),
        new Vector4(Mathf.Sqrt(2) / 2, 0f, Mathf.Sqrt(2) / 2),
        Vector4.zero,
        Vector4.zero);

    private static readonly int s_animIDMovementDiffX = Animator.StringToHash("MovementDiffX");
    private static readonly int s_animIDMovementDiffY = Animator.StringToHash("MovementDiffY");

    [Header("Utilities")]
    [SerializeField, Required] private Transform m_orientation;
    [Header("Walk")]
    [Tooltip("Move speed of the character in m/s")]
    [SerializeField] private float walkSpeed = 2.0f;
    [Header("Slow Walk")]
    [Tooltip("Slow walk speed of the character in m/s")]
    [SerializeField] private float slowWalkSpeed = 0.6f;
    [Header("Sprint")]
    [Tooltip("Sprint speed of the character in m/s")]
    [SerializeField] private float sprintSpeed = 5.335f;
    [SerializeField] private bool alwaysSprint = false;
    [Header("Dash")]
    [Tooltip("Dash speed of the character in m/s")]
    [SerializeField] private float dashSpeed = 30.0f;
    [Tooltip("Dash duration of the character in seconds")]
    [SerializeField] private float dashDurationInSeconds = 0.21f;
    [Tooltip("Dash cooldown of the character in seconds")]
    [SerializeField] private float dashCooldownInSeconds = 1.5f;
    [Tooltip("Maximum dash count")]
    [SerializeField] private int maxDashCount = 2;
    [Header("Rotation")]
    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    [SerializeField] private float rotationSmoothTime = 0.12f;
    [Header("Acceleration")]
    [Tooltip("Acceleration and deceleration")]
    [SerializeField] private float speedChangeRate = 10.0f;
    [SerializeField] private float animSpeedChangeCoefficient = 10.0f;
    [Space]
    [Header("Sound")]
    [SerializeField] private SoundAsset m_concreteFootstepsSoundAsset;
    [SerializeField] private SoundAsset m_grassFootstepsSoundAsset;
    [SerializeField] private SoundAsset m_dashSoundAsset;

    //player
    private float _currentHorizontalSpeed;
    private float _rotationVelocity;

    //dash
    private int _dashCount = 0;
    private float _dashCooldownTimer = 0;
    private float _dashDurationTimer = 0;

    public float DashCooldown => dashCooldownInSeconds;
    public float DashCooldownTimer => _dashCooldownTimer;
    public float DashDuration => dashDurationInSeconds;
    public float DashDurationTimer => _dashDurationTimer;
    public int MaxDashCount => maxDashCount;
    public int DashCount => _dashCount;

    //animation
    private float _horizontalSpeedAnimationBlend;
    private int _animIDSpeed;
    private int _animIDMotionSpeed;
    private int _animIDDashTrigger;
    private int _animIDAttackTrigger;
    private int _animIDDeathTrigger;
    Vector2 m_movementDiff;

    //components
    private Animator _animator;
    private CharacterController _controller;
    private PlayerInputHandler _input;
    private GameObject _mainCamera;

    //extras (Events, SFX, VFX, Achievements)
    private PlayerStats _playerStats;
    private SoundFXManager _soundFXManager;

    float m_initialY;

    [Inject]
    private void ZenjectSetup(PlayerStats playerStats,SoundFXManager soundFXManager)
    {
        _playerStats = playerStats;
        _soundFXManager = soundFXManager;

        if (_playerStats == null)
            Debug.LogError("ThirdPersonController Zenject setup failed!! Player Stats is null");
        if (_soundFXManager == null)
            Debug.LogError("ThirdPersonController Zenject setup failed!! Player Stats is null");
    }
    private void Awake()
    {
        if (_mainCamera == null)
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        m_initialY = transform.position.y;
    }
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputHandler>();

        _dashCount = maxDashCount;

        AssignAnimationIDs();
    }
    private void Update()
    {
        // TEMPORARY FIX !!!

        bool isConsoleOpen = com.absence.consolesystem.ConsoleWindow.Instance.IsOpen;

        if (isConsoleOpen)
            return;

        HandleDash();
        HandleHorizontalMovement();

        Vector3 position = transform.position;
        position.y = m_initialY;
        transform.position = position;
    }

    #region Horizontal Movement

    private void HandleHorizontalMovement()
    {
        float stat = (_playerStats.GetStat(PlayerStatType.WalkSpeed) / 10) + 1;
        float targetSpeed = CalculateMaximumSpeed() * stat;
        _currentHorizontalSpeed = CalculateCurrentSpeed(targetSpeed);

        _horizontalSpeedAnimationBlend = CalculateAnimationBlend(Mathf.Min(1f, targetSpeed));

        Vector3 inputDirection = GetInputDirection();
        float targetRotation = CalculateTargetRotation(inputDirection);
        //ApplyRotation(targetRotation);

        Vector3 targetDirection = CalculateTargetDirection(_input.MovementInput);

        float realForwardDifference = Vector3.SignedAngle(Vector3.forward, m_orientation.forward, Vector3.up);

        Vector2 movementDiff = _input.MovementInput.normalized.RotateByAngle(realForwardDifference - 45f);

        m_movementDiff = Vector2.Lerp(m_movementDiff, movementDiff, Time.deltaTime * speedChangeRate);

        MovePlayer(targetDirection * _input.MovementInput.magnitude);

        UpdateAnimator();
    }
    private float CalculateMaximumSpeed()
    {
        if (_input.MovementInput == Vector2.zero)
            return 0.0f;

        if (alwaysSprint)
            return sprintSpeed;

        if (_input.SprintButtonHeld)
            return sprintSpeed;

        //if(PlayerInputHandler.Instance.AttackButtonHeld)
        //    return slowWalkSpeed;

        return walkSpeed;
    }

    private float CalculateCurrentSpeed(float targetSpeedThisFrame)
    {
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.MovementInput.magnitude : 1f;

        if (currentHorizontalSpeed < targetSpeedThisFrame - speedOffset ||
            currentHorizontalSpeed > targetSpeedThisFrame + speedOffset)
        {
            float newSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeedThisFrame * inputMagnitude, Time.deltaTime * speedChangeRate);
            return Mathf.Round(newSpeed * 1000f) / 1000f;
        }

        if (_dashDurationTimer > 0)
            return dashSpeed;

        return targetSpeedThisFrame;
    }
    private float CalculateAnimationBlend(float targetSpeed)
    {
        float blend = Mathf.Lerp(_horizontalSpeedAnimationBlend, targetSpeed, Time.deltaTime * speedChangeRate * animSpeedChangeCoefficient);
        blend = Mathf.Clamp01(blend);
        return blend;
    }
    private Vector3 GetInputDirection()
    {
        return new Vector3(_input.MovementInput.x, 0.0f, _input.MovementInput.y).normalized;
    }
    private float CalculateTargetRotation(Vector3 inputDirection)
    {
        if (_input.MovementInput == Vector2.zero )
            return transform.eulerAngles.y;

        return Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
    }
    private void ApplyRotation(float targetRotation)
    {
        if (PlayerInputHandler.Instance.AttackButtonHeld)
            return;

        targetRotation = Mathf.Clamp(targetRotation, -360f, 360f);

        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref _rotationVelocity, rotationSmoothTime);
        Quaternion rotationQuat = Quaternion.Euler(0.0f, rotation, 0.0f);

        rotationQuat = Quaternion.Normalize(rotationQuat);

        transform.rotation = rotationQuat;
    }
    private Vector3 CalculateTargetDirection(Vector2 input)
    {
        return CameraMatrix.MultiplyVector(input).normalized; // ??
    }
    private void MovePlayer(Vector3 targetDirection)
    {
        _controller.Move(targetDirection.normalized * (_currentHorizontalSpeed * Time.deltaTime));
    }
    #endregion

    #region Dash
    private void HandleDash()
    {
        HandleDashTimers();

        if (!PlayerInputHandler.Instance.DashButtonPressed) return;

        if (CanDash())
            StartDash();
    }
    private bool CanDash()
    {
        return _dashCount > 0 && _dashDurationTimer <= 0;
    }
    private void StartDash()
    {
        _dashCount--;
        _dashDurationTimer = dashDurationInSeconds;
        _dashCooldownTimer = dashCooldownInSeconds + dashDurationInSeconds;
        _animator.SetTrigger(_animIDDashTrigger);
        //_soundFXManager.PlayRandomSoundFXAtPosition(_soundFXManager.dashSoundEffects, transform);
    }
    private void HandleDashTimers()
    {
        _dashCooldownTimer = Mathf.Max(0, _dashCooldownTimer - Time.deltaTime);
        _dashDurationTimer = Mathf.Max(0, _dashDurationTimer - Time.deltaTime);

        if (_dashCooldownTimer <= 0)
            _dashCount = maxDashCount;
    }
    #endregion

    #region Animation
    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        _animIDDashTrigger = Animator.StringToHash("Dash");
        _animIDDeathTrigger = Animator.StringToHash("Death");
        _animIDAttackTrigger = Animator.StringToHash("Attack");
    }
    private void UpdateAnimator()
    {
        _animator.SetFloat(_animIDSpeed, _horizontalSpeedAnimationBlend);
        _animator.SetFloat(_animIDMotionSpeed, _input.analogMovement ? _input.MovementInput.magnitude : 1f);
        _animator.SetFloat(s_animIDMovementDiffX, m_movementDiff.x);
        _animator.SetFloat(s_animIDMovementDiffY, m_movementDiff.y);
    }
    #endregion

    #region Animation Events
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            PlaySFX(m_grassFootstepsSoundAsset);
        }
            
    }
    private void OnDashStart(AnimationEvent animationEvent)
    {
        PlaySFX(m_dashSoundAsset);
    }
    #endregion

    void PlaySFX(ISoundAsset asset)
    {
        if (SoundManager.Instance == null)
            return;

        Sound.Create(asset)
            .AtPosition(transform.position)
            .Play();
    }
}