using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionManager : MonoBehaviour
{
    private const string MathsFilePath = "/Resources/banque-questions-mathématiques.csv";
    private const string FrenchFilePath = "/Resources/banque-questions-français.csv";
    [SerializeField] private Button _ans1;
    [SerializeField] private Button _ans2;
    [SerializeField] private Button _ans3;
    [SerializeField] private Button _ans4;
    [SerializeField] private TextMeshProUGUI _question;
    [SerializeField] private Timer _timer;
    [SerializeField] private GameObject _knight;

    private string _pathFile;
    private string _type;
    private string _genre;
    private string _niveau;
    private string[] _responses = new string[4];
    private string _currentQuestion;
    private int _correctAnswerIndex;

    private GameManager _instance;
    private GameManager GameManager => _instance ??= GameManager.Instance;

    private bool _isFirstQuestionDisplayed = false;
    private bool _isQuestionDisplayed = false;
    private bool _isResponsesDisplayed = false;
    private GameObject _enemy;

    private void Start()
    {
        _ans1.onClick.AddListener(() => OnAnswerButtonClicked(0));
        _ans2.onClick.AddListener(() => OnAnswerButtonClicked(1));
        _ans3.onClick.AddListener(() => OnAnswerButtonClicked(2));
        _ans4.onClick.AddListener(() => OnAnswerButtonClicked(3));
        Setup();
    }

    public void SetType(string type)
    {
        _type = type;
        _pathFile = _type == "Maths" ? MathsFilePath : FrenchFilePath;
    }

	public void SetEnemy(GameObject enemy)
    {
        _enemy = enemy;
    }

    public void Reset()
    {
        _timer.StopTimer();
        ResetQuestion();
        _isFirstQuestionDisplayed = false;
    }
    
    private void Update()
    {
        if (!GameManager.PauseManager.IsPaused && GameManager.LevelManager.IsCurrentLevelState("Fighting") && GameManager.UiManager.IsCurrentState("Questions"))
        {
            if (!_isFirstQuestionDisplayed)
            {
                StartQuestionSequence();
            }
            else
            {
                HandleQuestionDisplay();
            }
        }
    }

    private void Setup()
    {
        _isFirstQuestionDisplayed = false;
        ResetQuestion();
    }

    private void StartQuestionSequence()
    {
        _isFirstQuestionDisplayed = true;
        StartQuestion();
    }

    private void HandleQuestionDisplay()
    {
        if (_isQuestionDisplayed && !_isResponsesDisplayed && !_timer.TimerIsRunning)
        {
            DisplayResponses();
        }
        else if (_isResponsesDisplayed && !_isQuestionDisplayed && !_timer.TimerIsRunning)
        {
            _question.text = "Time Run Out!";
            HandleTimeOut();
        }
    }

    private void OnAnswerButtonClicked(int buttonIndex)
    {
        if (buttonIndex == _correctAnswerIndex)
        {
            HandleCorrectAnswer();
        }
        else
        {
            HandleIncorrectAnswer();
        }
    }

    private void HandleCorrectAnswer()
    {
        _question.text = "Bonne réponse !";   
        _timer.StopTimer();
        ResetQuestion();

        int damage = 5 + Mathf.RoundToInt(10 * _timer.TimeRemaining / 100);
        var enemyHealth = _enemy.GetComponent<CharacterManager>();
        enemyHealth.DamageLife(damage);

        if (enemyHealth.IsDead())
        {
            _knight.GetComponent<CharacterManager>().GainLife(5);
            _isFirstQuestionDisplayed = false;
            ResetQuestion();
            GameManager.LevelManager.EndFight();
        }
        else
        {
            StartQuestion();
        }
    }

    private void HandleIncorrectAnswer()
    {
        _question.text = "Mauvaise réponse !";
        _knight.GetComponent<CharacterManager>().DamageLife(5);
        CheckKnightHealth();
    }

    private void HandleTimeOut()
    {
        _isResponsesDisplayed = false;
        _knight.GetComponent<CharacterManager>().DamageLife(5);
        CheckKnightHealth();
        StartQuestion();
    }

    private void CheckKnightHealth()
    {
        if (_knight.GetComponent<CharacterManager>().IsDead())
        {
            Reset();
            GameManager.LevelManager.DisplayResults();
        }
        else
        {
            StartQuestion();
        }
    }

    private void StartQuestion() 
    {     
        ResetQuestion();
        GetQuestion();
        _timer.StartTimer(3f);
        DisplayQuestion();	

        Invoke(nameof(StartResponses), 3f);
    }

    private void StartResponses()
    {
        _timer.StartTimer(10f);
        DisplayResponses();
    }

    private void GetQuestion() 
    {
        if (!System.IO.File.Exists(Application.dataPath + _pathFile))
        {
            Debug.LogError("File not found");
            return;
        }

        string fileData = System.IO.File.ReadAllText(Application.dataPath + _pathFile);
        string[] lines = fileData.Split("\n"[0]);
        int randIndex = UnityEngine.Random.Range(0, lines.Length);
        string[] lineData = (lines[randIndex].Trim()).Split(","[0]);
            
        _genre = lineData[0];
		_niveau = lineData[1];
        _currentQuestion = lineData[2];

        _correctAnswerIndex = UnityEngine.Random.Range(0, 4);
        _responses[_correctAnswerIndex] = lineData[3];

        int responseIndex = (_correctAnswerIndex + 1) % 4;

        for (int i = 4; i < 7; i++)
        {
            _responses[responseIndex] = lineData[i];
            responseIndex = (responseIndex + 1) % 4;
            
        }
    }

    private void DisplayQuestion() 
    {
        _question.text = _currentQuestion;
        _isQuestionDisplayed = true;
    }

    private void DisplayResponses()
    {
        _ans1.GetComponentInChildren<TextMeshProUGUI>().text = _responses[0];
        _ans2.GetComponentInChildren<TextMeshProUGUI>().text = _responses[1];
        _ans3.GetComponentInChildren<TextMeshProUGUI>().text = _responses[2];
        _ans4.GetComponentInChildren<TextMeshProUGUI>().text = _responses[3];
        _isResponsesDisplayed = true;
        _isQuestionDisplayed = false;
    }

    private void ResetQuestion()
    {
        _question.text = "";
        foreach (var button in new[] { _ans1, _ans2, _ans3, _ans4 })
        {
            button.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
        _isResponsesDisplayed = false;
        _isQuestionDisplayed = false;
    }
    
    public void UseEnDeux()
    {
        if (_isResponsesDisplayed && !_isQuestionDisplayed && _timer.TimerIsRunning) 
        {
            // Call some functionality on the knight's ObjectManager
            // _knight.GetComponent<ObjectManager>().run(Reponses, vraieReponse);
            DisplayResponses();
        }
    }
}