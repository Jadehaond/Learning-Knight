using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitController : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }
    
    public void OnApplicationQuit()
    {
	    PlayerPrefs.SetString("QuitTime", "The application last closed at: " + System.DateTime.Now);
    }

    public void QuitGame()
    {
     	#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
