using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomConnecter : MonoBehaviour
{
    [SerializeField] Transform teleportLocation;
    [SerializeField] enum RelativeLocation { left, right, bottom, top };
    [SerializeField] RelativeLocation currentRelativeLocation;
    [SerializeField] float cooldown;

    private void Update()
    {
        cooldown =  Mathf.Clamp(cooldown += Time.deltaTime, 0, 1);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player" && cooldown >= 0.2f)
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
            Hud.Instance.fadeAnimator.Play("SlideRight");
            yield return new WaitForSeconds(0.25f);
            GameManager.Instance.playerScript.transform.position = new Vector2(vect.x + 1, vect.y);
            GameManager.Instance.vCam.SetActive(false);

            Hud.Instance.fadeAnimator.Play("SlideLeft 0");
            GameManager.Instance.vCam.SetActive(true);
        }
        if (currentRelativeLocation == RelativeLocation.right)
        {
            Hud.Instance.fadeAnimator.Play("SlideLeft");
            yield return new WaitForSeconds(0.25f);
            GameManager.Instance.playerScript.transform.position = new Vector2(vect.x + -1, vect.y);
            GameManager.Instance.vCam.SetActive(false);

            Hud.Instance.fadeAnimator.Play("SlideRight 0");
            GameManager.Instance.vCam.SetActive(true);
        }
        if (currentRelativeLocation == RelativeLocation.bottom)
        {
            Hud.Instance.fadeAnimator.Play("SlideTop");
            yield return new WaitForSeconds(0.25f);
            GameManager.Instance.playerScript.transform.position = new Vector2(vect.x, vect.y + 1);
            GameManager.Instance.vCam.SetActive(false);

            Hud.Instance.fadeAnimator.Play("SlideBottom 0");
            GameManager.Instance.vCam.SetActive(true);
        }
        if (currentRelativeLocation == RelativeLocation.top)
        {
            Hud.Instance.fadeAnimator.Play("SlideBottom");
            yield return new WaitForSeconds(0.25f);
            GameManager.Instance.playerScript.transform.position = new Vector2(vect.x + 2, vect.y - 1);
            GameManager.Instance.vCam.SetActive(false);

            Hud.Instance.fadeAnimator.Play("SlideTop 0");
            GameManager.Instance.vCam.SetActive(true);
        }
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
