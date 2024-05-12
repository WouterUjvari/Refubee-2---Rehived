using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class SceneDoor : MonoBehaviour
{

    [SerializeField] private bool isWarpPortal;
    private float cooldown;
    [SerializeField] private int sceneIndex;
    [SerializeField] private Transform prompt;
    [SerializeField] private Transform scoreObj;
    [SerializeField] private TMP_Text score;
    [SerializeField] private TMP_Text shards;
    [SerializeField] private TMP_Text key;

    // Update is called once per frame

    void Start()
    {
        GameObject.FindGameObjectWithTag("Player").gameObject.transform.position = new Vector2(PlayerPrefs.GetInt("SpawnX"), PlayerPrefs.GetInt("SpawnY"));
        int shardscount = 0;
        if (PlayerPrefs.GetInt("level" + sceneIndex + "ShardOneFound") == 1)
        {
            shardscount++;
        }
        if (PlayerPrefs.GetInt("level" + sceneIndex + "ShardTwoFound") == 1)
        {
            shardscount++;
        }
        if (PlayerPrefs.GetInt("level" + sceneIndex + "ShardThreeFound") == 1)
        {
            shardscount++;
        }
        if (PlayerPrefs.GetInt("level" + sceneIndex + "ShardFourFound") == 1)
        {
            shardscount++;
        }
        Debug.Log(shardscount);
        score.text = "Score: "+ PlayerPrefs.GetInt("level" + sceneIndex + "Highscore") + "";
        shards.text = "Shards: "+ shardscount+ "/4";
        key.text = "Keybee: "+ PlayerPrefs.GetInt("level" + sceneIndex + "KeybeeFound") + "/1";
        
    }
    
    
    void Update()
    {
        if (true)
        {
            if (GameManager.Instance.readControls)
            {
                cooldown = Mathf.Clamp(cooldown += Time.deltaTime, 0, 5);
                if (cooldown >= 3)
                {
                    float distance = Vector2.Distance(transform.position, GameManager.Instance.playerScript.transform.position);
                    if (distance < 2)
                    {
                        prompt.gameObject.SetActive(true);
                        DisplayScore(); 
                        if (Input.GetAxisRaw("Vertical") > 0)
                        {
                            cooldown = 0;
                            StartCoroutine(Warp());
                        }
                    }
                    else
                    {
                        prompt.gameObject.SetActive(false);
                        HideScore();
                    }
                }
            }
        }
                       
    }
    IEnumerator Warp()
    {
        PlayerPrefs.SetInt("SpawnX", Mathf.RoundToInt(transform.position.x));
        PlayerPrefs.SetInt("SpawnY", Mathf.RoundToInt(transform.position.y));
        GameManager.Instance.readControls = false;
        GameManager.Instance.playerScript.rbPlayer.isKinematic = true;
        GameManager.Instance.playerScript.rbPlayer.velocity = Vector2.zero;
        Hud.Instance.fadeAnimator.Play("FadeEffect_CircleZoomIn");
        yield return new WaitForSeconds(1);
        LevelManager.Instance.StartLoadLevel(sceneIndex);
    }

    void DisplayScore()
    {
        scoreObj.gameObject.SetActive(true);

    }

    void HideScore()
    {
        scoreObj.gameObject.SetActive(false);
    }
}
