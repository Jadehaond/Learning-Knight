using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Health : MonoBehaviour
{
    [SerializeField] private GameObject _healthObject = null;
    [SerializeField] private Slider _healthBar = null;
    [SerializeField] private float _value = 10;
    public float Value => _value;
    [SerializeField] private float _maxHealth = 10;
    public float MaxHealth => _maxHealth;

    void Start()
    {
        _value = _maxHealth;
        if (_healthBar != null) _healthBar.value = _maxHealth;
    }

    public void SetEnemyHealthBar()
    {
        _value = 10;
        _healthObject = GameObject.Find("EnnemyHealth");
        _healthBar = GameObject.Find("Ennemi_Health_bar_slider").GetComponent<Slider>();
        _healthBar.value = _value;
    }

    public void DamageLife(float damage)
    {
        _value -= damage;
        if (_healthBar != null) _healthBar.value = _value;   
    }

    public void GainLife(float gain)
    {
        if(_value != _maxHealth)
        {
            if((_value + gain) >= _maxHealth)
            {
                _value = _maxHealth;
            } else 
            {
                _value += gain;
            }
        }
        if (_healthBar != null) _healthBar.value = _value;
    }

    public bool IsDead()
    {
        if (_value <= 0)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public void ResetHealth()
    {
        _value = _maxHealth;
        if (_healthBar != null) _healthBar.value = _maxHealth;

    }
}
