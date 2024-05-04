using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyRoute : MonoBehaviour
{
    [SerializeField] private Transform belongsTo;
    void Start()
    {
        belongsTo = this.transform.parent;
        this.gameObject.transform.parent = null;
    }
}
