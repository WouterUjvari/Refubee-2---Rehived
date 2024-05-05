using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float xInput;
    [SerializeField] private float yInput;
    public bool jumpInput;
    public bool stunned;
    public float bounceCooldown;
    public float damageCooldown;
    public float dealdamageCooldown;
    public float knockbackCooldown;
    public float timeSinceLastXInput;
    public float jumpCooldown;
    public bool punching;
    public bool killed;
    public bool flame;
    public bool slope;
    private float speedmult;

    private float underwatertimer;
    //public int healthPoints, maxHealthPoints;
    //public int points;
    [Header("ExperimentalForces")]
    [SerializeField] private float acceleration;
    [SerializeField] private float decceleration;
    [SerializeField] private float velPower;

    [Header("Platforming settings")]
    [SerializeField] private float moveSpeedPlatforming;
    [SerializeField] private float jumpSpeedPlatforming;
    [SerializeField] private float gravityScalePlatforming;
    [SerializeField] private float dragPlatforming;
    public bool standsOnLedge;
    private Collider2D ledgeCol;
    [Header("Swimming settings")]
    [SerializeField] private float moveSpeedSwimming;
    [SerializeField] private float maxSpeedSwimming;



    [Header("Components")]
    public Animator animator;
    public Animator fadeAnimator;
    public Rigidbody2D rbPlayer;
    [SerializeField] private Collider2D groundCheckCollider;
    public Collider2D bodyCollider;
    [SerializeField] private PhysicsMaterial2D friction;
    [SerializeField] private PhysicsMaterial2D nofriction;
    public Collider2D antennaCollider;
    [SerializeField] private Collider2D punchCollider;
    public Transform heldItem;
    public float pickupcooldown;
    public Keybee keybee;
    [SerializeField] private bool grounded;
    [SerializeField] private LayerMask groundmask;
    [SerializeField] private LayerMask watermask;

    public enum ControlType { Platforming, Swimming, Sprinting};
    public ControlType currentControlType;


    private void Start()
    {
        GameManager.Instance.playerScript = GetComponent<PlayerScript>();
    }
    private void Update()
    {
        if (GameManager.Instance.readControls)
        {
            xInput = Input.GetAxisRaw("Horizontal");
            yInput = Input.GetAxisRaw("Vertical");
            jumpInput = Input.GetButton("Jump");
            if(Math.Abs(rbPlayer.velocity.x) < 1)
            {
                timeSinceLastXInput = Mathf.Clamp(timeSinceLastXInput += Time.deltaTime, 0, 1);
            }
            else
            {
                timeSinceLastXInput = 0;
            }
        }

        if (standsOnLedge) 
        {
            rbPlayer.gravityScale = gravityScalePlatforming;
        }
        else if(grounded)
        {
            rbPlayer.gravityScale = 0;
        }
        else 
        { 
            rbPlayer.gravityScale = gravityScalePlatforming;
        }

        jumpCooldown = Mathf.Clamp(jumpCooldown += Time.deltaTime, 0, 1);
        bounceCooldown = Mathf.Clamp(bounceCooldown += Time.deltaTime, 0, 2);
        damageCooldown = Mathf.Clamp(damageCooldown += Time.deltaTime, 0, 2);
        knockbackCooldown = Mathf.Clamp(knockbackCooldown += Time.deltaTime, 0, 2);
        dealdamageCooldown = Mathf.Clamp(dealdamageCooldown += Time.deltaTime, 0, 2);
        pickupcooldown = Mathf.Clamp(pickupcooldown += Time.deltaTime, 0, 1);

        //HandleGroundCheck();
        HandleGroundCheck();
        

        if(Input.GetButtonDown("Fire1"))
        {
            if (!punching && currentControlType == ControlType.Platforming && !stunned && GameManager.Instance.readControls)
            {
                StartCoroutine(Punch());
            }
        }


        if (Input.GetKeyDown("g"))
        {
            RaycastHit2D hit = Physics2D.Raycast(this.gameObject.transform.position, Vector2.down);
            if (hit.collider != null)
            {

            }
        }

        if (flame)
        {
            Physics2D.IgnoreLayerCollision(6, 17, false);
        }
        else
        {
            Physics2D.IgnoreLayerCollision(6, 17, true);
        }
    }
    

    private void FixedUpdate()
    {
        if (true)
        {
            HandleControlType();
        }

        HandleStuns();

    }

    void HandleControlType()
    {
        if (Physics2D.OverlapAreaAll(bodyCollider.bounds.min, bodyCollider.bounds.max, watermask).Length > 0)
        {
            currentControlType = ControlType.Swimming;
        }
        else if(currentControlType != ControlType.Sprinting)
        {
            currentControlType = ControlType.Platforming;
        }

        if (currentControlType == ControlType.Platforming && !stunned)
        {
            HandlePlatformingMovement();
        }
        else if (currentControlType == ControlType.Sprinting && !stunned)
        {
            HandleSprintingMovement();
        }
        else if (currentControlType == ControlType.Swimming)
        {
            HandleSwimmingMovement();
        }
    }

    void HandlePlatformingMovement()
    {
        flame = false;
        // Calculates movementforce
        float targetSpeed = xInput * moveSpeedPlatforming;
        float speedDif = targetSpeed - rbPlayer.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        rbPlayer.AddForce(movement * Vector2.right);

        //rbPlayer.gravityScale = gravityScalePlatforming;
        rbPlayer.drag = dragPlatforming;

        if (grounded && !punching)
        {
            if (jumpInput && jumpCooldown > 0.2f)
            {
                if (standsOnLedge && yInput >= 0)
                {
                    jumpCooldown = 0;
                    grounded = false;
                    jumpInput = false;
                    rbPlayer.velocity = new Vector2(rbPlayer.velocity.x, 0);
                    rbPlayer.AddForce(new Vector2(0, 600));
                    grounded = false;

                }
                if (!standsOnLedge)
                {
                    jumpCooldown = 0;
                    grounded = false;
                    jumpInput = false;
                    rbPlayer.velocity = new Vector2(rbPlayer.velocity.x, 0);
                    rbPlayer.AddForce(new Vector2(0, 600));
                    grounded = false;
                }
            }
            if (true)
            {
                RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, Vector2.down, 0.25f);
                if (hit.Length > 0)
                {
                    for (int i = 0; i < hit.Length; i++)
                    {
                        if (hit[i].collider.gameObject.GetComponent<PlatformEffector2D>() != null)
                        {
                            standsOnLedge = true;
                            ledgeCol = hit[i].collider.gameObject.GetComponent<Collider2D>();
                        }
                        else
                        {
                            standsOnLedge = false;
                            ledgeCol = null;
                        }
                    }
                }
            }
            if (standsOnLedge && yInput < 0)
            {
                Debug.Log("Ledgejump");
                jumpCooldown = 0;
                grounded = false;
                StartCoroutine(LedgeJump(ledgeCol));
            }
        }

        if (!punching && !stunned)
        {
            if (grounded)
            {
                if (xInput != 0)
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
                if (rbPlayer.velocity.y > 0)
                {
                    if (!Input.GetButton("Jump"))
                    {
                        //jumpcancel very buggy
                        //jumpInput = false;
                        //rbPlayer.velocity = new Vector2(rbPlayer.velocity.x, 0);
                    }
                }
                if (rbPlayer.velocity.y > 0.1f)
                {
                    ChangeAnimation("JUMP");
                }
                else if (rbPlayer.velocity.y < -0.1f)
                {
                    ChangeAnimation("FALL");
                }
            }
        }
        
        FaceInput();
        underwatertimer = 0;
    }
    void HandleSprintingMovement()
    {
        if (MathF.Abs(rbPlayer.velocity.x) < 10)
        {
            ChangeAnimation("SPRINTFASTER");
            flame = false;
            speedmult = 1f;
        }
        else if (MathF.Abs(rbPlayer.velocity.x) < 25)
        {
            ChangeAnimation("SPRINTFASTEST");
            flame = true;
            speedmult = 2f;
        }
        else if (MathF.Abs(rbPlayer.velocity.x) < 100)
        {
            ChangeAnimation("SPRINTMAX");
            flame = true;
            speedmult = 3f;
        }
        if (currentControlType == ControlType.Sprinting)
        {
            if (MathF.Abs(rbPlayer.velocity.x) < 4)
            {
                flame = false;
                currentControlType = ControlType.Platforming;

            }
        }

        // Calculates movementforce
        float targetSpeed = xInput * moveSpeedPlatforming *4 * speedmult;
        float speedDif = targetSpeed - rbPlayer.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration /8 : decceleration /8;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        rbPlayer.AddForce(movement * Vector2.right);









        //rbPlayer.gravityScale = gravityScalePlatforming;
        rbPlayer.drag = dragPlatforming;

        if (grounded)
        {
            if (jumpInput && jumpCooldown > 0.5f)
            {
                if (standsOnLedge && yInput >= 0)
                {
                    jumpCooldown = 0;
                    grounded = false;
                    jumpInput = false;
                    rbPlayer.velocity = new Vector2(rbPlayer.velocity.x, 0);
                    rbPlayer.AddForce(new Vector2(0, 600));
                    grounded = false;
                }
                if (!standsOnLedge)
                {
                    jumpCooldown = 0;
                    grounded = false;
                    jumpInput = false;
                    rbPlayer.velocity = new Vector2(rbPlayer.velocity.x, 0);
                    rbPlayer.AddForce(new Vector2(0, 600));
                    grounded = false;
                }
            }
            if (yInput < 0)
            {
                RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, Vector2.down, 0.25f);
                if (hit.Length > 0)
                {
                    for (int i = 0; i < hit.Length; i++)
                    {
                        if (hit[i].collider.gameObject.GetComponent<PlatformEffector2D>() != null)
                        {
                            standsOnLedge = true;
                            ledgeCol = hit[i].collider.gameObject.GetComponent<Collider2D>();
                        }
                        else
                        {
                            standsOnLedge = false;
                            ledgeCol = null;
                        }
                    }
                }
            }
            if (standsOnLedge && yInput < 0)
            {
                Debug.Log("Ledgejump");
                jumpCooldown = 0;
                grounded = false;
                StartCoroutine(LedgeJump(ledgeCol));
            }
        }



        
        FaceInput();
        underwatertimer = 0;
    }

    private IEnumerator LedgeJump(Collider2D col)
    {
        Physics2D.IgnoreCollision(bodyCollider, col);
        yield return new WaitForSeconds(0.5f);       
        Physics2D.IgnoreCollision(bodyCollider, col, false);
        standsOnLedge = false;
    }
    void HandleSwimmingMovement()
    {
        stunned = false;
        flame = false;
        underwatertimer += Time.deltaTime;

        float targetSpeed = xInput * moveSpeedSwimming;
        float speedDif = targetSpeed - rbPlayer.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        rbPlayer.AddForce(movement * Vector2.right);

        if (underwatertimer > 0.2f)
        {
            if (Input.GetButton("Jump"))
            {
                if (Physics2D.OverlapAreaAll(antennaCollider.bounds.min, antennaCollider.bounds.max, watermask).Length == 0)
                {
                    if(jumpCooldown > 0.2f)
                    {
                        jumpCooldown = 0;
                        Debug.Log("Dive!");
                        rbPlayer.velocity = Vector2.zero;

                        rbPlayer.AddForce(new Vector2(0, 400));
                        underwatertimer = 0;
                    }
                    
                    
                }
            }
        }
        
        FaceInput();
        if (grounded)
        {
            if (xInput == 0)
            {
                ChangeAnimation("IDLE");
            }
            else
            {
                {
                    ChangeAnimation("WALK");
                }
            }
        }
        else
        {
            ChangeAnimation("FALL");
        }
    }

    void HandleGroundCheck()
    {
        grounded = Physics2D.OverlapAreaAll(groundCheckCollider.bounds.min, groundCheckCollider.bounds.max, groundmask).Length > 0;
    }


    IEnumerator Punch()
    {
        punching = true;
        animator.Play("PUNCH");
        yield return new WaitForSeconds(0.1f);
        rbPlayer.velocity = Vector2.zero;
        rbPlayer.AddForce(new Vector2(600 * transform.localScale.x, 200));
        punchCollider.enabled = true;     
        yield return new WaitForSeconds(0.2f);
        punchCollider.enabled = false;
        yield return new WaitForSeconds(0.1f);
        punching = false;
        if (!stunned)
        {
            if (Mathf.Abs(xInput) > 0.75f && Input.GetButton("Fire1"))
            {
                currentControlType = ControlType.Sprinting;
            }
        }
    }
    IEnumerator KeepSlippery(float time)
    {
        decceleration = 5f;
        acceleration = 2f;
        yield return new WaitForSeconds(time);
        acceleration = 5f;
        decceleration = 10;
    }

    void HandleStuns()
    {
        
        if (grounded && damageCooldown > 0.2f)
        {
            stunned = false;
        }
        if (stunned)
        {
            ChangeAnimation("SWIM");
            //bodyCollider.sharedMaterial = friction;
        }
        else
        {
            //bodyCollider.sharedMaterial = nofriction;
        }
        //StartCoroutine(UnStunBackup());
    }

    IEnumerator UnStunBackup()
    {
        yield return new WaitForSeconds(2);
        stunned = false;
    }
    IEnumerator DelayJump(float delay)
    {
        yield return new WaitForSeconds(delay);
        rbPlayer.AddForce(new Vector2(0, 600));
    }


    void FaceInput()
    {
        if (xInput > 0.1f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        if (xInput < -0.1f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    
    public void ChangeAnimation(string animName)
    {
        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != animName)
        {
            animator.Play(animName);
        }
    }
 
    public void TakeKnockback(float amountX, float amountY)
    {
       if(knockbackCooldown > 0.2f)
       {
            if (!flame)
            {
                rbPlayer.velocity = Vector2.zero;
            }
           knockbackCooldown = 0;
           rbPlayer.AddForce(new Vector2(amountX, amountY));
       }
       
    }

    public void DeathSequence()
    {
        float chance = UnityEngine.Random.Range(0, 1);
        grounded = false;
        rbPlayer.gravityScale = gravityScalePlatforming;
        bodyCollider.enabled = false;
        rbPlayer.velocity = Vector2.zero;
        rbPlayer.angularVelocity = 0;
        rbPlayer.freezeRotation = false;
        rbPlayer.AddForce(new Vector2(UnityEngine.Random.Range(-200, 200), 300));
        if(chance <= 0.5f)
        {
            rbPlayer.AddTorque(400);
        }
        else
        {
            rbPlayer.AddTorque(-400);
        }
        
        ChangeAnimation("SWIM");
        Destroy(gameObject, 1);
        GetComponent<PlayerScript>().enabled = false;
    }
}
