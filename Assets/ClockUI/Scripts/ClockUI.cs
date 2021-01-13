/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockUI : MonoBehaviour {

    private const float REAL_SECONDS_PER_INGAME_DAY = 60 * 24 * 60;

    private Transform clockHourHandTransform;
    private Transform clockMinuteHandTransform;
    private Text timeText;
    public float day;
    public bool stop;
    public string name;

    private void Awake() {
        clockHourHandTransform = transform.Find("hourHand");
        clockMinuteHandTransform = transform.Find("minuteHand");
        timeText = transform.Find("timeText").GetComponent<Text>();
        stop = true;
    }

    public void startTimer(float time)
    {
        day = time / (60 * 24 * 60);
        updateText();
    }

    private void updateText()
    {
        float dayNormalized = day % 1f;

        float rotationDegreesPerDay = 360f;
        clockHourHandTransform.eulerAngles = new Vector3(0, 0, -dayNormalized * rotationDegreesPerDay * 24);

        float hoursPerDay = 24f;
        clockMinuteHandTransform.eulerAngles = new Vector3(0, 0, -dayNormalized * rotationDegreesPerDay * 24 * 60);

        string hoursString = Mathf.Floor(dayNormalized * hoursPerDay).ToString("00");

        float minutesPerHour = 60f;
        string minutesString = Mathf.Floor(((dayNormalized * hoursPerDay) % 1f) * minutesPerHour).ToString("00");
        string secondsString = Mathf.Floor(((((dayNormalized * hoursPerDay) % 1f) * minutesPerHour) % 1f) * 60).ToString("00");

        timeText.text = minutesString + ":" + secondsString;
    }

    private void Update() {
        if (stop)
            return;

        day -= Time.deltaTime / REAL_SECONDS_PER_INGAME_DAY;
        updateText();
    }

}
