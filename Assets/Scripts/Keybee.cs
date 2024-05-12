using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Keybee : MonoBehaviour
{
    public State currentState;
    [SerializeField] private float followspeed;
    public Animator animator;
    private GameObject keybeeFollowPoint;
    public float pickupCooldown;
    public SpriteRenderer sprite;

    private void Start()
    {
        GameManager.Instance.keybee = GetComponent<Keybee>();
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            if (!GameManager.Instance.hasKeybee && pickupCooldown >= 1.2f)
            {
                if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "KeybeeTwirl")
                {
                    animator.Play("KeybeeTwirl");                 
                }
                GameManager.Instance.hasKeybee = true;
                keybeeFollowPoint = GameObject.FindGameObjectWithTag("KeybeeFollowPoint");
            }
        }
    }

    private void Update()
    {
        if (GameManager.Instance.hasKeybee)
        {
            Follow();
            pickupCooldown = 0;
        }
        else
        {
            pickupCooldown = Mathf.Clamp(pickupCooldown += Time.deltaTime, 0, 2);
        }
        if(PlayerPrefs.GetInt("level" + SceneManager.GetActiveScene().buildIndex + "KeybeeFound") == 1)
        {
            sprite.color = new Color(1, 1, 1, 0.4f);
        }

    }

    void Follow()
    {
        if(keybeeFollowPoint != null)
        {
            float distance = Vector2.Distance(keybeeFollowPoint.transform.position, transform.position);
            float step = followspeed * distance * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, keybeeFollowPoint.transform.position, step);
            if(distance > 20)
            {
                transform.position = GameManager.Instance.playerScript.transform.position;
            }
        }        
    }
}
