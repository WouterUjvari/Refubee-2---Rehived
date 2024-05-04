using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeEffect : MonoBehaviour
{
    [SerializeField] private string effectName;
    [SerializeField] private bool followPlayer;
    [SerializeField] private Animator animator;

    // Update is called once per frame

    void Start()
    {
        animator.Play(effectName);
    }
    void Update()
    {
        if (followPlayer)
        {
            transform.position = GameManager.Instance.playerScript.bodyCollider.transform.position;
        }    
    }


}
