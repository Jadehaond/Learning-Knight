using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;  // Pour l'utilisation de LINQ
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<Level> _levels = new List<Level>();
    [SerializeField] private GameObject _knight;
    [SerializeField] private GameObject _enemy;
    [SerializeField] private Transform _levelParent;
    [SerializeField] private GameObject _levelButton;
    [SerializeField] private Transform _levelButtonParent;
    [SerializeField] private TextMeshProUGUI _resultsText;
    [SerializeField] private Button _nextResults;
    [SerializeField] private Button _reloadResults;
    public FocusCamera GameCamera;

    private GameManager _instance;
    private GameManager GameManager => _instance ??= GameManager.Instance;

    private Level _currentLevel;
    private enum LevelState {NotStarted, Setup, Running, Fighting, Finished}
    private LevelState _currentLevelState = LevelState.NotStarted;

    private void Awake()
    {
        ChangeLevelState(LevelState.NotStarted);
    }

    public bool IsCurrentLevelState(string stateName)
    {
        return _currentLevelState.ToString() == stateName;
    }

    public void GenerateButtons()
    {
        if (_levelButtonParent.childCount == 0)
        {
            foreach (Level level in _levels)
            {
                GameObject button = Instantiate(_levelButton, _levelButtonParent);
                button.name = level.name;
                LevelButtonUI levelButtonUI = button.GetComponent<LevelButtonUI>();
                levelButtonUI.SetLevelButton(level);

                Button uiButton = button.GetComponent<Button>();
                if (uiButton != null)
                {
                    uiButton.onClick.AddListener(() => LoadLevel(level));
                }
            }
        }
    }

    public void ChangeLevelStateByString(String newState)
    {
        switch (newState)
        { case "NotStarted":
            ChangeLevelState(LevelState.NotStarted);
                break;
            case "Setup":
            ChangeLevelState(LevelState.Setup);
                break;
            case "Running":
            ChangeLevelState(LevelState.Running);
                break;
            case "Fighting":
            ChangeLevelState(LevelState.Fighting);
                break;
            case "Finished":
            ChangeLevelState(LevelState.Finished);
                break;
        }
    }

    private void ChangeLevelState(LevelState newState)
    {
        _currentLevelState = newState;

        switch (newState)
        {
            case LevelState.NotStarted:
                ResetLevel();
                break;
            case LevelState.Setup:
                SetupLevel();
                break;
            case LevelState.Running:
                StartLevel();
                break;
            case LevelState.Fighting:
                StartFight();
                break;
            case LevelState.Finished:
                DisplayResults();
                break;
        }
    }

    public void LoadLevel(Level level)
    {
        _currentLevel = level;
        ChangeLevelState(LevelState.Setup);
    }

    private void SetupLevel()
    {
        ClearAllLevels();
        CreateLevel(_currentLevel);

        _knight.SetActive(true);
        _knight.transform.position = GameObject.Find("PlayerSpawn").transform.position;
        _knight.GetComponent<CharacterManager>().enabled = false;

        foreach (GameObject enemy in GetEnemies())
        {
            enemy.SetActive(true);
            enemy.GetComponent<CharacterManager>().ResetHealth();
        }

        if (GameCamera != null) GameCamera.StartCameraFocus();
        GameManager.UiManager.DisplayControlsInGame();
        GameManager.UiManager.DisplayIntroTimer();
    }

    private void StartLevel()
    {
        GameManager.UiManager.DisplayControlsInGame();
    }

    public void StartFight()
    {
        GameManager.QuestionManager.SetEnemy(_enemy);

        _knight.GetComponent<KnightManager>().SetUpFightScene();
        _knight.GetComponent<CharacterManager>().SetUpFightScene(false);
        _enemy.GetComponent<CharacterManager>().SetUpFightScene(true);
    }

    public void EndFight()
    {
        _enemy.SetActive(false);

        if (_enemy.name == "Ennemi_Boss")
        {
            ChangeLevelState(LevelState.Finished);
            DisplayResults();
        }
        else
        {
            GameManager.QuestionManager.SetEnemy(null);
            SetEnemy(null);
            _knight.GetComponent<KnightManager>().EndUpFightScene();
            _knight.GetComponent<CharacterManager>().EndUpFightScene(false);
            GameManager.UiManager.DisplayControlsInGame();
            GameManager.UiManager.DisplayIntroTimer();
        }
    }

    public void DisplayResults()
    {
        _resultsText.text = _knight.GetComponent<CharacterManager>().IsDead() ? "Perdu" : "Gagné";
        GameManager.UiManager.DisplayResults();

        _nextResults.gameObject.SetActive(_levels.IndexOf(_currentLevel) < _levels.Count - 1 && _resultsText.text != "Perdu");
        _reloadResults.gameObject.SetActive(_resultsText.text == "Perdu");
    }

    public void ReloadLevel()
    {
        ChangeLevelState(LevelState.Setup);
    }

    public void LoadNextLevel()
    {
        int nextLevelIndex = (_levels.IndexOf(_currentLevel) + 1) % _levels.Count;
        LoadLevel(_levels[nextLevelIndex]);
    }

    private void ResetLevel()
    {
        _knight.SetActive(false);
        _knight.GetComponent<CharacterManager>().ResetCharacterState();
        GameManager.QuestionManager.Reset();
        ClearAllLevels();
    }

    public void StopLevel()
    {
        if (!IsCurrentLevelState("NotStarted"))
        {
            ChangeLevelState(LevelState.NotStarted);
            ResetLevel();
        }
    }

    private void ClearAllLevels()
    {
        foreach (Transform child in _levelParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateLevel(Level level)
    {
        GameObject lvl = Instantiate(level.LevelPrefab, _levelParent);
        lvl.name = level.name;
    }

    private List<GameObject> GetEnemies()
    {
        return GameObject.Find("Enemies").transform.Cast<Transform>().Select(child => child.gameObject).ToList();
    }

    public void SetEnemy(GameObject enemy)
    {
        _enemy = enemy;
    }

    private void Update() 
    {
    	if (_knight.transform.GetComponent<CharacterManager>().IsDead())
    	{
    		DisplayResults();
    	}
    }
}