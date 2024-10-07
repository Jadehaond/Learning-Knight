using System.Collections;
using UnityEngine;

// Base class for all pausable behaviours
public class PausableBehaviour : MonoBehaviour
{
    // You can add methods or properties here that 
    // should be common to all pausable behaviors
}

public class PauseManager : MonoBehaviour
{
    private PausableBehaviour[] _pausableBehaviours;

    private bool _isPaused;
    public bool IsPaused => _isPaused;

    private void Awake()
    {                         
        _isPaused = false;  
        _pausableBehaviours = FindObjectsByType<PausableBehaviour>(FindObjectsSortMode.None);
        Debug.Log($"Found {_pausableBehaviours.Length} pauseable behaviours");
    }

    /// <summary>
    /// Pauses or unpauses the game based on the input parameter.
    /// </summary>
    /// <param name="pause">True to pause the game; false to resume.</param>
    public void Pause(bool pause)
    {
        _isPaused = pause;
        if (pause) 
            PauseGame();
        else 
            PlayGame();
    }

    private void PauseGame()
    {
        foreach (PausableBehaviour behaviour in _pausableBehaviours)
        {
            behaviour.StopAllCoroutines(); // Stop any running coroutines
            behaviour.enabled = false;      // Disable the behaviour
        }
        
        Time.timeScale = 0f; // Freeze the game time
    }

    private void PlayGame()
    {
        foreach (PausableBehaviour behaviour in _pausableBehaviours)
        {
            // Enable the behaviour only if it's currently disabled
            if (!behaviour.enabled)
            {
                behaviour.enabled = true; 
            }
        }
        
        Time.timeScale = 1f; // Resume the game time
    }
}