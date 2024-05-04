using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuzzerBlock : MonoBehaviour
{
    [SerializeField] private bool switchedOn;
    [SerializeField] private GameObject OnBlock;
    [SerializeField] private GameObject OffBlock;

    private bool doneItsJob;

    private void Start()
    {
        OnBlock.SetActive(switchedOn);
        OffBlock.SetActive(!switchedOn);
    }
    private void FixedUpdate()
    {
        if (GameManager.Instance.buzzerActivated)
        {
            if(!doneItsJob)
            {
                Flip();
            }          
        }
    }

    void Flip()
    {
        doneItsJob = true;
        switchedOn = !switchedOn;

        OnBlock.SetActive(switchedOn);
        OffBlock.SetActive(!switchedOn);
    }
}
