using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using static PlayerScript;


public class Enemy : MonoBehaviour
{
    public enum Frontfeature {Spikespot, Stunspot, Hurt, Weakspot, Shielded};
    public Frontfeature featureFrontfeature;
    public enum Backfeature { Spikespot, Stunspot, Hurt, Weakspot, Shielded };
    public Backfeature featureBackfeature;
    public enum Topfeature { Spikespot, Stunspot, Hurt, Weakspot, Shielded };
    public Topfeature featureTopfeature;
    public enum Bottomfeature { Spikespot, Stunspot, Hurt, Weakspot, Shielded };
    public Bottomfeature featureBottomfeature;


    public bool currentlyStunned;
    public bool playerIsInFrontOfMe;
    public bool playerIsRightOfMe;
    public bool playerIsAboveOfMe;
    public bool playerIsBelowOfMe;
    bool leftHitGround;
    bool RightHitGround;
    public bool grounded;


    public enum PathfindType { Idle, Linear, Jumper, Chase, Escape, Flying, Chaseflying };
    public PathfindType currentPathfindType;
    private bool useMovement;
    [Header("Stats")]
    [SerializeField] private int health;
    public int damage;
    [SerializeField] private float moveSpeed;
    public float knockbackX;
    public float knockbackY;
    private int direction;
    [Header("Agro")]
    [SerializeField] private bool doAgro;
    [SerializeField] private bool doEscape;
    [SerializeField] private float detectRange;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private bool canJumpChase;
    [SerializeField] private float jumpforce;
    [Header("Cooldowns")]
    private float changeDirectionDelay;
    private float destinationChoiceCooldown;
    private float stunCooldown;
    [HideInInspector] public float jumpCooldown;
    private float killCooldown;
    private float chaseCooldown;
    private float destinationX;
    private float distanceX;
    private float dealDamageCooldown;
    [Header("Components")]
    [SerializeField] Animator animator;
    public Rigidbody2D rb;
    [SerializeField] private Transform center;
    [SerializeField] private Collider2D col;
    [SerializeField] private LayerMask watermask;
    private bool isDead;
    [SerializeField] private Transform[] disableOnDeath;
    


    [System.Serializable]
    public class Data
    {
        public GameObject dropItem;
        public int dropAmount;
        public int dropPercentage;
    }
    public Data[] droptable;

    private void FixedUpdate()
    {
        if(!currentlyStunned)
        {
            Movement();
            FaceInput();
            HandlePathfindingType();
        }
        CheckPlayerLocation();
        checkGrounded();
        if (grounded && !currentlyStunned && !isDead)
        {
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = 3;
        }
    }

