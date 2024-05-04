using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlying : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int health;
    [SerializeField] private int damage;
    [SerializeField] private float speed;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float detectionRange;
    [SerializeField] private float knockbackX;
    [SerializeField] private float knockbackY;
    public enum PathfindType {Flying, Chaseflying,GroundFlee};
    public PathfindType currentPathfindType;
    public enum WeakspotLocation { Top, Bottom, Right, Left };
    public WeakspotLocation currentWeakspotLocation;

    [Header("flying")]
    [SerializeField] private Transform[] route;   
    [SerializeField] private bool fly = true;
    [SerializeField] private Transform currentTarget;
    private int index;
    private float accel = 1;
    private float killCooldown;
    private float chaseCooldown;
    public float jumpCooldown;
    private float changeDirectionCooldown;
    [Header("Grounded")]
    [SerializeField] private bool loseWingsOnDamage;
    [SerializeField] private bool lostWings;
    [SerializeField] private float direction = -1;
    [SerializeField] private float groundSpeed;
    [SerializeField] private float jumpForce;
    [Header("Components")]
    public Rigidbody2D rb;
    [SerializeField] private Transform center;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform[] disableOnDeath;
    [System.Serializable]
    public class Data
    {
        public GameObject dropItem;
        public int dropAmount;
        public int dropPercentage;
    }
    public Data[] droptable;


    private void Start()
    {
        index = 0;
        currentTarget = route[0];
    }

    private void Update()
    {
        killCooldown = Mathf.Clamp(killCooldown += Time.deltaTime, 0, 1);
        chaseCooldown = Mathf.Clamp(chaseCooldown += Time.deltaTime, 0, 1);
        jumpCooldown = Mathf.Clamp(jumpCooldown += Time.deltaTime, 0, 1);
        changeDirectionCooldown = Mathf.Clamp(changeDirectionCooldown += Time.deltaTime, 0, 1);
    }

    private void FixedUpdate()
    {
        FaceInput();
        if (!lostWings)
        {
            if (detectionRange != 0)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, GameManager.Instance.playerScript.transform.position);
                if (chaseCooldown >= 1f)
                {
                    if (distanceToPlayer < detectionRange)
                    {
                        currentPathfindType = PathfindType.Chaseflying;
                    }
                    else if (distanceToPlayer > detectionRange + 2)
                    {
                        currentPathfindType = PathfindType.Flying;
                    }
                }
                else
                {
                    currentPathfindType = PathfindType.Flying;
                }
            }
        }
        
        
        if(currentPathfindType == PathfindType.Flying)
        {
            Roadmap();
            FollowPath(currentTarget);
        }
        if(currentPathfindType == PathfindType.Chaseflying)
        {
            ChasePlayer(GameManager.Instance.playerScript.transform);
        }
        if(currentPathfindType == PathfindType.GroundFlee)
        {
            GroundFlee();
        }
    }

    void Roadmap()
    {
        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);
        
        if (distanceToTarget <= 0.2f)
        {
            if (index == route.Length - 1)
            {
                index = 0;
            }
            else
            {
                index++;
            }
            accel = 0;
            currentTarget = route[index];
        }       
    }
    void FollowPath(Transform target)
    {
        if (fly)
        {
            accel = Mathf.Clamp(accel += Time.deltaTime, 0, 1);
            float distance = Mathf.Clamp(Vector2.Distance(target.position, transform.position), 0, 1);
            float step = speed * distance * accel * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        }
    }

    void ChasePlayer(Transform target)
    {
        if (fly)
        {
            accel = Mathf.Clamp(accel += Time.deltaTime, 0, 1);
            float distance = Mathf.Clamp(Vector2.Distance(target.position, transform.position), 0, 1);
            float step = chaseSpeed * distance * accel * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        }
    }

    void GroundFlee()
    {
        ChangeAnimation("WALK");
        if (GameManager.Instance.playerScript.transform.position.x > transform.position.x)
        {
            direction = -1;
        }
        else
        {
            direction = 1;
        }

        
        
        if(jumpCooldown > 0.5f && lostWings)
        {
            RaycastHit2D[] sideHit = Physics2D.RaycastAll(transform.position, Vector2.right * direction, 0.75f);
            if (sideHit.Length > 0 && jumpCooldown > 0.5f)
            {
                for (int i = 0; i < sideHit.Length; i++)
                {
                    string name = sideHit[i].collider.tag;
                    if (name == "Ground" || (name == "Enemy" && (sideHit[i].transform != transform)) || name == "Edgemarker")
                    {
                        //Obstacle Detected
                        Debug.Log("test1");
                        RaycastHit2D[] groundHit = Physics2D.RaycastAll(transform.position, Vector2.down, 0.75f);
                        if (groundHit.Length > 0 && jumpCooldown > 0.5f)
                        {
                            for (int j = 0; j < groundHit.Length; j++)
                            {
                                string groundname = groundHit[j].collider.tag;
                                if (groundname == "Ground" || groundname == "Edgemarker")
                                {
                                    jumpCooldown = 0;
                                    rb.velocity = Vector2.zero;
                                    rb.AddForce(new Vector2(0, jumpForce));
                                }
                            }
                        }
                    }
                }
            }
        }

        Debug.Log("Walking");
        float targetSpeed = groundSpeed * direction;
        float speedDif = targetSpeed - rb.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? 10 : 10;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, 1) * Mathf.Sign(speedDif);
        rb.AddForce(movement * Vector2.right);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player" && killCooldown > 0.5f)
        {
            //player takes damage
            if (currentWeakspotLocation == WeakspotLocation.Top)
            {
                if (GameManager.Instance.playerScript.transform.position.y < center.position.y /*higher*/)
                {
                    if (GameManager.Instance.playerScript.damageCooldown > 0.2f)
                    {
                        if (damage != 0)
                        {
                            DealDamage();
                        }
                    }
                    if (GameManager.Instance.playerScript.knockbackCooldown > 0.2f)
                    {
                        if (knockbackX != 0 || knockbackY != 0)
                        {

                            DealKnockback();
                        }
                    }
                }
                else
                {
                    if (GameManager.Instance.playerScript.dealdamageCooldown > 0.2f)
                    {
                        TakeDamage();
                        //GameManager.Instance.playerScript.BounceOffEnemy();
                    }
                }
            }
            if (currentWeakspotLocation == WeakspotLocation.Bottom)
            {
                if (GameManager.Instance.playerScript.transform.position.y > center.position.y /*lower*/)
                {
                    if (GameManager.Instance.playerScript.damageCooldown > 0.2f)
                    {
                        if (damage != 0)
                        {
                            DealDamage();
                        }
                    }
                    if (GameManager.Instance.playerScript.knockbackCooldown > 0.2f)
                    {
                        if (knockbackX != 0 || knockbackY != 0)
                        {

                            DealKnockback();
                        }
                    }
                }
                else
                {
                    if (GameManager.Instance.playerScript.dealdamageCooldown > 0.2f)
                    {
                        TakeDamage();
                        //GameManager.Instance.playerScript.BounceOffEnemy();
                    }
                }
            }

            if (currentWeakspotLocation == WeakspotLocation.Right)
            {
                if (GameManager.Instance.playerScript.transform.position.x < center.position.x /*right*/)
                {
                    if (GameManager.Instance.playerScript.damageCooldown > 0.2f)
                    {
                        if (damage != 0)
                        {
                            DealDamage();
                        }
                    }
                    if (GameManager.Instance.playerScript.knockbackCooldown > 0.2f)
                    {
                        if (knockbackX != 0 || knockbackY != 0)
                        {

                            DealKnockback();
                        }
                    }
                }
                else
                {
                    if (GameManager.Instance.playerScript.dealdamageCooldown > 0.2f)
                    {
                        TakeDamage();
                        //GameManager.Instance.playerScript.BounceOffEnemy();
                    }
                }
            }

            if (currentWeakspotLocation == WeakspotLocation.Left)
            {
                if (GameManager.Instance.playerScript.transform.position.x > center.position.x /*left*/)
                {
                    if (GameManager.Instance.playerScript.damageCooldown > 0.2f)
                    {
                        if (damage != 0)
                        {
                            DealDamage();
                        }
                    }
                    if (GameManager.Instance.playerScript.knockbackCooldown > 0.2f)
                    {
                        if (knockbackX != 0 || knockbackY != 0)
                        {

                            DealKnockback();
                        }
                    }
                }
                else
                {
                    if (GameManager.Instance.playerScript.dealdamageCooldown > 0.2f)
                    {
                        TakeDamage();
                        //GameManager.Instance.playerScript.BounceOffEnemy();
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
        if (GameManager.Instance.playerScript.transform.position.x > center.position.x)
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

    public void TakeDamage()
    {
        health--;

        if (health <= 0)
        {
            DeathSequence();
        }
        else if (loseWingsOnDamage)
        {
            lostWings = true;
            currentPathfindType = PathfindType.GroundFlee;
            rb.isKinematic = false;
        }
    }

    void ChangeAnimation(string animName)
    {
        if (animator != null)
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
        if (currentPathfindType == PathfindType.Chaseflying)
        {
            if (GameManager.Instance.playerScript.transform.position.x > transform.position.x)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }
        }
        if (currentPathfindType == PathfindType.Flying)
        {
            if (currentTarget.transform.position.x > transform.position.x)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }
        } 
    }


    void DeathSequence()
    {
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].gameObject.SetActive(false);
        }
        ChangeAnimation("DEAD");
        rb.isKinematic = false;
        rb.velocity = Vector2.zero;
        rb.freezeRotation = false;
        rb.AddForce(new Vector2(Random.Range(-300, 300), 300));
        rb.AddTorque(Random.Range(-400, 400));
        Destroy(gameObject, 2);
        DropLoot();
        GetComponent<EnemyFlying>().enabled = false;
    }

    void DropLoot()
    {
        if (droptable.Length > 0)
        {
            for (int i = 0; i < droptable.Length; i++)
            {
                if (droptable[i].dropPercentage >= Random.Range(0, 100))
                {
                    for (int j = 0; j <= droptable[i].dropAmount - 1; j++)
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

    private void OnDrawGizmos()
    {
        if (route.Length != 0)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < route.Length; i++)
            {
                if (i < route.Length - 1)
                {
                    Gizmos.DrawLine(route[i].position, route[i + 1].position);
                }
            }
            Gizmos.DrawLine(route[route.Length - 1].position, route[0].position);
        }
    }
}
