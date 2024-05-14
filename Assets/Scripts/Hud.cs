using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    public static Hud Instance;

    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private GameObject heartObjects;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite halfHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private TMP_Text CounterCopper;
    [SerializeField] private TMP_Text CounterSilver;
    [SerializeField] private TMP_Text CounterGold;
    public Animator fadeAnimator;
    public GameObject timer;
    public TimerController timerControllerScript;
    public Image[] heartComponents;
    public Image[] honeyshards;
    public GameObject paused;

    private void Awake()
    {
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }
    private void FixedUpdate()
    {

        DrawHearts();
        DrawPoints();
        DrawShards();   
    }
    void DrawHearts()
    {
        if (GameManager.Instance.playerCurrentHealthPoints <= GameManager.Instance.playerMaxHealthPoints && GameManager.Instance.playerCurrentHealthPoints >= 0)
        {
            int drawAmountFullHearts = GameManager.Instance.playerCurrentHealthPoints / 2;
            int drawAmountHalfHearts = GameManager.Instance.playerCurrentHealthPoints % 2;
            for (int i = 0; i < heartComponents.Length; i++)
            {
                heartComponents[i].sprite = emptyHeart;
            }
            for (int i = 0; i < drawAmountFullHearts; i++)
            {
                heartComponents[i].sprite = fullHeart;
            }
            if (drawAmountHalfHearts != 0)
            {
                heartComponents[drawAmountFullHearts].sprite = halfHeart;
            }
        }     
    }      

    void DrawPoints()
    {
        CounterCopper.text = "X " + GameManager.Instance.coinsCopper;
        CounterSilver.text = "X " + GameManager.Instance.coinsSilver;
        CounterGold.text = "X " + GameManager.Instance.coinsGold;
    }

    void DrawShards()
    {
        for (int i = 0; i < honeyshards.Length; i++)
        {
            honeyshards[i].enabled = GameManager.Instance.hasShard[i];
        }
    }
}
