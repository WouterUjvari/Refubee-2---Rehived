using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public bool readControls = true;
    public int points;
    public int coinsCopper;
    public int coinsSilver;
    public int coinsGold;
    public bool[] hasShard;
    public int playerCurrentHealthPoints, playerMaxHealthPoints;
    public bool hasKeybee;
    public bool buzzerActivated;

    [Header("Compontents(Automatic)")]
    public Keybee keybee;
    public PlayerScript playerScript;
    public PhysicsMaterial2D frictionMaterial;
    public PhysicsMaterial2D noFrictionMaterial;
    [Header("debugspawns")]
    [SerializeField] private GameObject debugObject1;
    [SerializeField] private GameObject debugObject2;
    [SerializeField] private GameObject debugObject3;
    [SerializeField] private GameObject debugObject4;
    [SerializeField] private GameObject debugObject5;
    [SerializeField] private GameObject debugObject6;
    [SerializeField] private GameObject debugObject7;
    [SerializeField] private GameObject debugObject8;
    [SerializeField] private GameObject debugObject9;
    public GameObject camholder;
    public GameObject vCam1;
    public GameObject vCam2;




    private void Awake()
    {
        Instance = this;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        //DontDestroyOnLoad(this.gameObject);       
    }

    private void Update()
    {
        //DebugFunctions();
        if(buzzerActivated)
        {
            Hud.Instance.timer.gameObject.SetActive(true);
            vCam1.SetActive(false);
            vCam2.SetActive(true);
        }
        if(Hud.Instance.timerControllerScript.countdownTimer <=0 && Instance.playerScript.killed == false)
        {
            Instance.playerScript.killed = true;
            Instance.playerCurrentHealthPoints = 0;
            Instance.playerScript.DeathSequence();
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }
    public void TakeHealth(int amount)
    {
        GameManager.Instance.playerCurrentHealthPoints = Mathf.Clamp(GameManager.Instance.playerCurrentHealthPoints += amount, 0, GameManager.Instance.playerMaxHealthPoints);
        Instance.playerScript.currentControlType = PlayerScript.ControlType.Platforming;
        Hud.Instance.fadeAnimator.Play("FlashRed");
        if (playerCurrentHealthPoints ==0)
        {
            Instance.playerScript.DeathSequence();
        }

        if(hasKeybee && amount < 0)
        {
            if (GameManager.Instance.keybee.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "KeybeeDetach")
            {
                GameManager.Instance.keybee.animator.Play("KeybeeDetach");
            }
            hasKeybee = false;
        }     
    }

    public void StartControlRestriction(float duration)
    {
        StartCoroutine(RestrictControls(duration));
    }
    IEnumerator RestrictControls(float duration)
    {
        readControls = false;
        Instance.playerScript.rbPlayer.isKinematic = true;
        Instance.playerScript.rbPlayer.velocity = Vector2.zero;
        yield return new WaitForSeconds(duration);
        readControls = true;
        Instance.playerScript.rbPlayer.isKinematic = false;
    }

    public int calculateScore()
    {
        int score;
        score = (coinsCopper *1 + coinsSilver *5 + coinsGold *25) 
            + (playerCurrentHealthPoints *25) 
            + Mathf.FloorToInt(Hud.Instance.timerControllerScript.countdownTimer * 5);
        return score;
    }
    
    void DebugFunctions()
    {
        if (Input.GetKeyDown("z"))
        {
            if(debugObject1 != null)
            {
                GameObject g = Instantiate(debugObject1);
                g.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                                g.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, 0);
            }
            else { Debug.Log("No Assigned OBJ in this key"); }            
        }
        if (Input.GetKeyDown("x"))
        {
            if (debugObject1 != null)
            {
                GameObject g = Instantiate(debugObject2);
                g.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, 0);
            }
            else { Debug.Log("No Assigned OBJ in this key"); }
        }
        if (Input.GetKeyDown("c"))
        {
            if (debugObject1 != null)
            {
                GameObject g = Instantiate(debugObject3);
                g.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, 0);
            }
            else { Debug.Log("No Assigned OBJ in this key"); }
        }
        if (Input.GetKeyDown("v"))
        {
            if (debugObject1 != null)
            {
                GameObject g = Instantiate(debugObject4);
                g.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, 0);
            }
            else { Debug.Log("No Assigned OBJ in this key"); }
        }
        if (Input.GetKeyDown("b"))
        {
            if (debugObject1 != null)
            {
                GameObject g = Instantiate(debugObject5);
                g.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, 0);
            }
            else { Debug.Log("No Assigned OBJ in this key"); }
        }
    }
}
