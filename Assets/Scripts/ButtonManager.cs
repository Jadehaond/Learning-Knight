using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
	private Button Fr;
	private Button Maths;
	
	void Awake()
	{
		Fr = GameObject.Find("Btn_French").GetComponent<Button>();
		Maths = GameObject.Find("Btn_Maths").GetComponent<Button>();
	}
	
	public void SectionManager(string name)
	{
		
		Maths.GetComponent<Image>().color = new Color(1.2f, 1.2f, 1.2f, 1.0f);
		Fr.GetComponent<Image>().color = new Color(1.2f, 1.2f, 1.2f, 1.0f);
		
		if (name == "French")
		{
			Fr.GetComponent<Image>().color = new Color(1.2f, 1.2f, 1.2f, 0.6862745f);
		}
		else
		{
			Maths.GetComponent<Image>().color = new Color(1.2f, 1.2f, 1.2f, 0.6862745f);
		}
		
		
	}   
}
