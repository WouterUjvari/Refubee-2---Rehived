using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncepad : MonoBehaviour
{
    public float forceX;
    public float forceY;
    private float cooldown;
    [SerializeField] private Animator animator;

    private void Update()
    {
        cooldown = Mathf.Clamp(cooldown += Time.deltaTime, 0, 1);
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            if(GameManager.Instance.playerScript.bounceCooldown > 0.2f)
            {
                if (cooldown > 0.5f)
                {
                    cooldown = 0;
                    ApplyBouncePlayer(forceX, forceY);
                }
                
            }
        }
        else if (col.gameObject.tag == "Enemy")
        {

            Enemy script1 = col.gameObject.GetComponentInParent<Enemy>();
            EnemyFlying script2 = col.gameObject.GetComponentInParent<EnemyFlying>();

            if (script1 != null)
            {
                if(script1.rb.gravityScale != 0)
                {
                    if(script1.jumpCooldown > 0.2f)
                    {
                        script1.jumpCooldown = 0;
                        ApplyBounceOther(script1.rb ,forceX, forceY);
                    }
                }
            }

            if (script2 != null)
            {
                if (script2.rb.gravityScale != 0)
                {
                    if (script2.jumpCooldown > 0.2f)
                    {
                        script2.jumpCooldown = 0;
                        ApplyBounceOther(script2.rb, forceX, forceY);
                    }
                }
            }
        }
        else if (col.gameObject.tag == "Pickup")
        {
            Pickup script = col.gameObject.GetComponentInParent<Pickup>();
            if(script != null)
            {
                if(script.transform.GetComponent<Rigidbody2D>() != null)
                {
                    if(script.bounceCooldown > 0.2f)
                    {
                        script.bounceCooldown = 0;
                        Rigidbody2D rb = script.transform.GetComponent<Rigidbody2D>();
                        ApplyBounceOther(rb, forceX, forceY);
                    }
                    
                }
            }
        }
    }

    void ApplyBouncePlayer(float x, float y)
    {
        GameManager.Instance.playerScript.bounceCooldown = 0;
        GameManager.Instance.playerScript.stunned = false;
        GameManager.Instance.playerScript.rbPlayer.velocity = new Vector2(GameManager.Instance.playerScript.rbPlayer.velocity.x, 0);

        GameManager.Instance.playerScript.rbPlayer.AddForce(new Vector2(x, y));

        if(animator != null)
        {
            animator.Play("Bounce");
        }
    }

    void ApplyBounceOther(Rigidbody2D rb ,float x, float y)
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(new Vector2(x, y));
    }
}
