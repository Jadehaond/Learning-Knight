using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.Random;
using TMPro;

public class ObjectManager : MonoBehaviour
{
    private GameManager _instance;
    private GameManager GameManager => _instance ??= GameManager.Instance;

    [SerializeField] private RectTransform camembertContainer; // Conteneur circulaire
    [SerializeField] private GameObject menuContainer; // Conteneur des boutons (si nécessaire)
    [SerializeField] private List<Sprite> objectIcons; // Liste des icônes associées aux OBJETS

    private Dictionary<OBJETS, Sprite> objectIconMap = new Dictionary<OBJETS, Sprite>();
    private List<OBJETS> activeObjects = new List<OBJETS>(); // Liste des objets actifs
    private List<GameObject> parts = new List<GameObject>(); // Images des parts circulaires

    public enum OBJETS { SOUDOYER, PLUME, CHRONO, DE, QUITTEOUDOUBLE }
    [SerializeField] private Timer _timer;

    private void Start()
    {
        
        activeObjects.Add(OBJETS.SOUDOYER);
        activeObjects.Add(OBJETS.PLUME);
        activeObjects.Add(OBJETS.CHRONO);
        activeObjects.Add(OBJETS.DE);
        activeObjects.Add(OBJETS.QUITTEOUDOUBLE);

        InitializeObjectMappings();
        UpdateCamembert();
    }

    private void InitializeObjectMappings()
    {
        // Associer les icônes aux OBJETS
        for (int i = 0; i < objectIcons.Count && i < System.Enum.GetValues(typeof(OBJETS)).Length; i++)
        {
            objectIconMap[(OBJETS)i] = objectIcons[i];
        }
    }

    private void UpdateCamembert()
{
    // Supprimer les parts existantes
    foreach (var part in parts)
    {
        Destroy(part.gameObject);
    }

    parts.Clear();

    int elementCount = activeObjects.Count;
    if (elementCount == 0) return;

    // Calculer l'angle de chaque part
    float anglePerPart = 360f / elementCount;
    float radius = camembertContainer.rect.width / 2f;  // Rayon du cercle en fonction de la taille du conteneur

    for (int i = 0; i < elementCount; i++)
    {
        OBJETS objet = activeObjects[i];

        // Créer un nouveau GameObject pour chaque bouton
        GameObject button = new GameObject($"Button_{objet}", typeof(RectTransform), typeof(Button));
        button.transform.SetParent(camembertContainer, false);  // Parenté au conteneur principal

        // Configurer le RectTransform du bouton
        RectTransform buttonTransform = button.GetComponent<RectTransform>();
        buttonTransform.sizeDelta = new Vector2(50f, 50f);  // Taille du bouton (ajustez selon vos besoins)

        // Calculer la position du bouton sur la bordure du cercle (coordonnées polaires -> cartésiennes)
        float angleInRadians = Mathf.Deg2Rad * (anglePerPart * i); // Conversion de l'angle en radians
        float xPos = Mathf.Cos(angleInRadians) * radius; // Position en x
        float yPos = Mathf.Sin(angleInRadians) * radius; // Position en y
        buttonTransform.localPosition = new Vector3(xPos, yPos, 0);

        // Centrer le pivot du bouton sur lui-même
        buttonTransform.anchorMin = new Vector2(0.5f, 0.5f);
        buttonTransform.anchorMax = new Vector2(0.5f, 0.5f);
        buttonTransform.pivot = new Vector2(0.5f, 0.5f);

        // Créer un GameObject séparé pour l'image
        GameObject imageObject = new GameObject("Image", typeof(Image));
        imageObject.transform.SetParent(button.transform, false);  // Faire de l'image un enfant du bouton

        // Configurer le RectTransform de l'image
        RectTransform imageTransform = imageObject.GetComponent<RectTransform>();
        imageTransform.sizeDelta = new Vector2(50f, 50f); // Taille de l'image (ajustez selon vos besoins)
        imageTransform.anchorMin = new Vector2(0.5f, 0.5f); // Centrer l'image dans le bouton
        imageTransform.anchorMax = new Vector2(0.5f, 0.5f);
        imageTransform.pivot = new Vector2(0.5f, 0.5f);

        // Ajouter l'icône à l'image
        Image buttonImage = imageObject.GetComponent<Image>();
        buttonImage.sprite = objectIconMap[objet]; // Utiliser l'icône de l'objet
        buttonImage.preserveAspect = true; // Garder les proportions de l'image

        // Ajouter un Mask pour délimiter la zone cliquable
        Mask mask = button.AddComponent<Mask>();
        mask.showMaskGraphic = false;  // Masquer le graphique du mask pour ne garder que la zone de clic

        // Ajouter l'action du bouton
        Button buttonComponent = button.GetComponent<Button>();
        buttonComponent.onClick.AddListener(() => UseObject(objet)); // Action lors du clic

        // Ajouter le bouton à la liste des parts
        parts.Add(button);
    }
}

