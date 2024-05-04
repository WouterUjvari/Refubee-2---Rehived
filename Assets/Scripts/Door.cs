using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform teleLocation;
    [SerializeField] private Transform prompt;
    public float teleportCooldown;
    private void Update()
    {
        if(teleLocation != null)
        {
            teleportCooldown = Mathf.Clamp(teleportCooldown += Time.deltaTime, 0, 4);
            if(GameManager.Instance.playerScript != null)
            {
                if (Vector2.Distance(transform.position, GameManager.Instance.playerScript.transform.position) < 0.5f && teleportCooldown >= 2)
                {
                    prompt.gameObject.SetActive(true);
                    if (Input.GetAxisRaw("Vertical") > 0)
                    {
                        StartCoroutine(Teleport());

                    }
                }
                else
                {
                    prompt.gameObject.SetActive(false);
                }
            }           
        }       
    }

    private IEnumerator Teleport()
    {
        
        GameManager.Instance.playerScript.rbPlayer.isKinematic = true;
        GameManager.Instance.playerScript.rbPlayer.velocity = Vector3.zero;
        GameManager.Instance.readControls = false;
        teleportCooldown = 0;
        teleLocation.GetComponent<Door>().teleportCooldown = 0;
        Hud.Instance.fadeAnimator.Play("FadeEffect_CircleZoomIn");
        yield return new WaitForSeconds(1);
        GameManager.Instance.playerScript.transform.position = teleLocation.position;
        yield return new WaitForSeconds(0.5f);
        Hud.Instance.fadeAnimator.Play("FadeEffect_CircleZoomOut");
        yield return new WaitForSeconds(1);
        GameManager.Instance.playerScript.rbPlayer.isKinematic = false;
        GameManager.Instance.readControls = true;
        GameManager.Instance.playerScript.rbPlayer.velocity = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        if (teleLocation != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(prompt.transform.position, teleLocation.transform.position);
        }
    }
}
