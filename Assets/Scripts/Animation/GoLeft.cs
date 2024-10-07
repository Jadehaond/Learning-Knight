using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoLeft : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _distance;
    private float _startX;
        
    // Start is called before the first frame update
    void Start()
    {
    	_speed = 2.5f;
    	_distance = 15.5f;
    	_startX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
       if (transform.position.x < _startX+_distance)
       {
       		transform.Translate(Vector3.left * (Time.deltaTime * _speed));
       }
    }
}
