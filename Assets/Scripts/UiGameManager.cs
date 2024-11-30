using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGameManager : MonoBehaviour
{
    [SerializeField] private CharacterManager _knight;
    [SerializeField] private GameObject canvasSprint;
    [SerializeField] private GameObject canvasAth;
    [SerializeField] private GameObject canvasQuestions;

    private enum UIState { Options, Controls, Questions, Results, Resume }
    //private UIState _currentState = UIState.Controls;
    [SerializeField] private Timer _timer;
    [SerializeField] private TextMeshProUGUI _timerIntroText;
    private Coroutine _startIntroTimerCoroutine;

    private GameManager _instance;
    private GameManager GameManager => _instance ??= GameManager.Instance;
   
    private void Start()
    {
    }

    private void OnDestroy()
    {
    }

    public void SetFightScene(bool fight)
    {
        canvasSprint.SetActive(!fight);
        canvasQuestions.SetActive(fight);

        if (fight)
        {
            GameManager.LevelManager.ChangeLevelStateByString("Fight");
            GameManager.UiManager.DisplayQuestions();
        }
        else
        {
            GameManager.LevelManager.ChangeLevelStateByString("Running");
            GameManager.UiManager.DisplayControls();
        }

    }

    public void DisplayIntroTimer()
    {
        _startIntroTimerCoroutine = StartCoroutine(StartIntroTimer(3f));
    }

 
    private IEnumerator StartIntroTimer(float countdownTime)
    {
        _timerIntroText.transform.parent.gameObject.SetActive(true);
        float remainingTime = countdownTime;

        while (remainingTime >= 0f)
        {
            _timerIntroText.text = remainingTime.ToString();
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }

        _timerIntroText.transform.parent.gameObject.SetActive(false);
        GameManager.LevelManager.ChangeLevelStateByString("Running");
    }
}
