using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FullText : MonoBehaviour
{
    [SerializeField] Transform text;

    private void FixedUpdate()
    {

        if (GameManager.Instance.playerCurrentHealthPoints == GameManager.Instance.playerMaxHealthPoints
                && Vector2.Distance(transform.position, GameManager.Instance.playerScript.transform.position) < 1.5f)
        {
            text.gameObject.SetActive(true);
        }
        else
        {
            text.gameObject.SetActive(false);
        }

    }
}
