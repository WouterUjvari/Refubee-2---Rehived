using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using static PlayerScript;
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerScript : MonoBehaviour
{
    public float measureSpeedY;
    public float measureSpeedX;
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
    private float accelmult;

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
    [SerializeField] private LayerMask groundmask;
    [SerializeField] private LayerMask ledgemask;
    [SerializeField] private LayerMask watermask;
    [SerializeField] private LayerMask playerlayer;

    public enum ControlType { Platforming, Swimming, Sprinting};
    public ControlType currentControlType;

    /// <summary>
    /// ////////////////////////////////////////////////////////////////////
    /// </summary>
    private CapsuleCollider2D _col;
    private FrameInput _frameInput;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;

    #region Interface

    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;

    #endregion

    private float _time;

    private void Start()
    {
        GameManager.Instance.playerScript = GetComponent<PlayerScript>();
        _col = GetComponent<CapsuleCollider2D>();
        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
    }
    private void Update()
    {
        GatherInput();
        _time += Time.deltaTime;
        measureSpeedX = rbPlayer.velocity.x;
        measureSpeedY = rbPlayer.velocity.y;
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

            //run
            if(!stunned && _grounded && !punching)
            {
                if (Input.GetButton("Fire2"))
                {
                    if(currentControlType == ControlType.Platforming)
                    {
                        if(MathF.Abs(rbPlayer.velocity.x) >= 3)
                        {
                            currentControlType = ControlType.Sprinting;
                        }
                        
                    }
                }
            }
        }

        if (_grounded && MathF.Abs(xInput) == 0)
        {
            rbPlayer.sharedMaterial = friction;
            bodyCollider.sharedMaterial = friction;
        }
        else
        {
            rbPlayer.sharedMaterial = nofriction;
            bodyCollider.sharedMaterial = nofriction;
        }
        {
            //rbPlayer.gravityScale = gravityScalePlatforming;
        }
        

        

        jumpCooldown = Mathf.Clamp(jumpCooldown += Time.deltaTime, 0, 1);
        bounceCooldown = Mathf.Clamp(bounceCooldown += Time.deltaTime, 0, 2);
        damageCooldown = Mathf.Clamp(damageCooldown += Time.deltaTime, 0, 2);
        knockbackCooldown = Mathf.Clamp(knockbackCooldown += Time.deltaTime, 0, 2);
        dealdamageCooldown = Mathf.Clamp(dealdamageCooldown += Time.deltaTime, 0, 2);
        pickupcooldown = Mathf.Clamp(pickupcooldown += Time.deltaTime, 0, 1);



        

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

        if(_grounded)
        {
            rbPlayer.gravityScale = 0.1f;
        }
        else
        {
            rbPlayer.gravityScale = gravityScalePlatforming;
        }
    }
    

    private void FixedUpdate()
    {
        if (true)
        {
            HandleControlType();
        }

        HandleStuns();
        CheckCollisions();
        HandleJump();
        
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
        rbPlayer.mass = 1;
        flame = false;
        // Calculates movementforce
        float targetSpeed = xInput * moveSpeedPlatforming;
        float speedDif = targetSpeed - rbPlayer.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? (acceleration * 1) : (decceleration * 1);
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        rbPlayer.AddForce(movement * Vector2.right);


        rbPlayer.drag = dragPlatforming;

        if (_grounded && !punching)
        {
            if (true)
            {
                RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, Vector2.down, 0.25f);
                Collider2D col = Physics2D.OverlapArea(groundCheckCollider.bounds.min, groundCheckCollider.bounds.max, ledgemask);
                if (true)
                {
                    if (true)
                    {
                        if (col != null)
                        {
                            
                            standsOnLedge = true;
                            col.gameObject.GetComponent<Collider2D>();
                        }
                        else
                        {
                            standsOnLedge = false;
                            ledgeCol = null;
                        }
                    }
                }
            }
            if (standsOnLedge && yInput < -0.25f && measureSpeedY >= 0)
            {
                Debug.Log("Ledgejump");
                jumpCooldown = 0;
                _grounded = false;
                StartCoroutine(LedgeJump());
            }
        }

        if (!punching && !stunned)
        {
            if (_grounded)
            {
                if (xInput != 0)
                {
                    ChangeAnimation("WALK");
                }
                else if(xInput == 0)
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
                if (rbPlayer.velocity.y > 0.1f && !punching)
                {
                    ChangeAnimation("JUMP");
                }
                else if (rbPlayer.velocity.y < -0.1f && !punching)
                {
                    ChangeAnimation("FALL");
                }
            }
        }
        if (_grounded)
        {
            //rbPlayer.gravityScale = 0;
        }
        else
        {
            //rbPlayer.gravityScale = gravityScalePlatforming;
        }
        FaceInput();
        HandleGravity();
        underwatertimer = 0;
    }
    void HandleSprintingMovement()
    {
        rbPlayer.sharedMaterial = nofriction;
        bodyCollider.sharedMaterial = nofriction;
        if (MathF.Abs(rbPlayer.velocity.x) < 5)
        {
            ChangeAnimation("SPRINT");
            flame = false;
            speedmult = 0.75f;
            accelmult = 0.75f;
        }
        else if (MathF.Abs(rbPlayer.velocity.x) < 10)
        {
            ChangeAnimation("SPRINTFASTER");
            flame = false;
            speedmult = 1f;
            accelmult = 1;
        }
        else if (MathF.Abs(rbPlayer.velocity.x) < 18)
        {
            ChangeAnimation("SPRINTFASTEST");
            flame = true;
            speedmult = 1.25f;

            accelmult = 2;
        }
        else if (MathF.Abs(rbPlayer.velocity.x) < 100)
        {
            ChangeAnimation("SPRINTMAX");
            flame = true;
            speedmult = 2f;
            accelmult = 3;
        }
        if (currentControlType == ControlType.Sprinting)
        {
            if (MathF.Abs(rbPlayer.velocity.x) < 2f)
            {
                flame = false;
                currentControlType = ControlType.Platforming;

            }
        }

        // Calculates movementforce
        float targetSpeed = xInput * moveSpeedPlatforming *4 * speedmult;
        float speedDif = targetSpeed - rbPlayer.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration /8 * accelmult : decceleration /8;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        rbPlayer.AddForce(movement * Vector2.right);









        //rbPlayer.gravityScale = gravityScalePlatforming;
        rbPlayer.drag = dragPlatforming;

        if (_grounded)
        {
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
                _grounded = false;
                StartCoroutine(LedgeJump());
            }
        }
        if (_grounded)
        {
            //rbPlayer.gravityScale = 0;
        }
        else
        {
            //rbPlayer.gravityScale = gravityScalePlatforming;
        }
        FaceInput();
        HandleGravity();
        underwatertimer = 0;
    }
    private IEnumerator LedgeJump()
    {
        //Collider2D col = Physics2D.OverlapAreaAll(groundCheckCollider.bounds.min, groundCheckCollider.bounds.max, ledgemask);
        Collider2D col = Physics2D.OverlapArea(groundCheckCollider.bounds.min, groundCheckCollider.bounds.max, ledgemask);
        if(col != null)
        {
            Physics2D.IgnoreCollision(bodyCollider, col, true);
        }
        
        rbPlayer.gravityScale = gravityScalePlatforming;
        rbPlayer.velocity = new Vector2(rbPlayer.velocity.x, -1);
        yield return new WaitForSeconds(0.5f);

        if (col != null)
        {
            Physics2D.IgnoreCollision(bodyCollider, col, false);
        }

        standsOnLedge = false;
    }
    void HandleSwimmingMovement()
    {
        _coyoteUsable = false;
        _jumpToConsume = false;
        _bufferedJumpUsable = false;

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
            if (_frameInput.JumpHeld)
            {
                if (Physics2D.OverlapAreaAll(antennaCollider.bounds.min, antennaCollider.bounds.max, watermask).Length == 0)
                {
                    if(jumpCooldown > 0.2f)
                    {
                        jumpCooldown = 0;
                        Debug.Log("Dive!");
                        rbPlayer.velocity = Vector2.zero;

                        rbPlayer.AddForce(new Vector2(0, 700));
                        underwatertimer = 0;
                    }
                    
                    
                }
            }
        }
        if (_frameInput.JumpHeld)
        {
            rbPlayer.mass = 1f;

        }
        else
        {
            rbPlayer.mass = 2f;
        }
        rbPlayer.drag = 2;
        
        FaceInput();
        if (_grounded)
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
        if (_grounded && damageCooldown > 0.2f)
        {
            stunned = false;
        }
        if (stunned)
        {
            ChangeAnimation("SWIM");

        }
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
        _grounded = false;
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
        Destroy(gameObject, 3);
        //GetComponent<PlayerScript>().enabled = false;
        StartCoroutine(Reload());
    }

    IEnumerator Reload() 
    {

        yield return new WaitForSeconds(1);
        Hud.Instance.fadeAnimator.Play("FadeEffect_CircleZoomIn");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(0);
    }

    //new stuff
    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    void GatherInput()
    {
        _frameInput = new FrameInput
        {
            JumpDown = Input.GetButton("Jump") || Input.GetKeyDown(KeyCode.C),
            JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
        };

        if (_frameInput.JumpDown)
        {
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;
        }
    }

    private float _frameLeftGrounded = float.MinValue;
    private bool _grounded;
    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        // Ground and Ceiling
        bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, 0.1f, groundmask);
        bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, 0.1f, groundmask);

        // Hit a Ceiling
        if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

        // Landed on the Ground
        if (!_grounded && groundHit && measureSpeedY <= 0.01f)
        {
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
        }
        // Left the Ground
        else if (_grounded && !groundHit)
        {
            _grounded = false;
            _frameLeftGrounded = _time;
            GroundedChanged?.Invoke(false, 0);
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }
    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _endedJumpEarly;
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;

    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + 0.2f;
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + 0.15f;

    private void HandleJump()
    {
        if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && rbPlayer.velocity.y > 0) _endedJumpEarly = true;

        if (!_jumpToConsume && !HasBufferedJump) return;

        if (_grounded || CanUseCoyote) ExecuteJump();

        _jumpToConsume = false;
    }
    private void ExecuteJump()
    {
        Debug.Log("JUMP NOW!");
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        rbPlayer.velocity = new Vector2(rbPlayer.velocity.x, 0);
        rbPlayer.AddForce(new Vector2(0, 600));
        Jumped?.Invoke();
    }
    private void HandleGravity()
    {
        if (_grounded && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = -1.5f;
        }
        else
        {
            var inAirGravity = 110;
            if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= 3;
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, 40, inAirGravity * Time.fixedDeltaTime);
        }
    }
    public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;
        public Vector2 Move;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;

        public event Action Jumped;
        public Vector2 FrameInput { get; }
    }
}
