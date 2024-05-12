using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField] private Animator animator;
    [SerializeField] private bool boot;

    private void Awake()
    {
        Instance = this;    
    }
    public void StartLoadLevel(int SceneIndex)
    {
        Instance.StartCoroutine(LoadLevel(SceneIndex));
    }

    IEnumerator LoadLevel(int SceneIndex)
    {
        yield return new WaitForSeconds(1);
        Debug.Log("Loading scene " + SceneIndex);
        SceneManager.LoadScene(SceneIndex);
        yield return new WaitForSeconds(5);
    }



    void ChangeAnimation(string animName)
    {
        if (animator != null)
        {
            if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != animName)
            {
                animator.Play(animName);
            }
        }
    }
}
