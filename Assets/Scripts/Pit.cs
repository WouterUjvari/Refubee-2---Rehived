using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pit : MonoBehaviour
{
    [SerializeField] Transform respawnpoint;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            StartCoroutine(TeleportRoutine());
        }
    }

    IEnumerator TeleportRoutine()
    {
        GameManager.Instance.playerScript.rbPlayer.isKinematic = true;
        GameManager.Instance.playerScript.rbPlayer.velocity = Vector3.zero;
        GameManager.Instance.readControls = false;
        GameManager.Instance.TakeHealth(-2);
        GameManager.Instance.playerScript.stunned = true;
        GameManager.Instance.playerScript.damageCooldown = 0;
        GameManager.Instance.playerScript.dealdamageCooldown = 0;
        Hud.Instance.fadeAnimator.Play("FadeEffect_CircleZoomIn");
        yield return new WaitForSeconds(1);
        GameManager.Instance.playerScript.transform.position = respawnpoint.position;
        yield return new WaitForSeconds(0.5f);
        Hud.Instance.fadeAnimator.Play("FadeEffect_CircleZoomOut");
        yield return new WaitForSeconds(1);
        GameManager.Instance.playerScript.rbPlayer.isKinematic = false;
        GameManager.Instance.readControls = true;
        GameManager.Instance.playerScript.rbPlayer.velocity = Vector3.zero;
    }
}
