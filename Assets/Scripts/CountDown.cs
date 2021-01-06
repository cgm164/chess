using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class CountDown : MonoBehaviour
{

    public float timeLeft;
    public bool stop = true;

    private float minutes;
    private float seconds;
    private bool end = false;

    public Text text;

    public void startTimer(float from)
    {
        timeLeft = from;
        minutes = Mathf.Floor(timeLeft / 60);
        seconds = timeLeft % 60;
        Update();
        StartCoroutine(updateCoroutine());
    }

    void Update()
    {
        if (stop) return;
        timeLeft -= Time.deltaTime;

        minutes = Mathf.Floor(timeLeft / 60);
        seconds = timeLeft % 60;
        if (seconds > 59) seconds = 59;
        if (minutes < 0)
        {
            stop = true;
            minutes = 0;
            seconds = 0;
        }

        if (timeLeft < 0)
            end = true;
        //        fraction = (timeLeft * 100) % 100;
    }

    private IEnumerator updateCoroutine()
    {
        while (!end)
        {
            text.text = string.Format("{0:0}:{1:00}", minutes, seconds);
            yield return new WaitForSeconds(0.2f);
        }
    }
}