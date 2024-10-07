using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    private float angle;

    void Start()
    {
        angle = 0.0f;
    }

    void Update()
    {
	if (angle <= 180.0f)
	{
      var eulerAngles = this.gameObject.transform.eulerAngles;
	this.gameObject.transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, -angle);
	angle = angle+0.05f;    	
	}

    }

}
