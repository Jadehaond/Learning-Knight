using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Random;
using UnityEngine.UI;
using TMPro;

/**
Associer les prefabs :
    Ajoutez vos prefabs bouton dans la liste objectPrefabs dans l'inspecteur Unity.
    L'ordre des prefabs doit correspondre à l'ordre de l'énumération OBJETS. Par exemple :
    Index 0 → SOUDEYER
    Index 1 → PLUME
    Index 2 → CHRONO
*/

public class ObjectManager : MonoBehaviour
{
    private Dictionary<OBJETS, string> objetDescriptions = new Dictionary<OBJETS, string>
    {
        { OBJETS.SOUDOYER, "Soudoie pour obtenir un avantage." },
        { OBJETS.PLUME, "Affiche des pourcentages simulés." },
        { OBJETS.CHRONO, "Ajoute du temps au chronomètre." },
        { OBJETS.ENVELOPPE, "Fournit un indice pour la question." },
        { OBJETS.DE, "Change la question actuelle." },
        { OBJETS.QUITTEOUDOUBLE, "Double ou rien pour le joueur." },
        { OBJETS.IMMUNITE, "Active une immunité temporaire." }
    };
    
    private GameManager _instance;
    private GameManager GameManager => _instance ??= GameManager.Instance;

    public enum OBJETS { SOUDOYER, PLUME, CHRONO, ENVELOPPE, DE, QUITTEOUDOUBLE, IMMUNITE }
    private List<OBJETS> _objet = new List<OBJETS>();
    private bool isImmune = false;
    [SerializeField] private Timer _timer;
    [SerializeField] private GameObject menuContainer; // Conteneur du menu (Panel ou Scroll View Content)
    [SerializeField] private List<GameObject> objectPrefabs; // Liste des prefabs (dans l'ordre des OBJETS)
    private Dictionary<OBJETS, GameObject> objectPrefabMap = new Dictionary<OBJETS, GameObject>();
    private Dictionary<OBJETS, Sprite> objectIconMap;
    [SerializeField] private List<Sprite> objectIcons;
   
    private void Start()
    {
        objectIconMap = new Dictionary<OBJETS, Sprite>();
        for (int i = 0; i < objectIcons.Count && i < System.Enum.GetValues(typeof(OBJETS)).Length; i++)
        {
            objectIconMap[(OBJETS)i] = objectIcons[i];
        }

        PopulateMenu();
    }     

    private void PopulateMenu()
    {
        foreach (Transform child in menuContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (OBJETS objet in _objet)
        {
            if (objectPrefabMap.ContainsKey(objet))
            {
                GameObject button = Instantiate(objectPrefabMap[objet], menuContainer.transform);
                button.GetComponent<Button>().onClick.AddListener(() => UseObject(objet));
            }
            else
            {
                Debug.LogWarning($"Prefab non défini pour l'objet : {objet}");
            }
        }
    }

    private void ConfigurePrefab(GameObject button, OBJETS objet)
    {
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = objet.ToString();
        }

        Image icon = button.transform.Find("Icon").GetComponent<Image>();
        if (icon != null)
        {
            // Assignez une icône spécifique
            icon.sprite = GetIconForObject(objet);
        }
    }

    private Sprite GetIconForObject(OBJETS objet)
    {
        return objectIconMap.TryGetValue(objet, out Sprite icon) ? icon : null;
    }

    private void UseObject(OBJETS objet)
    {
        if (!_objet.Contains(objet)) 
        {
            Debug.LogWarning($"Tentative d'utilisation d'un objet non disponible : {objet}");
            return;
        }

        switch (objet)
        {
            case OBJETS.SOUDOYER:
                Debug.Log("Utilisation de l'objet Soudoie.");
                break;
            case OBJETS.PLUME:
                Debug.Log("Utilisation de l'objet Plume.");
                break;
            case OBJETS.CHRONO:
                Debug.Log("Ajout de temps avec Chrono.");
                runChrono(_timer);
                break;
            case OBJETS.ENVELOPPE:
                Debug.Log("Indice affiché avec Enveloppe.");
                break;
            case OBJETS.DE:
                Debug.Log("Question changée avec Dé.");
                runDe(GameManager.QuestionManager);
                break;
            case OBJETS.QUITTEOUDOUBLE:
                Debug.Log("Quitte ou double activé.");
                break;
            case OBJETS.IMMUNITE:
                Debug.Log("Immunité activée.");
                runImmunite();
                break;
        }
    }
    
    public void addObject(OBJETS objet)
    {
    	_objet.Add(objet);
    }
    
    public void removeObject(OBJETS objet)
    {
    	_objet.Remove(objet);
    }
    
    public int[] runPlume(int vraie)
    {
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

        removeObject(OBJETS.PLUME);
        GameManager.QuestionManager.DisplayResponses();
        return percentages;
    }
    
    public string[] runPiece(string[] Reponses, int vraie)
    {
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

        removeObject(OBJETS.SOUDOYER);
        GameManager.QuestionManager.DisplayResponses();
        return Reponses;
    }

    public void runDe(QuestionManager questionManager)
    {
        GameManager.QuestionManager.LoadNextQuestion();
        removeObject(OBJETS.DE);
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

    public void runImmunite()
    {
        isImmune = true;
        removeObject(OBJETS.IMMUNITE);
        StartCoroutine(RemoveImmunityAfterTime(10f)); // Immunité pendant 10 secondes
    }

    private IEnumerator RemoveImmunityAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isImmune = false;
    }

    public void runChrono(Timer timer)
    {
        _timer.AddTimer(3f);
        removeObject(OBJETS.CHRONO);
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

            removeObject();
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
