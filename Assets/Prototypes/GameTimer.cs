using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float timeRemaining = 60f;
    public TMP_Text timerText;

    private void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = $"TIME: {Mathf.Round(timeRemaining)}";
        }
        else
        {
            //end game logic (Event Placement?)
            timerText.text = "TIMES UP!";
        }
    }
}
