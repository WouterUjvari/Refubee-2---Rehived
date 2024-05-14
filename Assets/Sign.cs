using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sign : MonoBehaviour
{
    [SerializeField] private Transform prompt;
    // Start is called before the first frame update
    void Update()
    {
        float distance = Vector2.Distance(transform.position, GameManager.Instance.playerScript.transform.position);
        if (distance < 2)
        {
            prompt.gameObject.SetActive(true);
        }
        else
        {
            prompt.gameObject.SetActive(false);
        }
    }
}