    public void AddObject(OBJETS objet)
    {
        if (!activeObjects.Contains(objet))
        {
            activeObjects.Add(objet);
            UpdateCamembert();
        }
    }

    public void RemoveObject(OBJETS objet)
    {
        if (activeObjects.Contains(objet))
        {
            activeObjects.Remove(objet);
            UpdateCamembert();
        }
    }

    private void UseObject(OBJETS objet)
    {
        if (!activeObjects.Contains(objet)) 
        {
            Debug.LogWarning($"Tentative d'utilisation d'un objet non disponible : {objet}");
            return;
        }

        switch (objet)
        {
            case OBJETS.SOUDOYER:
                runPiece();
                Debug.Log("Utilisation de l'objet Soudoie.");
                break;
            case OBJETS.PLUME:
                runPlume();
                Debug.Log("Utilisation de l'objet Plume.");
                break;
            case OBJETS.CHRONO:
                Debug.Log("Ajout de temps avec Chrono.");
                runChrono();
                break;
            case OBJETS.DE:
                Debug.Log("Question changée avec Dé.");
                runDe();
                break;
            case OBJETS.QUITTEOUDOUBLE:
                runQuitteOuDouble();
                Debug.Log("Quitte ou double activé.");
                break;
        }
        RemoveObject(objet);
    }
    
    public void addObject(OBJETS objet)
    {
    	activeObjects.Add(objet);
    }
    
    public void removeObject(OBJETS objet)
    {
    	activeObjects.Remove(objet);
    }
    
    public void runPlume()
    {
        int vraie = GameManager.QuestionManager.CorrectAnswerIndex;
        int[] percentages = new int[4];
        int totalPercentage = 100;

        // Attribuer un pourcentage élevé à la bonne réponse
        percentages[vraie] = Range(40, 60); // Entre 40% et 60%
        totalPercentage -= percentages[vraie];

        // Répartir le reste entre les mauvaises réponses --> Faire des pourcentages différents entre les mauvaises réponses
        for (int i = 0; i < percentages.Length; i++)
        {
            if (i != vraie)
            {
                percentages[i] = Range(10, totalPercentage / 2);
                totalPercentage -= percentages[i];
            }
        }

        // La somme est égale à 100
        percentages[Range(0, 4)] += totalPercentage;

        // Mettre à jour l'affichage des réponses avec les pourcentages
        string[] responses = GameManager.QuestionManager.Reponses;
       
        for (int i = 0; i < responses.Length; i++)
        {
            // Ajouter le pourcentage entre parenthèses à chaque réponse
            if (responses[i] != null) // Vérifier si le champ de texte est non nul
            {
                responses[i] = $"{responses[i]} ({percentages[i]}%)"; // Ajouter le pourcentage à la réponse
            }
        }

        // Afficher les réponses dans l'interface
        GameManager.QuestionManager.DisplayResponses();
    }
    
    public string[] runPiece()
    {
        string[] Reponses = GameManager.QuestionManager.Reponses;
        int vraie = GameManager.QuestionManager.CorrectAnswerIndex;
        List<int> indicesFaux = new List<int>();

        for (int i = 0; i < Reponses.Length; i++)
        {
            if (i != vraie && !string.IsNullOrEmpty(Reponses[i]))
                indicesFaux.Add(i);
        }

        // Supprimer deux mauvaises réponses au hasard
        for (int i = 0; i < 2 && indicesFaux.Count > 0; i++)
        {
            int indexToRemove = indicesFaux[Range(0, indicesFaux.Count)];
            Reponses[indexToRemove] = "";
            indicesFaux.Remove(indexToRemove);
        }

        GameManager.QuestionManager.DisplayResponses();
        return Reponses;
    }

    public void runDe()
    {
        GameManager.QuestionManager.LoadNextQuestion();
    }

    public void runChrono()
    {
        _timer.AddTimer(3f);
    }

    public void runQuitteOuDouble()
    {
        GameManager.QuestionManager.HandleCriticalAnswer();
    }
}
