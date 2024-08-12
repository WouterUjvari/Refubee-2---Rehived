using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomConnecter : MonoBehaviour
{
    [SerializeField] Transform teleportLocation;
    [SerializeField] enum RelativeLocation { left, right, bottom, top };
    [SerializeField] RelativeLocation currentRelativeLocation;
    public float cooldown;

    private void Update()
    {
        cooldown =  Mathf.Clamp(cooldown += Time.deltaTime, 0, 1);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player" && cooldown >= 0.5f)
        {          
            StartCoroutine(TeleportRoutine());
        }
    }

    IEnumerator TeleportRoutine()
    {
        Vector2 vect = teleportLocation.position;
        cooldown = 0;
        teleportLocation.GetComponent<RoomConnecter>().cooldown = 0;
        
        
        if (currentRelativeLocation == RelativeLocation.left)
        {

            GameManager.Instance.camholder.SetActive(false);
            Vector2 currentVelocity = GameManager.Instance.playerScript.rbPlayer.velocity;
            GameManager.Instance.playerScript.rbPlayer.isKinematic = true;
            GameManager.Instance.playerScript.rbPlayer.velocity = Vector2.zero;
            GameManager.Instance.playerScript.enabled = false;
            Hud.Instance.fadeAnimator.Play("SlideRight");
            yield return new WaitForSeconds(0.25f);
            GameManager.Instance.playerScript.transform.position = new Vector2(vect.x + 2, vect.y -0.5f);        
            yield return new WaitForSeconds(0.1f);
            Hud.Instance.fadeAnimator.Play("SlideLeft 0");
            GameManager.Instance.camholder.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            GameManager.Instance.playerScript.enabled = true;
            GameManager.Instance.playerScript.rbPlayer.isKinematic = false;
            GameManager.Instance.playerScript.rbPlayer.velocity = currentVelocity;
        }
        if (currentRelativeLocation == RelativeLocation.right)
        {
            GameManager.Instance.camholder.SetActive(false);
            Vector2 currentVelocity = GameManager.Instance.playerScript.rbPlayer.velocity;
            GameManager.Instance.playerScript.rbPlayer.isKinematic = true;
            GameManager.Instance.playerScript.rbPlayer.velocity = Vector2.zero;
            GameManager.Instance.playerScript.enabled = false;
            Hud.Instance.fadeAnimator.Play("SlideLeft");
            yield return new WaitForSeconds(0.25f);
            GameManager.Instance.playerScript.transform.position = new Vector2(vect.x + -2, vect.y -0.5f);

            yield return new WaitForSeconds(0.1f);
            Hud.Instance.fadeAnimator.Play("SlideRight 0");
            GameManager.Instance.camholder.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            GameManager.Instance.playerScript.enabled = true;
            GameManager.Instance.playerScript.rbPlayer.isKinematic = false;
            GameManager.Instance.playerScript.rbPlayer.velocity = currentVelocity;
        }
        if (currentRelativeLocation == RelativeLocation.bottom)
        {
            GameManager.Instance.camholder.SetActive(false);
            Vector2 currentVelocity = GameManager.Instance.playerScript.rbPlayer.velocity;
            GameManager.Instance.playerScript.rbPlayer.isKinematic = true;
            GameManager.Instance.playerScript.rbPlayer.velocity = Vector2.zero;
            GameManager.Instance.playerScript.enabled = false;
            Hud.Instance.fadeAnimator.Play("SlideTop");
            yield return new WaitForSeconds(0.25f);
            GameManager.Instance.playerScript.transform.position = new Vector2(vect.x, vect.y + 1);

            yield return new WaitForSeconds(0.1f);
            Hud.Instance.fadeAnimator.Play("SlideBottom 0");
            GameManager.Instance.camholder.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            GameManager.Instance.playerScript.enabled = true;
            GameManager.Instance.playerScript.rbPlayer.isKinematic = false;
            GameManager.Instance.playerScript.rbPlayer.velocity = currentVelocity;
        }
        if (currentRelativeLocation == RelativeLocation.top)
        {
            GameManager.Instance.camholder.SetActive(false);
            Vector2 currentVelocity = GameManager.Instance.playerScript.rbPlayer.velocity;
            GameManager.Instance.playerScript.rbPlayer.isKinematic = true;
            GameManager.Instance.playerScript.rbPlayer.velocity = Vector2.zero;
            GameManager.Instance.playerScript.enabled = false;
            Hud.Instance.fadeAnimator.Play("SlideBottom");
            yield return new WaitForSeconds(0.25f);
            GameManager.Instance.playerScript.transform.position = new Vector2(vect.x, vect.y - 1);

            yield return new WaitForSeconds(0.1f);
            Hud.Instance.fadeAnimator.Play("SlideTop 0");
            GameManager.Instance.camholder.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            GameManager.Instance.playerScript.enabled = true;
            GameManager.Instance.playerScript.rbPlayer.isKinematic = false;
            GameManager.Instance.playerScript.rbPlayer.velocity = currentVelocity;
        }

        GameManager.Instance.playerScript.rbPlayer.mass = 1;
    }
    private void OnDrawGizmos()
    {
        if (teleportLocation != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, teleportLocation.transform.position);
        }
    }
}
