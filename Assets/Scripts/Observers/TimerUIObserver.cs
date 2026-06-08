using System;
using UnityEngine;
using UnityEngine.UI;

public class TimerUIObserver : Observer
{
    private const string TextTimerTag = "TextTimer";

    private GameController gameController;
    private float Minutes;
    private float Seconds;
    private Text TextTimer;
    private readonly float MaxMinutes;
    private readonly float StartTime;
    private bool timeExpired;

    public TimerUIObserver(GameController gameController, float maxMinutes)
    {
        GameObject textTimerObject = GameObject.FindGameObjectWithTag(TextTimerTag);
        if (textTimerObject)
        {
            TextTimer = textTimerObject.GetComponent<Text>();
        }
        else
        {
            Debug.LogError("TimerUIObserver requires a TextTimer tagged Text object in the scene.");
        }

        this.gameController = gameController;
        MaxMinutes = maxMinutes;
        StartTime = Time.time;
        this.gameController.Attach(this);
    }

    public override void UpdateObserver()
    {
        GameTimer();
    }

    private void GameTimer()
    {
        if (timeExpired || !TextTimer)
        {
            return;
        }

        float elapsedTime = Time.time - StartTime;
        Minutes = (int)(elapsedTime / 60);
        Seconds = (int)(elapsedTime % 60);
        TextTimer.text = Minutes.ToString("00") + ":" + Seconds.ToString("00");

        if (Minutes >= MaxMinutes)
        {
            timeExpired = true;
            gameController.ShowTimeUpPanel();
            Time.timeScale = 0f;
        }
    }
}
