using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float slopeCheckDistance = 0.5f;
    public float maxSlopeAngle = 45f;
    public PhysicsMaterial2D noFrictionMaterial;
    public PhysicsMaterial2D fullFrictionMaterial;
    public Animator animator;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isOnSlope;
    private float horizontalInput;
    private Vector2 slopeNormalPerp;
    private float slopeDownAngle;
    private float slopeDownAngleOld;
    private bool canWalkOnSlope;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get horizontal input
        horizontalInput = Input.GetAxis("Horizontal");

        // Check if the player is on the ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void FixedUpdate()
    {
        // Slope handling
        SlopeCheck();
        HandleAnimation();
        FaceInput();

        if (isOnSlope && canWalkOnSlope)
        {
            rb.velocity = new Vector2(horizontalInput * moveSpeed * slopeNormalPerp.x, horizontalInput * moveSpeed * slopeNormalPerp.y);
        }
        else if (isGrounded)
        {
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        }
    }

    void SlopeCheck()
    {
        Vector2 checkPos = transform.position + new Vector3(0.0f, 0.5f);
        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, groundLayer);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, groundLayer);

        if (slopeHitFront || slopeHitBack)
        {
            isOnSlope = true;
            slopeNormalPerp = Vector2.Perpendicular(slopeHitFront ? slopeHitFront.normal : slopeHitBack.normal).normalized;

            slopeDownAngle = Vector2.Angle(slopeHitFront ? slopeHitFront.normal : slopeHitBack.normal, Vector2.up);

            if (slopeDownAngle <= maxSlopeAngle)
            {
                canWalkOnSlope = true;
                rb.sharedMaterial = fullFrictionMaterial;
            }
            else
            {
                canWalkOnSlope = false;
                rb.sharedMaterial = noFrictionMaterial;
            }
        }
        else
        {
            isOnSlope = false;
            canWalkOnSlope = false;
            rb.sharedMaterial = noFrictionMaterial;
        }
    }

    void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, groundLayer);

        if (hit)
        {
            slopeDownAngleOld = slopeDownAngle;
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != slopeDownAngleOld)
            {
                isOnSlope = true;
            }
        }

        if (slopeDownAngle == 0.0f)
        {
            isOnSlope = false;
        }
    }

    void FaceInput()
    {
        if (horizontalInput > 0.1f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        if (horizontalInput < -0.1f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void HandleAnimation()
    {
        if (isGrounded)
        {
            if (horizontalInput != 0)
            {
                ChangeAnimation("WALK");
            }
            else
            {
                ChangeAnimation("IDLE");
            }
        }
        else
        {
            if (rb.velocity.y > 0.1f)
            {
                ChangeAnimation("JUMP");
            }
            else if (rb.velocity.y < -0.1f)
            {
                ChangeAnimation("FALL");
            }
        }
    }
    public void ChangeAnimation(string animName)
    {
        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != animName)
        {
            animator.Play(animName);
        }
    }
}
