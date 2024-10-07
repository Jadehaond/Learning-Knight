using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Levels", menuName = "ScriptableObjects/Levels", order = 1)]
public class Level : ScriptableObject 
{
    [Header("Level Information")]
    [SerializeField] private string _name;
    public string Name => _name;

    [TextArea]
    [SerializeField] private string _description;
    public string Description => _description;

    [Header("Level Configuration")]
    [SerializeField] private GameObject _levelPrefab;
    public GameObject LevelPrefab => _levelPrefab;

    [SerializeField] private List<GameObject> _ennemies = new List<GameObject>();
    public List<GameObject> Ennemies => _ennemies;

    //[SerializeField] private Vector3 _playerSpawn = new Vector3(0f, 0f, 0f);
    //public Vector3 PlayerSpawn => _playerSpawn;

    [SerializeField] private List<Vector3> _checkpoints = new List<Vector3>();
    public List<Vector3> Checkpoints => _checkpoints;

    [Header("Objectives")]
    [SerializeField] private int _targetScore;
    public int TargetScore => _targetScore;

    [SerializeField] private float _timeLimit = 0f; // Temps limité en secondes (0 si illimité)
    public float TimeLimit => _timeLimit;

    [SerializeField] private bool _mustSurvive = false; // Le joueur doit survivre un certain temps
    public bool MustSurvive => _mustSurvive;

    [SerializeField] private List<string> _secondaryObjectives = new List<string>();
    public List<string> SecondaryObjectives => _secondaryObjectives;

    [Header("Rewards")]
    [SerializeField] private int _rewardCoins;
    public int RewardCoins => _rewardCoins;

    [SerializeField] private List<string> _rewardItems = new List<string>();
    public List<string> RewardItems => _rewardItems;

    [Header("Music and Sound")]
    [SerializeField] private AudioClip _backgroundMusic;
    public AudioClip BackgroundMusic => _backgroundMusic;

    public enum Difficulty { Easy, Medium, Hard }
    [SerializeField] private Difficulty _difficultyLevel;
    public Difficulty LevelDifficulty => _difficultyLevel;

    // Condition de réussite de niveau
    public bool IsLevelDone
    {
        get
        {
            return PlayerPrefs.GetInt(_name + ".isDone", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(_name + ".isDone", value ? 1 : 0);
        }
    }
}
