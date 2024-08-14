using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    void Update()
    {
        if(GameManager.Instance.playerScript != null)
        {
            Vector2 vect = GameManager.Instance.playerScript.transform.position;

            transform.position = new Vector3(vect.x, vect.y + 0.7f);
        }
        
    }
}
