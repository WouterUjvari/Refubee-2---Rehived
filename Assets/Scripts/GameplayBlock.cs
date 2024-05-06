using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayBlock : MonoBehaviour
{
    public enum BlockType { Crate, Block, Hardblock, Moveblock};
    public BlockType typeBlockType;
    public bool dead;
    public bool held;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D worldCollider;
    [SerializeField] private Rigidbody2D rb;

    private bool knock;
    [System.Serializable]
    public class Data
    {
        public GameObject dropItem;
        public int dropAmount;
        public int dropPercentage;
    }
    public Data[] droptable;

    private void Update()
    {
        if (typeBlockType == BlockType.Crate || typeBlockType == BlockType.Block)
        {
            if (GameManager.Instance.playerScript.flame)
            {
                Physics2D.IgnoreCollision(GameManager.Instance.playerScript.bodyCollider, worldCollider);
            }
            else
            {
                Physics2D.IgnoreCollision(GameManager.Instance.playerScript.bodyCollider, worldCollider, false);
            }
        }
                  

        if (held)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if(GameManager.Instance.playerScript.heldItem !=  null)
                {
                    held = false;
                    transform.SetParent(null);
                    rb.isKinematic = false;
                    rb.velocity = Vector2.zero;

                    if (GameManager.Instance.playerScript.transform.localScale.x == 1)
                    {
                        rb.AddForce(new Vector2(300, 300));
                    }
                    else
                    {
                        rb.AddForce(new Vector2(-300, 300));
                    }
                    GameManager.Instance.playerScript.heldItem = null;
                    GameManager.Instance.playerScript.pickupcooldown = 0;
                    GameManager.Instance.playerScript.rbPlayer.velocity = Vector2.zero;
                }
            }
            if(GameManager.Instance.playerScript.currentControlType != PlayerScript.ControlType.Platforming)
            {
                held = false;
                transform.SetParent(null);
                rb.isKinematic = false;
                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2(100, 100));
                GameManager.Instance.playerScript.heldItem = null;
                GameManager.Instance.playerScript.pickupcooldown = 0;
                GameManager.Instance.playerScript.rbPlayer.velocity = Vector2.zero;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!dead)
        {
            if(typeBlockType == BlockType.Crate)
            {
                if (col.gameObject.tag == "PlayerPunch")
                {
                    DeathSequence();
                    StartCoroutine(PlayerBumpback());

                }
                if (GameManager.Instance.playerScript.flame)
                {
                    if (col.gameObject.tag == "Player")
                    {
                        DeathSequence();
                    }
                }
            }
            if (typeBlockType == BlockType.Block)
            {
                if (col.gameObject.tag == "PlayerPunch")
                {
                    DeathSequence();
                    StartCoroutine(PlayerBumpback());

                }
                if (GameManager.Instance.playerScript.flame)
                {
                    if (col.gameObject.tag == "Player")
                    {
                        DeathSequence();
                    }
                }
            }
            if (typeBlockType == BlockType.Hardblock)
            {
                if (col.gameObject.tag == "PlayerPunch")
                {
                    animator.Play("BlockShake");
                    StartCoroutine(PlayerBumpback());

                }
                if (GameManager.Instance.playerScript.flame)
                {
                    if (col.gameObject.tag == "Player")
                    {
                        DeathSequence();
                        StartCoroutine(PlayerBumpback());   
                    }
                }
            }
            if (typeBlockType == BlockType.Moveblock)
            {
                if(GameManager.Instance.playerScript.heldItem == null)
                {
                    if (col.gameObject.tag == "PlayerPunch" && !held && GameManager.Instance.playerScript.pickupcooldown > 0.5f)
                    {
                        GameManager.Instance.playerScript.heldItem = this.gameObject.transform;
                        held = true;
                        transform.SetParent(GameManager.Instance.playerScript.antennaCollider.transform);
                        transform.position = new Vector2(GameManager.Instance.playerScript.antennaCollider.transform.position.x, GameManager.Instance.playerScript.transform.position.y + 1.75f);
                        rb.isKinematic = true;
                        GameManager.Instance.playerScript.pickupcooldown = 0;
                        GameManager.Instance.playerScript.rbPlayer.velocity = Vector2.zero;


                    }
                }
            }
        }
    }

    void DeathSequence()
    {
        dead = true;
        animator.Play("BlockExplode");
        Destroy(gameObject, 0.25f);
        DropLoot();
    }

    IEnumerator PlayerBumpback()
    {
        if (!knock)
        {
            knock = true;
            if (transform.position.x < GameManager.Instance.playerScript.transform.position.x)
            {
                //right
                GameManager.Instance.playerScript.TakeKnockback(400, 200);
            }
            else
            {
                //left
                GameManager.Instance.playerScript.TakeKnockback(-400, 200);
            }
            yield return new WaitForSeconds(0.5f);
            knock = false;
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
                        drop.transform.position = transform.position;
                        rb.velocity = Vector2.zero;
                        rb.AddForce(new Vector2(Random.Range(-200, 200), Random.Range(100, 500)));
                    }
                }
            }
        }
    }
}
