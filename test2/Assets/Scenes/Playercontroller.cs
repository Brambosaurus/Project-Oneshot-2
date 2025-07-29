using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _turnSpeed = 1000f;
    [SerializeField] private float acceleration = 10f;

    [Header("Sprint")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private float sprintMultiplier = 1.5f;

    [Header("Roll Dash")]
    [SerializeField] private float rollDistance = 5f;
    [SerializeField] private float rollDuration = 0.3f;
    [SerializeField] private float rollCooldown = 1f;
    [SerializeField] private AnimationCurve rollCurve;

    [Header("Roll in Mud")]
    [SerializeField] private float mudRollDistance = 2.5f;
    [SerializeField] private float mudRollDuration = 0.5f;

    [Header("Terrain")]
    [SerializeField] private float terrainSpeedMultiplier = 1f;

    [Header("Roll Charges")]
    [SerializeField] private int maxRolls = 2;
    private int rollsLeft;

    private bool isRolling = false;
    private float rollTimer = 0f;
    private float cooldownTimer = 0f;
    private Vector3 rollDirection;
    private bool isInMud = false;

    private float currentRollDistance;
    private float currentRollDuration;

    private Vector3 _input;
    private Vector3 _movementDirection;
    private Vector3 lastMovementDirection;

    private CameraZoomController camZoom;

    private float comboWindowTimer = 0f;
    [SerializeField] private float comboWindowDuration = 0.4f;

    public static bool IsAttacking { get; private set; }
    public static void SetAttacking(bool state)
    {
        if (!Instance.isRolling || state == false)
            IsAttacking = state;
    }

    public Vector3 movementDirection => _movementDirection;
    public float TerrainSpeedMultiplier => terrainSpeedMultiplier;
    public bool IsSprinting => Input.GetKey(sprintKey) && !isInMud;

    private static PlayerController Instance;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        camZoom = FindObjectOfType<CameraZoomController>();
        rollsLeft = maxRolls;
        Instance = this;
    }

    private void Update()
    {
        if (rollsLeft == 0)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                rollsLeft = maxRolls;
                cooldownTimer = 0f;
            }
        }

        if (comboWindowTimer > 0f)
            comboWindowTimer -= Time.deltaTime;

        if (!isRolling && Input.GetKeyDown(KeyCode.Space) && (rollsLeft > 0 || comboWindowTimer > 0f))
        {
            StartRoll();
        }

        if (!isRolling)
        {
            GatherInput();
            Look();
        }
    }

    private void FixedUpdate()
    {
        if (isRolling)
        {
            DoRollDash();
            return;
        }

        Move();
    }

    private void GatherInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        Vector3 targetDirection = _input.ToIso().normalized;

        if (targetDirection != Vector3.zero)
        {
            lastMovementDirection = targetDirection;
        }

        _movementDirection = Vector3.Lerp(_movementDirection, targetDirection, acceleration * Time.deltaTime);
    }

    private void Look()
    {
        if (_movementDirection == Vector3.zero) return;

        Quaternion rot = Quaternion.LookRotation(_movementDirection, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);
    }

    private void Move()
    {
        bool canSprint = Input.GetKey(sprintKey) && !isInMud;
        float baseSpeed = canSprint ? _speed * sprintMultiplier : _speed;
        float currentSpeed = baseSpeed * terrainSpeedMultiplier;

        _rb.MovePosition(transform.position + _movementDirection * currentSpeed * Time.deltaTime);
    }

    private void StartRoll()
    {
        isRolling = true;
        rollTimer = 0f;

        if (rollsLeft > 0)
        {
            rollsLeft--;
            if (rollsLeft == 0)
            {
                cooldownTimer = rollCooldown;
            }
        }

        currentRollDistance = isInMud ? mudRollDistance : rollDistance;
        currentRollDuration = isInMud ? mudRollDuration : rollDuration;

        rollDirection = _movementDirection != Vector3.zero ? _movementDirection : lastMovementDirection;
        rollDirection.y = 0f;
        rollDirection.Normalize();

        if (rollDirection == Vector3.zero)
        {
            rollDirection = Vector3.forward;
        }

        camZoom?.TriggerShake();
        IsAttacking = false; // make sure roll cancels any attack
    }

    private void DoRollDash()
    {
        rollTimer += Time.fixedDeltaTime;

        float percent = rollTimer / currentRollDuration;
        percent = Mathf.Clamp01(percent);

        float curveMultiplier = rollCurve != null ? rollCurve.Evaluate(percent) : 1f;
        float rollSpeed = (currentRollDistance / currentRollDuration) * curveMultiplier;

        Vector3 movement = rollDirection * rollSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(_rb.position + movement);

        if (rollTimer >= currentRollDuration)
        {
            isRolling = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mud"))
        {
            terrainSpeedMultiplier = 0.5f;
            isInMud = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Mud"))
        {
            terrainSpeedMultiplier = 1f;
            isInMud = false;
        }
    }

    public void OpenComboWindow()
    {
        comboWindowTimer = comboWindowDuration;
    }
}

public static class Helpers
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
}
