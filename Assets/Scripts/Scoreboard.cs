using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.AllocatorManager;

public class Scoreboard : MonoBehaviour
{
    static public Scoreboard instance;

    [SerializeField] private GameObject canvas;
    [SerializeField] private float displayDelay;
    [SerializeField] private GameObject coinsCopper;
    [SerializeField] private TMP_Text CounterCopper;
    [SerializeField] private GameObject coinsSilver;
    [SerializeField] private TMP_Text CounterSilver;
    [SerializeField] private GameObject coinsGold;
    [SerializeField] private TMP_Text CounterGold;
    [SerializeField] private GameObject timeLeft;
    [SerializeField] private TMP_Text CounterTime;
    [SerializeField] private GameObject heartsLeft;
    [SerializeField] private TMP_Text CounterHearts;
    [SerializeField] private GameObject finalScore;
    [SerializeField] private TMP_Text CounterScore;
    [SerializeField] private GameObject shardEmpty;
    [SerializeField] private GameObject shard1;
    [SerializeField] private GameObject shard2;
    [SerializeField] private GameObject shard3;
    [SerializeField] private GameObject shard4;
    [SerializeField] private GameObject savedShard1;
    [SerializeField] private GameObject savedShard2;
    [SerializeField] private GameObject savedShard3;
    [SerializeField] private GameObject savedShard4;
    [SerializeField] private GameObject keybee;

    private void Start()
    {
        instance = this;
        canvas.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButton("Jump"))
        {
            displayDelay = 0.1f;
        }
        else
        {
            displayDelay = 0.5f;
        }
    }

    public void StartScoreboard()
    {
        canvas.SetActive(true);
        StartCoroutine(DisplayRoutine());
    }

    IEnumerator DisplayRoutine()
    {      
        yield return new WaitForSeconds(1);
        //copper
        coinsCopper.SetActive(true);
        yield return new WaitForSeconds(displayDelay);
        CounterCopper.text = " X " + GameManager.Instance.coinsCopper + "";
        yield return new WaitForSeconds(displayDelay);
        CounterCopper.text = " X " + GameManager.Instance.coinsCopper + " X " + "1p" + " = " + GameManager.Instance.coinsCopper * 1;
        yield return new WaitForSeconds(displayDelay);
        //silver
        coinsSilver.SetActive(true);
        yield return new WaitForSeconds(displayDelay);
        CounterSilver.text = " X " + GameManager.Instance.coinsSilver + "";
        yield return new WaitForSeconds(displayDelay);
        CounterSilver.text = " X " + GameManager.Instance.coinsSilver + " X " + "5p" + " = " + GameManager.Instance.coinsSilver * 5;
        yield return new WaitForSeconds(displayDelay);
        //gold
        coinsGold.SetActive(true);
        yield return new WaitForSeconds(displayDelay);
        CounterGold.text = " X " + GameManager.Instance.coinsGold + "";
        yield return new WaitForSeconds(displayDelay);
        CounterGold.text = " X " + GameManager.Instance.coinsGold + " X " + "25p" + " = " + GameManager.Instance.coinsGold * 25;
        yield return new WaitForSeconds(displayDelay);

        //time
        timeLeft.SetActive(true);
        yield return new WaitForSeconds(displayDelay);
        CounterTime.text = " X " + Mathf.FloorToInt(Hud.Instance.timerControllerScript.countdownTimer) + "";
        yield return new WaitForSeconds(displayDelay);
        CounterTime.text = " X " + Mathf.FloorToInt(Hud.Instance.timerControllerScript.countdownTimer) + " X " + "5p" + " = " + Mathf.FloorToInt(Hud.Instance.timerControllerScript.countdownTimer) * 5;
        yield return new WaitForSeconds(displayDelay);

        //hearts
        heartsLeft.SetActive(true);
        yield return new WaitForSeconds(displayDelay);
        CounterHearts.text = " X " + GameManager.Instance.playerCurrentHealthPoints + "";
        yield return new WaitForSeconds(displayDelay);
        CounterHearts.text = " X " + GameManager.Instance.playerCurrentHealthPoints + " X " + "25p" + " = " + GameManager.Instance.playerCurrentHealthPoints * 25;
        yield return new WaitForSeconds(displayDelay);

        //finalscore
        finalScore.SetActive(true);
        yield return new WaitForSeconds(displayDelay);
        CounterScore.text = "Final score: " + GameManager.Instance.calculateScore();
        yield return new WaitForSeconds(displayDelay);

        //keybee
        yield return new WaitForSeconds(displayDelay);
        if (GameManager.Instance.hasKeybee == true)
        {
            keybee.SetActive(true);
        }

        yield return new WaitForSeconds(displayDelay);
        shardEmpty.SetActive(true);
        if(PlayerPrefs.GetInt("level" + SceneManager.GetActiveScene().buildIndex + "ShardOneFound") == 1)
        {
            savedShard1.SetActive(true);
        }
        if (PlayerPrefs.GetInt("level" + SceneManager.GetActiveScene().buildIndex + "ShardTwoFound") == 1)
        {
            savedShard2.SetActive(true);
        }
        if (PlayerPrefs.GetInt("level" + SceneManager.GetActiveScene().buildIndex + "ShardThreeFound") == 1)
        {
            savedShard3.SetActive(true);
        }
        if (PlayerPrefs.GetInt("level" + SceneManager.GetActiveScene().buildIndex + "ShardFourFound") == 1)
        {
            savedShard4.SetActive(true);
        }

        yield return new WaitForSeconds(displayDelay);
        //shards
        if (GameManager.Instance.hasShard[0])
        {
            shard1.SetActive(true);
        }
        
        yield return new WaitForSeconds(displayDelay);
        if (GameManager.Instance.hasShard[1])
        {
            shard2.SetActive(true);
        }

        yield return new WaitForSeconds(displayDelay);
        if (GameManager.Instance.hasShard[2])
        {
            shard3.SetActive(true);
        }

        yield return new WaitForSeconds(displayDelay);
        if (GameManager.Instance.hasShard[3])
        {
            shard4.SetActive(true);
        }
        yield return new WaitForSeconds(displayDelay);
        SaveManager.instance.SaveGame();
        yield return new WaitForSeconds(displayDelay);
        LevelManager.Instance.StartLoadLevel(0);
    }

}