    private void Update()
    {
        if (true)
        {
            if (Input.GetKeyDown("f"))
            {
                //StartCoroutine(GetStunned());
            }
        }
        

        
        jumpCooldown = Mathf.Clamp(jumpCooldown += Time.deltaTime, 0, 4);
        chaseCooldown = Mathf.Clamp(chaseCooldown += Time.deltaTime, 0, 2);
        destinationChoiceCooldown = Mathf.Clamp(destinationChoiceCooldown += Time.deltaTime, 0, 1);
        killCooldown = Mathf.Clamp(killCooldown += Time.deltaTime, 0, 1);
        stunCooldown = Mathf.Clamp(stunCooldown += Time.deltaTime, 0, 4);
        dealDamageCooldown = Mathf.Clamp(dealDamageCooldown += Time.deltaTime, 0, 1);
        if (doAgro)
        {
            if (Vector2.Distance(transform.position, GameManager.Instance.playerScript.transform.position) < detectRange)
            {
                if(chaseCooldown > 1)
                {
                    currentPathfindType = PathfindType.Chase;
                }
                else 
                {
                    currentPathfindType = PathfindType.Escape;
                }
            }
            if (Vector2.Distance(transform.position, GameManager.Instance.playerScript.transform.position) > detectRange + 5)
            {
                currentPathfindType = PathfindType.Linear;
            }
        }
        else if (doEscape)
        {
            if (Vector2.Distance(transform.position, GameManager.Instance.playerScript.transform.position) < detectRange)
            {
                currentPathfindType = PathfindType.Escape;
            }
            if (Vector2.Distance(transform.position, GameManager.Instance.playerScript.transform.position) > detectRange + 5)
            {
                currentPathfindType = PathfindType.Idle;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!isDead)
        {
            if (col.gameObject.tag == "PlayerPunch")
            {
                //punch front spike
                if (featureFrontfeature == Frontfeature.Spikespot && dealDamageCooldown > 0.25f)
                {
                    if (playerIsInFrontOfMe && (!playerIsBelowOfMe && !playerIsAboveOfMe))
                    {
                        DealDamage();
                        GameManager.Instance.playerScript.stunned = true;
                        GameManager.Instance.playerScript.damageCooldown = 0;
                        if (playerIsRightOfMe)
                        {
                            GameManager.Instance.playerScript.TakeKnockback(knockbackX, knockbackY);
                        }
                        else
                        {
                            GameManager.Instance.playerScript.TakeKnockback(-knockbackX, knockbackY);
                        }

                    }
                }
                //punch back spike
                if (featureBackfeature == Backfeature.Spikespot && dealDamageCooldown > 0.25f)
                {
                    if (!playerIsInFrontOfMe && (!playerIsBelowOfMe && !playerIsAboveOfMe))
                    {
                        DealDamage();
                        GameManager.Instance.playerScript.stunned = true;
                        GameManager.Instance.playerScript.damageCooldown = 0;
                        if (playerIsRightOfMe)
                        {
                            GameManager.Instance.playerScript.TakeKnockback(knockbackX, knockbackY);
                        }
                        else
                        {
                            GameManager.Instance.playerScript.TakeKnockback(-knockbackX, knockbackY);
                        }

                    }
                }
                //punch front stunspot
                if (featureFrontfeature == Frontfeature.Stunspot)
                {
                    if (playerIsInFrontOfMe && (!playerIsBelowOfMe && !playerIsAboveOfMe))
                    {
                        StartCoroutine(GetStunned());
                        if(GameManager.Instance.playerScript.currentControlType != ControlType.Sprinting)
                        {
                            Debug.Log("123");
                            if (playerIsRightOfMe)
                            {
                                GameManager.Instance.playerScript.TakeKnockback(400, 200);
                            }
                            else
                            {
                                GameManager.Instance.playerScript.TakeKnockback(-400, 200);
                            }
                        }
                        
                    }
                }
                //punch back stunspot
                if (featureBackfeature == Backfeature.Stunspot)
                {
                    if (!playerIsInFrontOfMe && (!playerIsBelowOfMe && !playerIsAboveOfMe))
                    {
                        StartCoroutine(GetStunned());
                        if (GameManager.Instance.playerScript.currentControlType != ControlType.Sprinting)
                        {
                            if (playerIsRightOfMe)
                            {
                                GameManager.Instance.playerScript.TakeKnockback(400, 200);
                            }
                            else
                            {
                                GameManager.Instance.playerScript.TakeKnockback(-400, 200);
                            }
                        }
                    }
                }
                //punch front weakspot
                if (featureFrontfeature == Frontfeature.Weakspot)
                {
                    if (playerIsInFrontOfMe && (!playerIsBelowOfMe && !playerIsAboveOfMe))
                    {
                        TakeDamage(1);
                        if (health > 0)
                        {
                            StartCoroutine(GetStunned());
                        }
                        if (GameManager.Instance.playerScript.currentControlType != ControlType.Sprinting)
                        {
                            if (playerIsRightOfMe)
                            {
                                GameManager.Instance.playerScript.TakeKnockback(400, 200);
                            }
                            else
                            {
                                GameManager.Instance.playerScript.TakeKnockback(-400, 200);
                            }
                        }
                    }
                }
                //puch back weakspot
                if (featureFrontfeature == Frontfeature.Weakspot)
                {
                    if (!playerIsInFrontOfMe && (!playerIsBelowOfMe && !playerIsAboveOfMe))
                    {
                        TakeDamage(1);
                        if (health > 0)
                        {
                            StartCoroutine(GetStunned());
                        }

                        if (GameManager.Instance.playerScript.currentControlType != ControlType.Sprinting)
                        {
                            if (playerIsRightOfMe)
                            {
                                GameManager.Instance.playerScript.TakeKnockback(400, 200);
                            }
                            else
                            {
                                GameManager.Instance.playerScript.TakeKnockback(-400, 200);
                            }
                        }
                    }
                }
                //punch front roughHurt
                if (featureFrontfeature == Frontfeature.Hurt)
                {
                    if (playerIsInFrontOfMe && (!playerIsBelowOfMe && !playerIsAboveOfMe))
                    {
                        StartCoroutine(GetStunned());
                        if (GameManager.Instance.playerScript.currentControlType != ControlType.Sprinting)
                        {
                            if (playerIsRightOfMe)
                            {
                                GameManager.Instance.playerScript.TakeKnockback(400, 200);
                            }
                            else
                            {
                                GameManager.Instance.playerScript.TakeKnockback(-400, 200);
                            }
                        }
                    }
                }

                //punch Back roughHurt
                if (featureBackfeature == Backfeature.Hurt)
                {
                    if (!playerIsInFrontOfMe && (!playerIsBelowOfMe && !playerIsAboveOfMe))
                    {
                        StartCoroutine(GetStunned());
                        if (GameManager.Instance.playerScript.currentControlType != ControlType.Sprinting)
                        {
                            if (playerIsRightOfMe)
                            {
                                GameManager.Instance.playerScript.TakeKnockback(400, 200);
                            }
                            else
                            {
                                GameManager.Instance.playerScript.TakeKnockback(-400, 200);
                            }
                        }
                    }
                }

                //punch front bouncespot
                if (featureFrontfeature == Frontfeature.Shielded)
                {
                    if (playerIsInFrontOfMe && (!playerIsBelowOfMe && !playerIsAboveOfMe))
                    {
                        if (!GameManager.Instance.playerScript.flame)
                        {
                            if (playerIsRightOfMe)
                            {
                                GameManager.Instance.playerScript.TakeKnockback(400, 100);
                            }
                            else
                            {
                                GameManager.Instance.playerScript.TakeKnockback(-400, 100);
                            }
                        }
                        else
                        {
                            StartCoroutine (GetStunned());
                        }
                    }
                }
                //punch back bouncespot
                if (featureBackfeature == Backfeature.Shielded)
                {
                    if (!playerIsInFrontOfMe && (!playerIsBelowOfMe && !playerIsAboveOfMe))
                    {
                        if (!GameManager.Instance.playerScript.flame)
                        {
                            if (playerIsRightOfMe)
                            {
                                GameManager.Instance.playerScript.TakeKnockback(400, 100);
                            }
                            else
                            {
                                GameManager.Instance.playerScript.TakeKnockback(-400, 100);
                            }
                        }
                        else
                        {
                            StartCoroutine(GetStunned());
                        }
                    }
                }
            }
            if (col.gameObject.tag == "Player")
            {
                //contact front spike
                if (featureFrontfeature == Frontfeature.Spikespot || featureFrontfeature == Frontfeature.Hurt)
                {
                    if (playerIsInFrontOfMe && (!playerIsBelowOfMe && !playerIsAboveOfMe) && dealDamageCooldown > 0.25f)
                    {
                        DealDamage();
                        GameManager.Instance.playerScript.stunned = true;
                        GameManager.Instance.playerScript.damageCooldown = 0;
                        if (playerIsRightOfMe)
                        {
                            GameManager.Instance.playerScript.TakeKnockback(knockbackX, knockbackY);
                        }
                        else
                        {
                            GameManager.Instance.playerScript.TakeKnockback(-knockbackX, knockbackY);
                        }

                    }
                }
                //contact back spike
                if (featureBackfeature == Backfeature.Spikespot || featureBackfeature == Backfeature.Hurt)
                {
                    if (!playerIsInFrontOfMe && (!playerIsBelowOfMe && !playerIsAboveOfMe) && dealDamageCooldown > 0.25f)
                    {
                        DealDamage();
                        GameManager.Instance.playerScript.stunned = true;
                        GameManager.Instance.playerScript.damageCooldown = 0;
                        if (playerIsRightOfMe)
                        {
                            GameManager.Instance.playerScript.TakeKnockback(knockbackX, knockbackY);
                        }
                        else
                        {
                            GameManager.Instance.playerScript.TakeKnockback(-knockbackX, knockbackY);
                        }

                    }
                }
                //contact above spike
                if (featureTopfeature == Topfeature.Spikespot || featureTopfeature == Topfeature.Hurt)
                {
                    if (playerIsAboveOfMe && dealDamageCooldown > 0.25f)
                    {
                        DealDamage();

                        GameManager.Instance.playerScript.stunned = true;
                        GameManager.Instance.playerScript.damageCooldown = 0;
                        Debug.Log("1");
                        if (playerIsRightOfMe)
                        {
                            GameManager.Instance.playerScript.TakeKnockback(knockbackX, knockbackY);
                        }
                        else
                        {
                            GameManager.Instance.playerScript.TakeKnockback(-knockbackX, knockbackY);
                        }

                    }
                }
                //contact bottom spike
                if (featureBottomfeature == Bottomfeature.Spikespot || featureBottomfeature == Bottomfeature.Hurt)
                {
                    if (playerIsBelowOfMe && dealDamageCooldown > 0.25f)
                    {
                        DealDamage();
                        GameManager.Instance.playerScript.stunned = true;
                        GameManager.Instance.playerScript.damageCooldown = 0;
                        Debug.Log("2");
                        if (playerIsRightOfMe)
                        {
                            GameManager.Instance.playerScript.TakeKnockback(knockbackX, knockbackY);
                        }
                        else
                        {
                            GameManager.Instance.playerScript.TakeKnockback(-knockbackX, knockbackY);
                        }

                    }
                }
                //contact top stun
                if (featureTopfeature == Topfeature.Stunspot)
                {
                    if (playerIsAboveOfMe)
                    {
                        StartCoroutine(GetStunned());
                        if (GameManager.Instance.playerScript.jumpInput)
                        {
                            GameManager.Instance.playerScript.TakeKnockback(0, 600);
                        }
                        else
                        {
                            GameManager.Instance.playerScript.TakeKnockback(0, 400);
                        }

                    }
                }
                //Contact Bottom stun
                if (featureBottomfeature == Bottomfeature.Stunspot)
                {
                    if (playerIsBelowOfMe)
                    {
                        StartCoroutine(GetStunned());
                        if (GameManager.Instance.playerScript.jumpInput)
                        {
                            GameManager.Instance.playerScript.TakeKnockback(0, 600);
                        }
                        else
                        {
                            GameManager.Instance.playerScript.TakeKnockback(0, 400);
                        }

                    }
                }
                ////////////////
                //contact front weakspot
                if (featureFrontfeature == Frontfeature.Weakspot)
                {
                    if (playerIsInFrontOfMe && (!playerIsBelowOfMe && !playerIsAboveOfMe))
                    {
                        TakeDamage(1);
                        if (health > 0)
                        {
                            StartCoroutine(GetStunned());
                        }
                        if (!GameManager.Instance.playerScript.flame)
                        {
                            if (playerIsRightOfMe)
                            {
                                GameManager.Instance.playerScript.TakeKnockback(200, 200);
                            }
                            else
                            {
                                GameManager.Instance.playerScript.TakeKnockback(-200, 200);
                            }
                        }
                    }
                }
                //contact back weakspot
                if (featureBackfeature == Backfeature.Weakspot)
                {
                    if (!playerIsInFrontOfMe && (!playerIsBelowOfMe && !playerIsAboveOfMe))
                    {
                        TakeDamage(1);
                        if (health > 0)
                        {
                            StartCoroutine(GetStunned());
                        }
                        if (!GameManager.Instance.playerScript.flame)
                        {
                            if (playerIsRightOfMe)
                            {
                                GameManager.Instance.playerScript.TakeKnockback(200, 200);
                            }
                            else
                            {
                                GameManager.Instance.playerScript.TakeKnockback(-200, 200);
                            }
                        }
                        
                    }
                }
                //contact top weakspot
                if (featureTopfeature == Topfeature.Weakspot)
                {
                    if (playerIsAboveOfMe)
                    {
                        TakeDamage(1);
                        if (health > 0)
                        {
                            StartCoroutine(GetStunned());
                        }

                        GameManager.Instance.playerScript.TakeKnockback(0, 400);
                    }
                }
                //contact bottom weakspot
                if (featureBottomfeature == Bottomfeature.Weakspot && currentlyStunned)
                {
                    if (playerIsBelowOfMe)
                    {
                        TakeDamage(1);
                        if (health > 0)
                        {
                            StartCoroutine(GetStunned());
                        }

                        
                        if (GameManager.Instance.playerScript.jumpInput)
                        {
                            GameManager.Instance.playerScript.TakeKnockback(0, 600);
                        }
                        else
                        {
                            GameManager.Instance.playerScript.TakeKnockback(0, 400);
                        }
                    }
                }
                //Contact front shielded
                if (featureFrontfeature == Frontfeature.Shielded || featureFrontfeature == Frontfeature.Stunspot)
                {
                    if (playerIsInFrontOfMe && (!playerIsBelowOfMe && !playerIsAboveOfMe))
                    {
                        if(!GameManager.Instance.playerScript.flame)
                        {
                            if (playerIsRightOfMe)
                            {
                                GameManager.Instance.playerScript.TakeKnockback(400, 100);
                            }
                            else
                            {
                                GameManager.Instance.playerScript.TakeKnockback(-400, 100);
                            }
                        }
                        else if(GameManager.Instance.playerScript.flame)
                        {
                            StartCoroutine(GetStunned());
                        }
                        
                    }
                }
                //Contact back shielded
                if (featureBackfeature == Backfeature.Shielded || featureBackfeature == Backfeature.Stunspot)
                {
                    if (!playerIsInFrontOfMe && (!playerIsBelowOfMe && !playerIsAboveOfMe))
                    {
                        if (!GameManager.Instance.playerScript.flame)
                        {
                            if (playerIsRightOfMe)
                            {
                                GameManager.Instance.playerScript.TakeKnockback(400, 100);
                            }
                            else
                            {
                                GameManager.Instance.playerScript.TakeKnockback(-400, 100);
                            }
                        }
                        else if (GameManager.Instance.playerScript.flame)
                        {
                            StartCoroutine(GetStunned());
                        }
                    }
                }
                //contact top shielded
                if (featureTopfeature == Topfeature.Shielded)
                {
                    if (playerIsAboveOfMe)
                    {
                        if (GameManager.Instance.playerScript.flame)
                        {

                            if (featureBackfeature == Backfeature.Shielded)
                            {
                                TakeDamage(1);
                                if (health > 0)
                                {
                                    StartCoroutine(GetStunned());
                                }
                            }

                        }
                        else if (GameManager.Instance.playerScript.jumpInput)
                        {
                            GameManager.Instance.playerScript.TakeKnockback(0, 600);
                        }
                        else
                        {
                            GameManager.Instance.playerScript.TakeKnockback(0, 400);
                        }
                    }
                }
                //contact bottom shielded
                if (featureBottomfeature == Bottomfeature.Shielded)
                {
                    if (playerIsBelowOfMe)
                    {
                        if (GameManager.Instance.playerScript.jumpInput)
                        {
                            GameManager.Instance.playerScript.TakeKnockback(0, 600);
                        }
                        else
                        {
                            GameManager.Instance.playerScript.TakeKnockback(0, 400);
                        }
                    }
                }

                if (!currentlyStunned)
                {
                    if (playerIsBelowOfMe && dealDamageCooldown > 0.25f)
                    {
                        DealDamage();
                        GameManager.Instance.playerScript.stunned = true;
                        GameManager.Instance.playerScript.damageCooldown = 0;
                        rb.velocity = Vector2.zero;
                        rb.AddForce(new Vector2(0, 600));
                        if (playerIsRightOfMe)
                        {
                            GameManager.Instance.playerScript.TakeKnockback(knockbackX/2, knockbackY/2);
                        }
                        else
                        {
                            GameManager.Instance.playerScript.TakeKnockback(-knockbackX/2, knockbackY/2);
                        }
                    }
                }
            }
        }   
    }

    private void DealDamage()
    {
        GameManager.Instance.TakeHealth(-damage);
        GameManager.Instance.playerScript.stunned = true;
        GameManager.Instance.playerScript.damageCooldown = 0;
        GameManager.Instance.playerScript.dealdamageCooldown = 0;
        chaseCooldown = 0;
    }
    private void DealKnockback()
    {
        if(GameManager.Instance.playerScript.transform.position.x > center.position.x)
        {
            GameManager.Instance.playerScript.rbPlayer.AddForce(new Vector2(knockbackX, knockbackY));
        }
        else
        {
            GameManager.Instance.playerScript.rbPlayer.AddForce(new Vector2(-knockbackX, knockbackY));
        }
        GameManager.Instance.playerScript.rbPlayer.velocity = Vector2.zero;     
        GameManager.Instance.playerScript.stunned = true;
        GameManager.Instance.playerScript.damageCooldown = 0;
        GameManager.Instance.playerScript.dealdamageCooldown = 0;

    }
    private void LinearMovement()
    {
        changeDirectionDelay += Time.deltaTime;
        if(changeDirectionDelay > 0.5f)
        {
            RaycastHit2D[] hit = Physics2D.RaycastAll(center.transform.position, Vector2.right * direction, 0.75f);
            if (hit.Length > 0)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    string name = hit[i].collider.tag;
                    if (name == "Ground" || (name == "Enemy" && (hit[i].transform != transform))  || name == "Edgemarker")
                    {
                        changeDirectionDelay = 0;
                        rb.velocity = Vector2.zero;
                        direction = direction * -1;
                    }
                }
            }
        }
    }
    private void JumperMovement()
    {
        changeDirectionDelay += Time.deltaTime;
        if (changeDirectionDelay > 0.5f)
        {
            RaycastHit2D[] hit = Physics2D.RaycastAll(center.transform.position, Vector2.right * direction, 0.75f);
            if (hit.Length > 0)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    string name = hit[i].collider.tag;
                    if (name == "Ground" || (name == "Enemy" && (hit[i].transform != transform)) || name == "Edgemarker")
                    {
                        changeDirectionDelay = 0;
                        rb.velocity = Vector2.zero;
                        direction = direction * -1;
                    }
                }
            }
        }

        if(true)
        {

            if (grounded)
            {
                if (jumpCooldown > 0.2f)
                {
                    jumpCooldown = 0;
                    rb.velocity = Vector2.zero;
                    rb.AddForce(new Vector2(0, jumpforce));
                    //Debug.Log(this.gameObject.name + " jumped!");
                }
            }
        }
    }
    void ChaseMovement()
    {
        if (GameManager.Instance.playerScript.transform.position.x > transform.position.x)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }
        if (jumpCooldown > 1f && canJumpChase)
        {
            RaycastHit2D[] hit = Physics2D.RaycastAll(center.transform.position, Vector2.right * direction, 0.75f);
            if (hit.Length > 0)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    string name = hit[i].collider.tag;
                    if (name == "Ground" || (name == "Enemy" && (hit[i].transform != transform)) || name == "Edgemarker")
                    {
                        //jumps
                        //rb.velocity = Vector2.zero;
                        //rb.AddForce(new Vector2(0, jumpforce));
                        //jumpCooldown = 0;

                        if (grounded)
                        {
                            //jumps
                            if (jumpCooldown > 1f)
                            {
                                jumpCooldown = 0;
                                rb.velocity = Vector2.zero;
                                rb.AddForce(new Vector2(0, jumpforce));
                                //Debug.Log(this.gameObject.name + " jumped!");
                            }
                        }
                    }
                }
            }
            if (Vector2.Distance(transform.position, GameManager.Instance.playerScript.transform.position) < 2 )
            {
                if (transform.position.y - GameManager.Instance.playerScript.transform.position.y < -0.5f)
                {                 
                    if (grounded)
                    {
                        //jumps
                        if (jumpCooldown > 1f)
                        {
                            jumpCooldown = 0;
                            rb.velocity = Vector2.zero;
                            rb.AddForce(new Vector2(0, jumpforce));
                            //Debug.Log(this.gameObject.name + " jumped!");
                        }
                    }
                }
            }
        }
    }
    void EscapeMovement()
    {

        if (GameManager.Instance.playerScript.transform.position.x > transform.position.x)
        {
            direction = -1;
        }
        else
        {
            direction = 1;
        }
        if (jumpCooldown > 1f && canJumpChase)
        {
            RaycastHit2D[] hit = Physics2D.RaycastAll(center.transform.position, Vector2.right * direction, 0.75f);
            if (hit.Length > 0)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    string name = hit[i].collider.tag;
                    if (name == "Ground" || (name == "Enemy" && (hit[i].transform != transform)) || name == "Edgemarker")
                    {
                        RaycastHit2D[] groundhit = Physics2D.RaycastAll(center.transform.position, Vector2.down, 0.75f);
                        if (groundhit.Length > 0)
                        {
                            for (int j = 0; j < groundhit.Length; j++)
                            {
                                string groundname = groundhit[j].collider.tag;
                                if (name == "Ground" || name == "Enemy" || name == "Edgemarker")
                                {
                                    //jumps
                                    if (jumpCooldown > 1f)
                                    {
                                        jumpCooldown = 0;
                                        rb.velocity = Vector2.zero;
                                        rb.AddForce(new Vector2(0, jumpforce));
                                        //Debug.Log(this.gameObject.name + " jumped!");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (Vector2.Distance(transform.position, GameManager.Instance.playerScript.transform.position) < 2)
            {
                if (transform.position.y - GameManager.Instance.playerScript.transform.position.y < -0.5f)
                {
                    RaycastHit2D[] groundhit = Physics2D.RaycastAll(transform.position, Vector2.down, 0.5f);
                    if (groundhit.Length > 0)
                    {
                        for (int k = 0; k < groundhit.Length; k++)
                        {
                            string groundname = groundhit[k].collider.tag;
                            if (groundname == "Ground" || name == "Enemy" || name == "Water")
                            {
                                //jumps
                                if (jumpCooldown > 0.5f)
                                {
                                    jumpCooldown = 0;
                                    rb.velocity = Vector2.zero;
                                    rb.AddForce(new Vector2(0, jumpforce));
                                    //Debug.Log(this.gameObject.name + " jumped!");
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void WaterJump()
    {
        if(jumpCooldown >= 4)
        {
            if(Mathf.Abs(rb.velocity.x) < 1)
            {
                if (Physics2D.OverlapAreaAll(col.bounds.min, col.bounds.max, watermask).Length > 0)
                {
                    rb.AddForce(Vector2.up * 300);
                }
            }
            
        }
    }
    void HandlePathfindingType()
    {
        if (currentPathfindType == PathfindType.Idle)
        {
            direction = 0;
            ChangeAnimation("IDLE");
        }
        if (currentPathfindType == PathfindType.Linear)
        {
            LinearMovement();
            if(direction == 0)
            {
                direction = -1;
            }
            ChangeAnimation("WALK");
        }
        if (currentPathfindType == PathfindType.Jumper)
        {
            JumperMovement();
            if (direction == 0)
            {
                direction = -1;
            }
            ChangeAnimation("WALK");
        }
        if (currentPathfindType == PathfindType.Chase)
        {
            ChaseMovement();
            ChangeAnimation("WALK");
        }
        if (currentPathfindType == PathfindType.Escape)
        {
            EscapeMovement();
            ChangeAnimation("WALK");
        }
    }

    private void Movement()
    {
        WaterJump();
        if (currentPathfindType == PathfindType.Chase) 
        {
            float targetSpeed = chaseSpeed * direction;
            float speedDif = targetSpeed - rb.velocity.x;
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? 5 : 5;
            float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, 1) * Mathf.Sign(speedDif);
            rb.AddForce(movement * Vector2.right);
        }
        else
        {
            float targetSpeed = moveSpeed * direction;
            float speedDif = targetSpeed - rb.velocity.x;
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? 10 : 10;
            float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, 1) * Mathf.Sign(speedDif);
            rb.AddForce(movement * Vector2.right);
        }      
    }

    public void TakeDamage(int amount)
    {
        GameManager.Instance.playerScript.stunned = false;
        health -= amount;
        if(health <= 0)
        {
            DeathSequence();
        }
    }

    IEnumerator GetStunned()
    {
        dealDamageCooldown = 0;
        Debug.Log(name + " got stunned!");
        stunCooldown = 0;
        rb.sharedMaterial = GameManager.Instance.frictionMaterial;
        rb.velocity = Vector2.zero;
        playerIsInFrontOfMe = !playerIsInFrontOfMe;

        CheckPlayerLocation();

        if(!currentlyStunned)
        {
            if (playerIsInFrontOfMe)
            {
                ChangeAnimation("StunL");
            }
            else
            {
                ChangeAnimation("StunR");
            }
        }
        currentlyStunned = true;

        if (playerIsRightOfMe)
        {
            rb.AddForce(new Vector2(-200, 300));
        }
        else
        {
            rb.AddForce(new Vector2(200, 300));
        }
        yield return new WaitForSeconds(0.5f);
        ChangeAnimation("STRUGGLE");
        yield return new WaitForSeconds(6);
        if(stunCooldown >= 3)
        {
            StartCoroutine(GetUnStunned());
        }     
    }

    IEnumerator GetUnStunned()
    {
        if (playerIsInFrontOfMe)
        {
            ChangeAnimation("UnStunL");
        }
        else
        {
            ChangeAnimation("UnStunR");
        }
        rb.AddForce(new Vector2(0, 300));
        yield return new WaitForSeconds(0.5f);
        rb.sharedMaterial = GameManager.Instance.noFrictionMaterial;
        currentlyStunned = false;
    }

    void DeathSequence()
    {
        rb.gravityScale = 3;
        if(!isDead)
        {
            isDead = true;
            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                disableOnDeath[i].gameObject.SetActive(false);
            }
            rb.velocity = Vector2.zero;
            rb.freezeRotation = false;
            rb.AddForce(new Vector2(Random.Range(-100, 100), 300));
            rb.AddTorque(Random.Range(-400, 400));
            Destroy(gameObject, 2);
            DropLoot();
            GetComponent<Enemy>().enabled = false;
        }    
    }

    void DropLoot()
    {
        if (droptable.Length > 0)
        {
            for (int i = 0; i < droptable.Length; i++)
            {
                if (droptable[i].dropPercentage >= Random.Range(0, 100))
                {
                    for (int j = 0; j <= Random.Range(1, droptable[i].dropAmount) - 1; j++)
                    {
                        GameObject drop = Instantiate(droptable[i].dropItem);
                        Rigidbody2D rb = drop.GetComponent<Rigidbody2D>();
                        drop.transform.position = center.transform.position;
                        rb.velocity = Vector2.zero;
                        rb.AddForce(new Vector2(Random.Range(-200, 200), Random.Range(100, 500)));
                    }
                }
            }
        }
    }

    void ChangeAnimation(string animName)
    {
        if(animator != null)
        {
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != animName)
            {
                animator.Play(animName);
            }
        }      
    }

    void FaceInput()
    {
        if (direction > 0.1f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        if (direction < -0.1f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void CheckPlayerLocation()
    {
        if (transform.position.x > GameManager.Instance.playerScript.transform.position.x)
        {
            if (transform.localScale.x == 1)
            {
                playerIsInFrontOfMe = currentlyStunned;
            }
            else
            {
                playerIsInFrontOfMe = !currentlyStunned;
            }
            playerIsRightOfMe = false;
        }
        if (transform.position.x < GameManager.Instance.playerScript.transform.position.x)
        {
            if (transform.localScale.x == -1)
            {
                playerIsInFrontOfMe = currentlyStunned;
            }
            else
            {
                playerIsInFrontOfMe = !currentlyStunned;
            }
            playerIsRightOfMe = true;
        }
        //////////////////////
        ///

        if (currentlyStunned)
        {
            if (GameManager.Instance.playerScript.transform.position.y < center.position.y - 1.25f)
            {
                playerIsAboveOfMe = true;
            }
            else
            {
                playerIsAboveOfMe = false;
            }
            if (GameManager.Instance.playerScript.transform.position.y > center.position.y + 0.25f)
            {
                playerIsBelowOfMe = true;
            }
            else
            {
                playerIsBelowOfMe = false;
            }
        }
        else
        {
            if (GameManager.Instance.playerScript.transform.position.y > center.position.y + 0.25f)
            {
                playerIsAboveOfMe = true;
            }
            else
            {
                playerIsAboveOfMe = false;
            }
            if (GameManager.Instance.playerScript.transform.position.y < center.position.y - 1.25f)
            {
                playerIsBelowOfMe = true;
            }
            else
            {
                playerIsBelowOfMe = false;
            }
        }

    }

    void checkGrounded()
    {


        Vector2 vectL = new Vector2(center.transform.position.x - 0.25f, center.transform.position.y);
        Vector2 vectR = new Vector2(center.transform.position.x + 0.25f, center.transform.position.y);

        if (true)
        {
            RaycastHit2D[] hit = Physics2D.RaycastAll(vectL, Vector2.down, 0.75f);
            if (hit.Length > 0)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    string name = hit[i].collider.tag;
                    if (name == "Ground")
                    {
                        leftHitGround = true;
                    }
                    else
                    {
                        leftHitGround = false;
                    }
                }
            }
        }


        if (true)
        {
            RaycastHit2D[] hit = Physics2D.RaycastAll(vectR, Vector2.down, 0.75f);
            if (hit.Length > 0)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    string name = hit[i].collider.tag;
                    if (name == "Ground")
                    {
                        RightHitGround = true;
                    }
                    else
                    {
                        RightHitGround = false;
                    }
                }
            }
        }

        if (leftHitGround || RightHitGround)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }

    }
}
