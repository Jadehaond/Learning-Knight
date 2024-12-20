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
    [SerializeField] private GameObject menuContainer; // Conteneur des boutons
    [SerializeField] private List<GameObject> objectPrefabs; // Liste des prefabs dans l'ordre de l'énum OBJETS
    [SerializeField] private List<Sprite> objectIcons; // Liste des icônes associées aux OBJETS

    private Dictionary<OBJETS, GameObject> objectPrefabMap = new Dictionary<OBJETS, GameObject>();
    private Dictionary<OBJETS, Sprite> objectIconMap = new Dictionary<OBJETS, Sprite>();
    private List<OBJETS> activeObjects = new List<OBJETS>(); // Liste des objets actifs
    private List<Image> parts = new List<Image>(); // Images des parts circulaires

    public enum OBJETS { SOUDOYER, PLUME, CHRONO, ENVELOPPE, DE, QUITTEOUDOUBLE }

    [SerializeField] private Timer _timer;

    private void Start()
    {
        InitializeObjectMappings();
        PopulateMenu();
        UpdateCamembert();
    }

    private void InitializeObjectMappings()
    {
        // Associer les prefabs aux OBJETS
        for (int i = 0; i < objectPrefabs.Count && i < System.Enum.GetValues(typeof(OBJETS)).Length; i++)
        {
            objectPrefabMap[(OBJETS)i] = objectPrefabs[i];
        }

        // Associer les icônes aux OBJETS
        for (int i = 0; i < objectIcons.Count && i < System.Enum.GetValues(typeof(OBJETS)).Length; i++)
        {
            objectIconMap[(OBJETS)i] = objectIcons[i];
        }
    }

    private void PopulateMenu()
    {
        // Supprimer les anciens boutons
        foreach (Transform child in menuContainer.transform)
        {
            Destroy(child.gameObject);
        }

        // Créer un bouton pour chaque objet actif
        foreach (OBJETS objet in activeObjects)
        {
            if (objectPrefabMap.TryGetValue(objet, out GameObject prefab))
            {
                GameObject button = Instantiate(prefab, menuContainer.transform);
                ConfigureButton(button, objet);
            }
        }
    }

    private void ConfigureButton(GameObject button, OBJETS objet)
    {
        // Ajouter un texte descriptif
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = objet.ToString();
        }

        // Ajouter une icône
        Image icon = button.transform.Find("Icon").GetComponent<Image>();
        if (icon != null && objectIconMap.TryGetValue(objet, out Sprite sprite))
        {
            icon.sprite = sprite;
        }

        // Assigner une action au bouton
        Button btnComponent = button.GetComponent<Button>();
        if (btnComponent != null)
        {
            btnComponent.onClick.AddListener(() => UseObject(objet));
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

        for (int i = 0; i < elementCount; i++)
        {
            OBJETS objet = activeObjects[i];

            // Créer une Image pour chaque part
            GameObject partObject = new GameObject($"Part_{i}", typeof(RectTransform), typeof(Image));
            partObject.transform.SetParent(camembertContainer, false);

            // Configurer le RectTransform
            RectTransform partTransform = partObject.GetComponent<RectTransform>();
            partTransform.sizeDelta = camembertContainer.sizeDelta; // Même taille que le conteneur
            partTransform.localRotation = Quaternion.Euler(0, 0, -anglePerPart * i);

            // Configurer l'Image
            Image partImage = partObject.GetComponent<Image>();
            partImage.type = Image.Type.Filled;
            partImage.fillMethod = Image.FillMethod.Radial360;
            partImage.fillAmount = anglePerPart / 360f;

            // Ajouter une icône si disponible
            if (objectIconMap.TryGetValue(objet, out Sprite icon))
            {
                partImage.sprite = icon;
                partImage.color = Color.white; // Ajuster pour afficher correctement l'icône
            }
            else
            {
                partImage.color = Random.ColorHSV(); // Couleur aléatoire si aucune icône
            }

            parts.Add(partImage);
        }
    }

    public void AddObject(OBJETS objet)
    {
        if (!activeObjects.Contains(objet))
        {
            activeObjects.Add(objet);
            PopulateMenu();
            UpdateCamembert();
        }
    }

    public void RemoveObject(OBJETS objet)
    {
        if (activeObjects.Contains(objet))
        {
            activeObjects.Remove(objet);
            PopulateMenu();
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
                //runPiece();
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
            case OBJETS.ENVELOPPE:
                Debug.Log("Indice affiché avec Enveloppe.");
                break;
            case OBJETS.DE:
                Debug.Log("Question changée avec Dé.");
                runDe();
                break;
            case OBJETS.QUITTEOUDOUBLE:
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
    
    public int[] runPlume()
    {
        int vraie = GameManager.QuestionManager.CorrectAnswerIndex;
        int[] percentages = new int[4];
        int totalPercentage = 100;

        // Attribuer un pourcentage élevé à la bonne réponse
        percentages[vraie] = Range(40, 60); // Entre 40% et 60%
        totalPercentage -= percentages[vraie];

        // Répartir le reste entre les mauvaises réponses --> Faire des pourcentages dofférents entre les mauvaises réponses
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

        GameManager.QuestionManager.DisplayResponses();
        return percentages;
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

    /*public string runEnveloppe(Question currentQuestion)
    {
        // --> public string hint; Comment fournir l'indice ?? ajouter ceci dans le fichier excel qui sera lu par la suite puis stocké dansune varaible pendant la question et renouvelé a chaque réponse ?
        if (nb > 0)
        {
            removeObject();
            return currentQuestion.hint; // Retourne l'indice
        }
        return "";
    }*/

    public void runChrono()
    {
        _timer.AddTimer(3f);
    }

    /*public void runQuitteOuDouble(bool correctAnswer, Player player)
    {
        if (nb > 0)
        {
            if (correctAnswer)
            {
                //player.DealCriticalDamage();
                Debug.Log("Coup critique infligé !");
            }
            else
            {
                //player.ReduceDefense(10); // Réduction de défense
                Debug.Log("Perte de défense !");
            }
        }
    }*/

    /*  ????????????????????????
        Dans fichier KnightManager :
        public int attackPower;
        public int defense;

        public void DealCriticalDamage()
        {
            int criticalDamage = attackPower * 2;
            Debug.Log("Dégâts critiques : " + criticalDamage);
        }

        public void ReduceDefense(int amount)
        {
            defense -= amount;
            Debug.Log("Défense réduite de : " + amount);
        }
    */
  
}
