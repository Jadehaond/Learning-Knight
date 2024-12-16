using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour 
{
    [SerializeField] private TextMeshProUGUI _timeText;
    private float _timeRemaining;
    public float TimeRemaining => _timeRemaining;
    public bool _timerIsRunning;
    public bool TimerIsRunning => _timerIsRunning;
    
    private void Awake()
    {
        _timerIsRunning = false;
    }
    
    void Update()
    {
        if (_timerIsRunning)
        {
            if (_timeRemaining > 0f)
            {
                _timeRemaining -= Time.deltaTime;
                DisplayTime(_timeRemaining);
            }
            else
            {
                _timeRemaining = 0f;
                _timerIsRunning = false;
            }
        }
    }
    
    public void StartTimer(float number) 
    {
    	_timeRemaining = number;
    	_timerIsRunning = true;
    }
    
    public void StopTimer() 
    {
    	_timerIsRunning = false;
        _timeRemaining = 0f;
    }

    public void AddTimer(float number) 
    {
        if (_timerIsRunning)
        {
    	    _timeRemaining += number;
        }
    }
        
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        //_timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        _timeText.text = string.Format("{0}", seconds);
    }

	IEnumerator MoveObject(GameObject pointA, GameObject pointB, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < 2f)
        {
            transform.position = Vector2.Lerp(pointA.transform.position, pointB.transform.position, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = pointB.transform.position;
    }
}
