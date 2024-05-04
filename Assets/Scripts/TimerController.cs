using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TimerController : MonoBehaviour
{
    [SerializeField] private float timeCounter;
    public float countdownTimer = 120f;
    [SerializeField] private int minutes;
    [SerializeField] private int seconds;
    [SerializeField] private bool isCountdown;
    [SerializeField] private bool active;
    [SerializeField] private TextMeshProUGUI timerText;

    private void Start()
    {
        int minutes = Mathf.FloorToInt(isCountdown ? countdownTimer / 60f : timeCounter / 60f);
        int seconds = Mathf.FloorToInt(isCountdown ? countdownTimer - minutes * 60 : timeCounter - minutes * 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    private void Update()
    {
        if(active)
        {
            if (isCountdown && countdownTimer > 0)
            {
                countdownTimer = Mathf.Clamp(countdownTimer -= Time.deltaTime, 0, 60 * 8);
            }
            else if (!isCountdown)
            {
                timeCounter += Time.deltaTime;
            }
            int minutes = Mathf.FloorToInt(isCountdown ? countdownTimer / 60f : timeCounter / 60f);
            int seconds = Mathf.FloorToInt(isCountdown ? countdownTimer - minutes * 60 : timeCounter - minutes * 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }     
    }
}
