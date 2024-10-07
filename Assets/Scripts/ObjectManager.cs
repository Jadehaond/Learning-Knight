using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Random;
using UnityEngine.UI;
using TMPro;

public class ObjectManager : MonoBehaviour
{
        
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
    
    public string[] run(string[] Reponses, int vraie)
    {
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
    
    public void addObject()
    {
    	if (nb < 5) nb = nb+1;
    }
    
    public void removeObject()
    {
    	if (nb > 0) nb = nb-1;
    }
    
}
