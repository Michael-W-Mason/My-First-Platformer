using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")]
public class PlayerModel : ScriptableObject
{
    [Header("Gravity")]
    [HideInInspector] public float gravityStrength;
    [HideInInspector] public float gravityScale;

    [Space(5)]
    public float fallGravityMultiplier;
    public float maxFallSpeed;
    [Space(5)]

    public float fastFallGravityMultiplier;
    public float maxFastFallSpeed;

    [Space(20)]

    [Header("Run")]
    public float runMaxSpeed;
    public float runAcceleration;
    [HideInInspector] public float runAccelAmount;
    public float runDecceleration;
    [HideInInspector] public float runDeccelAmount;

    [Space(5)]
    [Range(0, 1f)] public float accelInAir;
    [Range(0, 1f)] public float deccelInAir;

    [Space(5)]
    public bool doConserveMomentum = true;

    [Space(20)]

    [Header("Jump")]
    public float jumpHeight;
    public float jumpTimeToApex;
    [HideInInspector] public float jumpForce;

    [Header("Both Jumps")]
    public float jumpCutGravityMultiplier;
    [Range(0, 1f)] public float jumpHangGravityMultiplier;
    public float jumpHangTimeThreshold;
    [Space(0.5f)]
    public float jumpHangAccelerationMultiplier;
    public float jumpHangMaxSpeedMultiplier;

    [Space(20)]

    [Header("Extras")]
    [Range(0.01f, 0.5f)] public float coyoteTime;
    [Range(0.01f, 0.5f)] public float jumpInputBufferTime;

    private void OnValidate()
    {
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

        gravityScale = gravityStrength / Physics2D.gravity.y;

        runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
        runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

        #region Variable Ranges
        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
        #endregion
    }    
}
