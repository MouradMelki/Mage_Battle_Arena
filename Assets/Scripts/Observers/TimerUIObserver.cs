using System;
using UnityEngine;
using UnityEngine.UI;

public class TimerUIObserver : Observer
{
    private GameController gameController;
    private float Minutes, Seconds;
    private Text TextTimer;
    private readonly float MaxMinutes = 1f;

    public TimerUIObserver(GameController gameController)
    {
        TextTimer = GameObject.FindGameObjectWithTag("TextTimer").GetComponent<Text>();
        this.gameController = gameController;
        this.gameController.Attach(this);
    }

    public override void UpdateObserver()
    {
        GameTimer();
    }

    private void GameTimer()
    {
        if (!(Minutes >= MaxMinutes))
        {
            Minutes = (int)(Time.time / 60);
            Seconds = (int)(Time.time % 60);
            TextTimer.text = Minutes.ToString("00") + ":" + Seconds.ToString("00");
            if (Minutes >= MaxMinutes)
            {
                Time.timeScale = 0f;
            }
        }
    }
}
