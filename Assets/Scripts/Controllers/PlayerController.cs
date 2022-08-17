using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerModel playerData;

    #region Variables
    public Rigidbody2D RB { get; private set; }
    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; private set; }
    public float LastOnGroundTime { get; private set; }
    private bool _isJumpCut;
    private bool _isJumpFalling;

    private Vector2 _moveInput;
    public float LastPressedJumpTime { get; private set; }

    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);

    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;
    #endregion

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        SetGravityScale(playerData.gravityScale);
        IsFacingRight = true;
    }

    private void Update()
    {
        #region Timers
        LastOnGroundTime -= Time.deltaTime;
        LastPressedJumpTime -= Time.deltaTime;
        #endregion

        #region Input Handling
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");

        if (_moveInput.x != 0)
        {
            CheckDirectionToFace(_moveInput.x > 0);
        }
        if (Input.GetButtonDown("Jump"))
        {
            OnJumpInput();
        }
        if (Input.GetButtonUp("Jump"))
        {
            OnJumpUpInput();
        }
        #endregion

        #region Collision Checks
        if (!IsJumping)
        {
            // Ground Check
            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !IsJumping)
            {
                LastOnGroundTime = playerData.coyoteTime;
            }
        }
        #endregion

        #region Jump Checks
        if (IsJumping && RB.velocity.y < 0)
        {
            IsJumping = false;
        }

        if (LastOnGroundTime > 0 && !IsJumping)
        {
            _isJumpCut = false;
            if (!IsJumping)
            {
                _isJumpFalling = false;
            }
        }

        // Jumping
        if (CanJump() && LastPressedJumpTime > 0)
        {
            IsJumping = true;
            _isJumpCut = false;
            _isJumpFalling = false;
            Jump();
        }
        #endregion

        #region Gravity
        if (RB.velocity.y < 0 && _moveInput.y < 0)
        {
            SetGravityScale(playerData.gravityScale * playerData.fastFallGravityMultiplier);
            RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -playerData.maxFallSpeed));
        }
        else if (_isJumpCut)
        {
            SetGravityScale(playerData.gravityScale * playerData.jumpCutGravityMultiplier);
            RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -playerData.maxFallSpeed));
        }
        else if (IsJumping && Mathf.Abs(RB.velocity.y) < playerData.jumpHangTimeThreshold)
        {
            SetGravityScale(playerData.gravityScale * playerData.jumpHangGravityMultiplier);
        }
        else if (RB.velocity.y < 0)
        {
            SetGravityScale(playerData.gravityScale * playerData.fallGravityMultiplier);
        }
        else
        {
            SetGravityScale(playerData.gravityScale);
        }
        #endregion
    }

    private void FixedUpdate()
    {
        Run(1);
    }
    public void SetGravityScale(float scale)
    {
        RB.gravityScale = scale;
    }

    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
        {
            Turn();
        }
    }

    private void Run(float lerpAmount)
    {
        float targetSpeed = _moveInput.x * playerData.runMaxSpeed;
        targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

        #region Calculate Accelertaion Rate
        float accelRate;

        if (LastOnGroundTime > 0)
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? playerData.runAccelAmount : playerData.runDeccelAmount;
        }
        else
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? playerData.runAccelAmount * playerData.accelInAir : playerData.runDeccelAmount * playerData.deccelInAir;
        }
        #endregion

        #region Add Bonus Jump Apex Acceleration
        if ((IsJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < playerData.jumpHangTimeThreshold)
        {
            accelRate *= playerData.jumpHangAccelerationMultiplier;
            targetSpeed *= playerData.jumpHangMaxSpeedMultiplier;
        }
        #endregion

        #region Conserve Momentum
        if (playerData.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            accelRate = 0;
        }
        #endregion

        float speedDif = targetSpeed - RB.velocity.x;

        float movement = speedDif * accelRate;

        RB.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    public void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        IsFacingRight = !IsFacingRight;
    }

    public void OnJumpInput()
    {
        LastPressedJumpTime = playerData.jumpInputBufferTime;
    }

    public void OnJumpUpInput()
    {
        if (CanJumpCut())
        {
            _isJumpCut = true;
        }
    }

    private bool CanJumpCut()
    {
        return IsJumping && RB.velocity.y > 0;
    }

    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !IsJumping;
    }

    private void Jump()
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        #region Perform Jump
        float force = playerData.jumpForce;
        if (RB.velocity.y < 0)
        {
            force -= RB.velocity.y;
        }
        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        #endregion
    }
}
