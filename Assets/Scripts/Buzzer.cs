using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerScript;
using UnityEngine.Windows;

public class Buzzer : MonoBehaviour
{
    public int seconds;
    [SerializeField] private Animator animator;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!GameManager.Instance.buzzerActivated)
        {
            if (col.gameObject.tag == "Player")
            {
                Debug.Log("Buzzer activated!!!");
                animator.Play("PRESS");
                GameManager.Instance.buzzerActivated = true;
                Hud.Instance.timerControllerScript.countdownTimer = seconds;
                GameManager.Instance.StartControlRestriction(3);

            }
        }        
    }


}
