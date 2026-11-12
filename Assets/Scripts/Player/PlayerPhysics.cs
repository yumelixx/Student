using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    None,
    Idle,
    GroundedMovement,
    Jumping,
    Firing,
    Hurt,
}

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPhysics : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float gravityModifier;
    [SerializeField] Rigidbody2D rigidBody2D;
    [SerializeField] float jumpSpeed;
    [SerializeField] float hurtJumpSpeed;
    [Header("Grounded")]
    [SerializeField] Vector2 groundCheckOffset;
    [SerializeField] Vector2 groundCheckSize;
    [SerializeField] LayerMask groundMask;
    [Header("Inputs")]
    [SerializeField] InputActionReference moveInput;
    [SerializeField] InputActionReference jumpInput;
    [SerializeField] InputActionReference fireInput;
    [Header("Bullet")]
    [SerializeField] Transform bulletSpawner;
    [SerializeField] Bullet bulletPrefab;

    float horizontalSpeed;
    Vector2 velocity;
    Vector2 knockbackVelocity;

    float initialGravity;
    bool isGrounded;
    bool canJump;
    int direction;
    Vector2 bulletSpawnerInitialPosition;

    StateMachine stateMachine;
    State idleState;
    State movementState;
    State jumpingState;
    State firingState;
    State hurtState;

    void Awake()
    {
        initialGravity = rigidBody2D.gravityScale;
        direction = 1;
        bulletSpawnerInitialPosition = bulletSpawner.localPosition;

        idleState = CreateState(PlayerState.Idle, IdleEnter, IdleUpdate, IdleExit);
        movementState = CreateState(PlayerState.GroundedMovement, MovementEnter, MovementUpdate, MovementExit);
        jumpingState = CreateState(PlayerState.Jumping, JumpEnter, JumpUpdate, JumpExit);
        firingState = CreateState(PlayerState.Firing, FiringEnter, null, null);
        hurtState = CreateState(PlayerState.Hurt, HurtEnter, null, HurtExit);

        stateMachine = new StateMachine();
        stateMachine.AddState(idleState);
        stateMachine.AddState(movementState);
        stateMachine.AddState(jumpingState);
        stateMachine.AddState(firingState);
        stateMachine.AddState(hurtState);
    }

    void OnEnable()
    {
        ChangeState(PlayerState.Idle);
    }

    void OnDisable()
    {
        stateMachine.Stop();
    }

    State CreateState(PlayerState playerState, Action onEnter, Action onUpdate, Action onExit)
    {
        var state = new State(playerState.ToString());
        state.OnEnter += onEnter;
        state.OnUpdate += onUpdate;
        state.OnExit += onExit;

        return state;
    }

    void ChangeState(PlayerState newState)
    {
        stateMachine.ChangeState(newState.ToString());
    }

    public void Hurt(Vector2 knockback)
    {
        knockbackVelocity = knockback;
        ChangeState(PlayerState.Hurt);
    }

    void IdleEnter()
    {
        moveInput.action.started += StartMoveHandler;

        jumpInput.action.started += JumpStartedHandler;

        fireInput.action.started += BulletFiredHandler;
    }

    void IdleUpdate()
    {
        horizontalSpeed = 0.0f;
        velocity = Vector2.zero;

        // The move key may already be held when we arrive here (after landing or firing)
        Vector2 moveVector = moveInput.action.ReadValue<Vector2>();
        if (moveVector.x != 0.0f)
        {
            ChangeState(PlayerState.GroundedMovement);
        }
    }

    void IdleExit()
    {
        moveInput.action.started -= StartMoveHandler;

        jumpInput.action.started -= JumpStartedHandler;

        fireInput.action.started -= BulletFiredHandler;
    }

    void MovementEnter()
    {
        moveInput.action.performed += PerformedMoveHandler;
        moveInput.action.canceled += CanceledMoveHandler;

        jumpInput.action.started += JumpStartedHandler;

        ReadMoveInput();
    }

    void MovementUpdate()
    {
        HorizontalMovement();
    }

    void MovementExit()
    {
        moveInput.action.performed -= PerformedMoveHandler;
        moveInput.action.canceled -= CanceledMoveHandler;

        jumpInput.action.started -= JumpStartedHandler;
    }

    void JumpEnter()
    {
        moveInput.action.performed += PerformedMoveHandler;
        moveInput.action.canceled += CanceledMoveHandler;

        jumpInput.action.canceled += JumpReleasedHandler;

        ReadMoveInput();
        velocity.y = jumpSpeed;
        canJump = true;
    }

    void JumpUpdate()
    {
        HorizontalMovement();
    }

    void JumpExit()
    {
        moveInput.action.performed -= PerformedMoveHandler;
        moveInput.action.canceled -= CanceledMoveHandler;

        jumpInput.action.canceled -= JumpReleasedHandler;

        velocity.y = 0.0f;
        canJump = false;
    }

    void FiringEnter()
    {
        var bullet = Instantiate(bulletPrefab,
            bulletSpawner.position,
            Quaternion.identity);
        bullet.SetDirection(direction);

        ChangeState(PlayerState.Idle);
    }

    void HurtEnter()
    {
        horizontalSpeed = 0.0f;
        velocity = knockbackVelocity;
        velocity.y += hurtJumpSpeed;
        canJump = true;
    }

    void HurtExit()
    {
        velocity = Vector2.zero;
        canJump = false;
    }

    void ReadMoveInput()
    {
        Vector2 moveVector = moveInput.action.ReadValue<Vector2>();
        horizontalSpeed = moveVector.x * speed;
    }

    void HorizontalMovement()
    {
        velocity.x = horizontalSpeed;

        if (velocity.x > 0)
        {
            direction = 1;
        }
        else if (velocity.x < 0)
        {
            direction = -1;
        }

        var bulletSpawnerPosition = bulletSpawner.localPosition;
        bulletSpawnerPosition.x = direction * bulletSpawnerInitialPosition.x;
        bulletSpawner.localPosition = bulletSpawnerPosition;
    }

    void StartMoveHandler(InputAction.CallbackContext ctx)
    {
        ChangeState(PlayerState.GroundedMovement);
    }

    void PerformedMoveHandler(InputAction.CallbackContext ctx)
    {
        var moveVector = ctx.ReadValue<Vector2>();
        horizontalSpeed = moveVector.x * speed;
    }

    void CanceledMoveHandler(InputAction.CallbackContext ctx)
    {
        horizontalSpeed = 0.0f;

        if (stateMachine.CurrentState == PlayerState.GroundedMovement.ToString())
        {
            ChangeState(PlayerState.Idle);
        }
    }

    void JumpStartedHandler(InputAction.CallbackContext ctx)
    {
        if (isGrounded)
        {
            ChangeState(PlayerState.Jumping);
        }
    }

    void JumpReleasedHandler(InputAction.CallbackContext ctx)
    {
        if (rigidBody2D.linearVelocityY > 0.0f)
        {
            velocity.y = 0.0f;
            canJump = true;
        }
    }

    void BulletFiredHandler(InputAction.CallbackContext ctx)
    {
        ChangeState(PlayerState.Firing);
    }

    void Update()
    {
        stateMachine.UpdateState();
    }

    void FixedUpdate()
    {
        rigidBody2D.gravityScale = initialGravity * gravityModifier;
        rigidBody2D.linearVelocityX = velocity.x;

        CheckJumping();
        CheckGround();
    }

    void CheckJumping()
    {
        if (canJump)
        {
            rigidBody2D.linearVelocityY = velocity.y;
            canJump = false;
        }
    }

    void CheckGround()
    {
        Collider2D colliderDetected = Physics2D.OverlapBox(
            rigidBody2D.position + groundCheckOffset,
            groundCheckSize,
            0.0f,
            groundMask);
        bool currentGroundedState = colliderDetected != null;
        if (currentGroundedState && !isGrounded)
        {
            ChangeState(PlayerState.Idle);
        }
        isGrounded = currentGroundedState;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            rigidBody2D.position + groundCheckOffset,
            groundCheckSize);
    }
}
