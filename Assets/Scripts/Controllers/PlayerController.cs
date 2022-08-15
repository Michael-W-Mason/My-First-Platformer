using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float SpeedModifier = 10f;
    [SerializeField] float JumpModifier = 100f;
    [SerializeField] Rigidbody2D _rigidbody;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    float horizontalMove;
    bool jumpDown;
    bool jumpUp;

    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        jumpDown = Input.GetButtonDown("Jump");
        jumpUp = Input.GetButtonUp("Jump");
        if (jumpDown && IsGrounded())
        {
            Debug.Log("Down");
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpModifier);
        }
        if (jumpUp && _rigidbody.velocity.y > 0f)
        {
            Debug.Log("Up");
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y * 0.5f);
        }
    }

    void FixedUpdate()
    {
        _rigidbody.velocity = new Vector2(horizontalMove * SpeedModifier, _rigidbody.velocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
}
