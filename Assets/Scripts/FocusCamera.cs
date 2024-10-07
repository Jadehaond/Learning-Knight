using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusCamera : MonoBehaviour
{
    [SerializeField] private GameObject _knight; // Chevalier à suivre par la caméra
    [SerializeField] private Vector3 _offset = new Vector3(15f, 7f, -15f); // Décalage par rapport à la position du chevalier
    private float deadZoneY = -50f; // Limite basse de la position Y, en-dessous de laquelle le joueur est considéré comme mort

    private GameManager _instance; // Instance du GameManager pour accéder à ses fonctions et variables
    private GameManager GameManager => _instance ??= GameManager.Instance;

    private void Start()
    {
        StartCameraFocus();
    }

    private void Update()
    {
        // Si le niveau est en cours d'exécution mais n'est ni en mode "Setup" ni en mode "Fight"
        if (IsCameraFollowAllowed())
        {
            DeadByBorder(); // Vérifie si le chevalier sort de la zone autorisée
            UpdateCameraPosition(); // Met à jour la position de la caméra
        }
    }

    /// <summary>
    /// Démarre le suivi de la caméra sur le chevalier.
    /// </summary>
    public void StartCameraFocus()
    {
        UpdateCameraPosition();
    }

    /// <summary>
    /// Met à jour la position de la caméra en fonction de la position du chevalier et de l'offset.
    /// </summary>
    private void UpdateCameraPosition()
    {
        transform.position = _knight.transform.position + _offset;
    }

    /// <summary>
    /// Vérifie si l'état actuel permet le suivi de la caméra.
    /// </summary>
    /// <returns>True si la caméra doit suivre, False sinon.</returns>
    private bool IsCameraFollowAllowed()
    {
        return GameManager.LevelManager.IsCurrentLevelState("Running") &&
               !GameManager.LevelManager.IsCurrentLevelState("Setup") &&
               !GameManager.LevelManager.IsCurrentLevelState("Fight");
    }

    /// <summary>
    /// Vérifie si le chevalier est sorti des limites autorisées et affiche les résultats si c'est le cas.
    /// </summary>
    private void DeadByBorder()
    {
        // Si le chevalier est trop à gauche de la caméra ou s'il est tombé en-dessous de la deadZoneY
        if (_knight.transform.position.x < transform.position.x - 25f || _knight.transform.position.y < deadZoneY)
        {
            GameManager.LevelManager.DisplayResults(); // Affiche les résultats de fin de partie
        }
    }
}
