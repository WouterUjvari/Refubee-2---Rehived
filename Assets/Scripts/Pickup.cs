using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Pickup : MonoBehaviour
{
    public enum CointType {None, Copper, Silver, Gold};
    public CointType currentCointType;
    public int health;
    public int uses;
    public float cooldown;
    [SerializeField] private Transform[] disableOnDeath;
    [SerializeField] private Animator animator;
    [SerializeField] private string usageAnimString;
    [SerializeField] private string deathAnimString;
    [SerializeField] private float deathSpeed;
    public float bounceCooldown;

    private void Update()
    {
        cooldown = Mathf.Clamp(cooldown += Time.deltaTime, 0, 1);
        bounceCooldown = Mathf.Clamp(bounceCooldown += Time.deltaTime, 0, 1);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            if(cooldown > 0.2f && uses > 0)
            {
                //health
                if (health > 0)
                {
                    if(GameManager.Instance.playerCurrentHealthPoints < GameManager.Instance.playerMaxHealthPoints)
                    {                                           
                        if (health != 0)
                        {
                            GameManager.Instance.TakeHealth(health);
                            uses--;
                        }                     
                    }
                }
                //coins
                if (currentCointType != CointType.None)
                {
                    if (currentCointType == CointType.Copper)
                    {
                        GameManager.Instance.coinsCopper++;
                    }
                    if (currentCointType == CointType.Silver)
                    {
                        GameManager.Instance.coinsSilver++;
                    }
                    if (currentCointType == CointType.Gold)
                    {
                        GameManager.Instance.coinsGold++;
                    }
                    uses--;
                }
                if (uses == 0)
                {
                    DeathSequence();
                }
            }
        }
    }
    public void Usage()
    {
        if(cooldown > 0.2f) 
        {        
            cooldown = 0;
            uses--;
            if(uses <= 0)
            {
                DeathSequence();
            }
            else
            {
                if (animator != null)
                {
                    if (usageAnimString != null)
                    {
                        animator.Play(usageAnimString);
                    }
                }
            }
        }
    }
    public void DeathSequence()
    {
        if(GetComponent<Rigidbody2D>() != null)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
        }
        
        if(animator != null)
        {
            if (deathAnimString != null)
            {
                animator.Play(deathAnimString);

            }
        }
        Destroy(gameObject, deathSpeed);
    }
}
