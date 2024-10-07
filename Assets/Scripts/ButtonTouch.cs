using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ButtonTouch :  MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public void OnPointerDown(PointerEventData eventData)
	{
      	this.gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
      	this.gameObject.GetComponent<Image>().color = new Color(1.2f, 1.2f, 1.2f, 0.6862745f);
	}
 
	public void OnPointerUp(PointerEventData eventData)
	{
    	this.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
      	this.gameObject.GetComponent<Image>().color = new Color(1.2f, 1.2f, 1.2f, 1.0f);
	}

}
