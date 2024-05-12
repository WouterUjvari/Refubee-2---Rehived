using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    [Header("saveData")]
    public int levelScore;
    public int LevelKeybeeFound;
    public int levelShardOneFound;
    public int levelShardTwoFound;
    public int levelShardThreeFound;
    public int levelShardFourFound;
    public int spawnX;
    public int spawnY;

    private void Awake()
    {
        instance = this;
        LoadGame();
    }

    public void SaveGame()
    {
        int levelIndex = SceneManager.GetActiveScene().buildIndex;
        levelScore = GameManager.Instance.calculateScore();
        LevelKeybeeFound = Convert.ToInt32(GameManager.Instance.hasKeybee);
        levelShardOneFound = Convert.ToInt32(GameManager.Instance.hasShard[0]);
        levelShardTwoFound = Convert.ToInt32(GameManager.Instance.hasShard[1]);
        levelShardThreeFound = Convert.ToInt32(GameManager.Instance.hasShard[2]);
        levelShardFourFound = Convert.ToInt32(GameManager.Instance.hasShard[3]);

        //Highscore
        if(levelScore > PlayerPrefs.GetInt("level" + SceneManager.GetActiveScene().buildIndex + "Highscore"))
        {
            PlayerPrefs.SetInt("level" + SceneManager.GetActiveScene().buildIndex + "Highscore", levelScore);
        }
        //Keybee
        if (GameManager.Instance.hasKeybee)
        {
            PlayerPrefs.SetInt("level" + SceneManager.GetActiveScene().buildIndex + "KeybeeFound", LevelKeybeeFound);
        }
        //Shards
        if (GameManager.Instance.hasShard[0])
        {
            PlayerPrefs.SetInt("level" + SceneManager.GetActiveScene().buildIndex + "ShardOneFound", levelShardOneFound);
        }
        if (GameManager.Instance.hasShard[1])
        {
            PlayerPrefs.SetInt("level" + SceneManager.GetActiveScene().buildIndex + "ShardTwoFound", levelShardTwoFound);
        }
        if (GameManager.Instance.hasShard[2])
        {
            PlayerPrefs.SetInt("level" + SceneManager.GetActiveScene().buildIndex + "ShardThreeFound", levelShardThreeFound);
        }
        if (GameManager.Instance.hasShard[3])
        {
            PlayerPrefs.SetInt("level" + SceneManager.GetActiveScene().buildIndex + "ShardFourFound", levelShardFourFound);
        }

        PlayerPrefs.Save();

    }
    void LoadGame()
    {

        //Debug.Log("Loading game");
        //PlayerPrefs.GetInt("asdfasdf");
        //Debug.Log(PlayerPrefs.GetInt("asdfasdf"));
    }


}
