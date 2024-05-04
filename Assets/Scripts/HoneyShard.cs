using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HoneyShard : MonoBehaviour
{
    [SerializeField] private int shardNumber;
    [SerializeField] bool taken;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer sprite;
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            if (!taken)
            {
                GameManager.Instance.hasShard[shardNumber - 1] = true;
                taken = true;
                DestroySequence();
                
            }
            
        }
    }

    private void Update()
    {
        if(shardNumber == 1)
        {
            if (PlayerPrefs.GetInt("level" + SceneManager.GetActiveScene().buildIndex + "ShardOneFound") == 1)
            {
                sprite.color = new Color(1, 1, 1, 0.2f);
            }
        }
        if (shardNumber == 2)
        {
            if (PlayerPrefs.GetInt("level" + SceneManager.GetActiveScene().buildIndex + "ShardTwoFound") == 1)
            {
                sprite.color = new Color(1, 1, 1, 0.2f);
            }
        }
        if (shardNumber == 3)
        {
            if (PlayerPrefs.GetInt("level" + SceneManager.GetActiveScene().buildIndex + "ShardThreeFound") == 1)
            {
                sprite.color = new Color(1, 1, 1, 0.2f);
            }
        }
        if (shardNumber == 4)
        {
            if (PlayerPrefs.GetInt("level" + SceneManager.GetActiveScene().buildIndex + "ShardFourFound") == 1)
            {
                sprite.color = new Color(1, 1, 1, 0.2f);
            }
        }

    }
    void DestroySequence()
    {
        Destroy(gameObject, 1);
        ChangeAnimation("Honeyshard_Consume");
    }

    void ChangeAnimation(string animName)
    {
        if(animator != null)
        {
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != animName)
            {
                animator.Play(animName);
            }
        }
        
    }

}
