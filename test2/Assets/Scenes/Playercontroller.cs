using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _turnSpeed = 1000f;
    [SerializeField] private float acceleration = 10f;

    [Header("Sprint")]
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Roll Dash")]
    [SerializeField] private float rollDistance = 5f;
    [SerializeField] private float rollDuration = 0.3f;
    [SerializeField] private float rollCooldown = 1f;
    [SerializeField] private AnimationCurve rollCurve;

    private bool isRolling = false;
    private float rollTimer = 0f;
    private float cooldownTimer = 0f;
    private Vector3 rollDirection;

    private Vector3 _input;
    private Vector3 _movementDirection;
    private Vector3 lastMovementDirection;

    public Vector3 movementDirection => _movementDirection;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (!isRolling && Input.GetKeyDown(KeyCode.Space) && cooldownTimer <= 0f)
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
        float currentSpeed = Input.GetKey(sprintKey) ? _speed * sprintMultiplier : _speed;
        _rb.MovePosition(transform.position + _movementDirection * currentSpeed * Time.deltaTime);
    }

    private void StartRoll()
    {
        isRolling = true;
        rollTimer = 0f;
        cooldownTimer = rollCooldown;

        rollDirection = _movementDirection != Vector3.zero ? _movementDirection : lastMovementDirection;
        rollDirection.y = 0f;
        rollDirection.Normalize();

        if (rollDirection == Vector3.zero)
        {
            rollDirection = Vector3.forward;
        }
    }

    private void DoRollDash()
    {
        rollTimer += Time.fixedDeltaTime;

        float percent = rollTimer / rollDuration;
        percent = Mathf.Clamp01(percent);

        float curveMultiplier = rollCurve != null ? rollCurve.Evaluate(percent) : 1f;
        float rollSpeed = (rollDistance / rollDuration) * curveMultiplier;

        Vector3 movement = rollDirection * rollSpeed * Time.fixedDeltaTime;
        transform.position += movement;

        if (rollTimer >= rollDuration)
        {
            isRolling = false;
        }
    }
}

public static class Helpers
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
}
