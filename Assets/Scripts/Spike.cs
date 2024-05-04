using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float knockbackStr;

    IEnumerator Quickstun()
    {
        GameManager.Instance.playerScript.stunned = true;
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.playerScript.stunned = false;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            if(GameManager.Instance.playerScript.damageCooldown > 0.25f)
            {         
                StartCoroutine(Quickstun());
                GameManager.Instance.TakeHealth(-damage);
                float z = Mathf.Round(gameObject.transform.localEulerAngles.z); 
                if (z == 0)
                {
                    //up
                    GameManager.Instance.playerScript.TakeKnockback(0, knockbackStr);
                    GameManager.Instance.playerScript.damageCooldown = 0;
                    GameManager.Instance.playerScript.knockbackCooldown = 0;
                }
                if (z == 180)
                {
                    //down
                    GameManager.Instance.playerScript.TakeKnockback(0, -knockbackStr/2);
                    GameManager.Instance.playerScript.damageCooldown = 0;
                    GameManager.Instance.playerScript.knockbackCooldown = 0;
                }
                if(z == 270)
                {
                    //right
                    GameManager.Instance.playerScript.TakeKnockback(knockbackStr, 200);
                    GameManager.Instance.playerScript.damageCooldown = 0;
                    GameManager.Instance.playerScript.knockbackCooldown = 0;
                }
                if (z == 90)
                {
                    //left
                    GameManager.Instance.playerScript.TakeKnockback(-knockbackStr, 200);
                    GameManager.Instance.playerScript.damageCooldown = 0;
                    GameManager.Instance.playerScript.knockbackCooldown = 0;
                }
            }
        }
        if (col.gameObject.tag == "Enemy")
        {
            Rigidbody2D rb = col.GetComponentInParent<Rigidbody2D>();
            if(col.GetComponentInParent<Enemy>() != null)
            {
                Enemy script1 = col.GetComponentInParent<Enemy>();
                script1.TakeDamage(damage);
            }
            if(col.GetComponentInParent<EnemyFlying>() != null)
            {
                EnemyFlying script2 = col.GetComponentInParent<EnemyFlying>();
                script2.TakeDamage();
            }
            
            


            float z = Mathf.Round(gameObject.transform.localEulerAngles.z);
            if (z == 0)
            {
                //up
                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2(0, knockbackStr));
            }
            if (z == 180)
            {
                //down
                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2(0, knockbackStr / 2));
            }
            if (z == 270)
            {
                //right
                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2(knockbackStr, 200));

            }
            if (z == 90)
            {
                //left
                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2(-knockbackStr, 200));
            }
        }
    }
}
