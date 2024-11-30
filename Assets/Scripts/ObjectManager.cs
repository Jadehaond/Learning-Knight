using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Random;
using UnityEngine.UI;
using TMPro;

public class ObjectManager : MonoBehaviour
{
    /*
    Plume : Affiche des pourcentages simulés sur les réponses.
    Pièce : Supprime deux mauvaises réponses.
    Dé : Change la question.
    Enveloppe : Fournit un indice.
    Médaillon : Active une immunité temporaire.
    Chronomètre : Ajoute du temps au joueur.
    Quitte ou double : Applique un coup critique ou une pénalité.
    */

   private enum OBJETS { SOUDOYER, PLUME, CHRONO, ENVELOPPE, DE, QUITTEOUDOUBLE, IMMUNITE }
   private List<OBJETS> _objet = new List<OBJETS>();
   private List<int> _nombre = new List<int>();
        
   private int nb = 1;
   
   public GameObject objs;
   
   public int NbEnDeux {
        get
        {
            return PlayerPrefs.GetInt(".NbEnDeux");
        }
        set
        {
            PlayerPrefs.SetInt(".NbEnDeux", nb);
        }
    }
    
    void Awake()
    {
    	nb = 2;
    	objs = GameObject.Find("Btn_Objects");
    }
    
    void Update()
    {
    	objs.GetComponentInChildren<TextMeshProUGUI>().text = ""+nb;
    	if (nb <= 0) objs.GetComponent<Image>().color = new Color(0.6f,0.6f,0.6f,0.6862745f);
    }
        
    public void addObject()
    {
    	if (nb < 5) nb = nb+1;
    }
    
    public void removeObject()
    {
    	if (nb > 0) nb = nb-1;
    }

    public string[] run(string[] Reponses, int vraie)
    {
        //Enlève deux réponses fausses à l'affichage
    	if (nb > 0)
    	{
	    	int rand = Range(0,4);
	    	if (rand == vraie) rand = rand + 1;
	    	if (rand == 4) rand = 0;
	    	if (Reponses[rand] == "") rand = rand + 1;
	    	if (rand == 4) rand = 0;
	    	Reponses[rand] = "";
	    	
	    	rand = Range(0,4);
	    	if (rand == vraie) rand = rand + 1;
	    	if (rand == 4) rand = 0;
	    	if (Reponses[rand] == "") rand = rand + 1;
	    	if (rand == 4) rand = 0;
	    	Reponses[rand] = "";   
	    	
	    	removeObject(); 	
       }            	
       return Reponses;
    }

    public string[] runPiece(string[] Reponses, int vraie)
    {
        if (nb > 0)
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

            removeObject();
        }
        return Reponses;
    }

    public int[] runPlume(int vraie)
    {
        int[] percentages = new int[4];
        int totalPercentage = 100;

        // Attribuer un pourcentage élevé à la bonne réponse
        percentages[vraie] = Range(40, 60); // Entre 40% et 60%
        totalPercentage -= percentages[vraie];

        // Répartir le reste entre les mauvaises réponses
        for (int i = 0; i < percentages.Length; i++)
        {
            if (i != vraie)
            {
                percentages[i] = Range(10, totalPercentage / 2);
                totalPercentage -= percentages[i];
            }
        }

        // Assurez-vous que la somme est égale à 100
        percentages[Range(0, 4)] += totalPercentage;

        return percentages;
    }

    /*public void runDe(QuestionManager questionManager)
    {
        if (nb > 0)
        {
            questionManager.LoadNextQuestion();
            removeObject();
        }
    }*/

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

    private bool isImmune = false;

    public void runImmunite()
    {
        if (nb > 0)
        {
            isImmune = true;
            removeObject();
            StartCoroutine(RemoveImmunityAfterTime(10f)); // Immunité pendant 10 secondes
        }
    }

    private IEnumerator RemoveImmunityAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isImmune = false;
    }

    //Chnager la manière dont les dégats sont pris ?
    /* 
    public void TakeDamage(int damage)
    {
        if (isImmune)
        {
            Debug.Log("Dégâts bloqués !");
            return;
        }

        currentHealth -= damage;
    }
    */

    public void runChrono(Timer timer)
    {
        if (nb > 0)
        {
            //timer.AddTime(10); // Ajoute 10 secondes
            removeObject();
        }
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
