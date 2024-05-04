using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WarpPortal : MonoBehaviour
{
    private float cooldown;
    [SerializeField] private int sceneIndex;
    [SerializeField] private Animator animator;
    void Update()
    {
        if (GameManager.Instance.buzzerActivated)
        {
            if (GameManager.Instance.readControls)
            {
                cooldown = Mathf.Clamp(cooldown += Time.deltaTime, 0, 1);
                if (cooldown >= 1)
                {
                    float distance = Vector2.Distance(transform.position, GameManager.Instance.playerScript.transform.position);
                    if (distance < 1)
                    {
                        cooldown = 0;
                        
                        StartCoroutine(Warp());
                    }
                }
            }
        }
        animator.SetBool("Open", GameManager.Instance.buzzerActivated);
    }

    IEnumerator Warp()
    {
        SaveManager.instance.SaveGame();
        Hud.Instance.timerControllerScript.enabled = false;
        GameManager.Instance.readControls = false;
        GameManager.Instance.playerScript.rbPlayer.isKinematic = true;
        GameManager.Instance.playerScript.rbPlayer.velocity = Vector2.zero;
        GameManager.Instance.playerScript.transform.position = new Vector2(transform.position.x, transform.position.y + -0.6f);
        animator.SetTrigger("Destroy");
        yield return new WaitForSeconds(0.85f);
        GameManager.Instance.playerScript.animator.gameObject.SetActive(false);
        Hud.Instance.fadeAnimator.Play("FadeEffect_CircleZoomIn");
        yield return new WaitForSeconds(1);     
        GameManager.Instance.keybee.transform.position = transform.position;
        Scoreboard.instance.StartScoreboard();
        yield return new WaitForSeconds(18);
        Hud.Instance.fadeAnimator.Play("FadeEffect_CircleZoomOut");
        yield return new WaitForSeconds(1);
        LevelManager.Instance.StartLoadLevel(sceneIndex);
    }
}
