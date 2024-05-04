using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    void Update()
    {
        Vector2 vect = GameManager.Instance.playerScript.transform.position;
        transform.position = new Vector3(vect.x, vect.y + 0.7f);
    }
}
