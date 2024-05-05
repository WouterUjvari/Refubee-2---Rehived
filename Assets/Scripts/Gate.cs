using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gate : MonoBehaviour
{
    [SerializeField] int keyRequiredIndex;
    [SerializeField] Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("level" + keyRequiredIndex + "KeybeeFound") == 1)
        {
            if(PlayerPrefs.GetInt("level" + keyRequiredIndex + "GateUnlocked") == 0)
            {
                animator.Play("GateKeybeeRoutine");
                PlayerPrefs.SetInt("level" + keyRequiredIndex + "GateUnlocked", 1);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }


}
