using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerScript;

public class HiddenWall : MonoBehaviour
{

    [SerializeField] private LayerMask entityMask;
    [SerializeField] private Collider2D col;
    [SerializeField] private Animator animator;
    [SerializeField] private bool faded;
    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            faded = true;
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            faded = false;
        }
    }

    private void Update()
    {
        animator.SetBool("Fade", faded);
    }

}
