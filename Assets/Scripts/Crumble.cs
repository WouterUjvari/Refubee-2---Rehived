using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crumble : MonoBehaviour
{
    [SerializeField] private Vector2 originPos;
    [SerializeField] private float crumbleTimer;
    [SerializeField] private float recoverTimer;
    [SerializeField] private bool currentlyCrumble;
    [SerializeField] private bool shake;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] private Collider2D col;
    [SerializeField] private Animator animator;


    private void Start()
    {
        originPos = transform.position;
    }

    private void FixedUpdate()
    {
        if (shake)
        {
            animator.Play("CrumbleShake");
        }
        else
        {
            animator.Play("Static");
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!currentlyCrumble)
        {
            if (col.gameObject.tag == "Player")
            {
                if(transform.position.y < GameManager.Instance.playerScript.transform.position.y)
                {
                    StartCoroutine(CrumbleDrop());
                }                
            }
        }     
    }
    private IEnumerator CrumbleDrop()
    {
        currentlyCrumble = true;
        shake = true;
        yield return new WaitForSeconds(crumbleTimer);
        Debug.Log("p1");
        rb.velocity = Vector3.zero;
        rb.isKinematic = false;
        //col.enabled = false;
        shake = false;
        yield return new WaitForSeconds(recoverTimer);
        Debug.Log("p2");
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        transform.position = originPos;
        currentlyCrumble = false;
        //col.enabled = true;
    }
}
