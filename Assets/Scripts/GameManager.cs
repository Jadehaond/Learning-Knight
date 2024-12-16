using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private LevelManager _levelManager;
    public LevelManager LevelManager => _levelManager;
    [SerializeField] private UIManager _uiManager;
    public UIManager UiManager => _uiManager;
    [SerializeField] private PauseManager _pauseManager;
    public PauseManager PauseManager => _pauseManager;
    [SerializeField] private QuestionManager _questionManager;
    public QuestionManager QuestionManager => _questionManager;
	
    void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning("A second instance of GameManager was destroyed!");
            return; // Quitter la méthode si l'instance est détruite
        }
        
        Instance = this; // Initialise l'instance
        DontDestroyOnLoad(gameObject); // Ne pas détruire cet objet lors du changement de scène

        // Vérifiez si tous les composants sont assignés
        if (_levelManager == null || _uiManager == null || _pauseManager == null || _questionManager == null)
        {
            Debug.LogError("Some components in GameManager are not assigned!");
        }
    }

    void OnDestroy()
    {
        Debug.Log("GameManager destroyed");
    }
}
