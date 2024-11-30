using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterManager : MonoBehaviour
{
    #region Variables
    private GameManager _instance;
    private GameManager GameManager => _instance ??= GameManager.Instance;

    [Header("Settings")]
    [SerializeField] private LayerMask _groundLayerMask;

    [Header("State Variables")]
    //private Transform _healthpoints = null;
    private Vector3 _savedPositionNonFightable;
    [SerializeField] private GameObject _healthObject = null;
    [SerializeField] private Slider _healthBar = null;
    [SerializeField] private float _value = 10;
    public float Value => _value;
    [SerializeField] private float _maxHealth = 10;
    public float MaxHealth => _maxHealth;
    private Camera _gameCamera;
    #endregion

     void Start()
    {
        _value = _maxHealth;
        if (_healthBar != null) _healthBar.value = _maxHealth;
    }

    public void SetHealthBar()
    {
        _value = 10;
        _healthObject = GameObject.Find("EnemyHealth");
        _healthBar = GameObject.Find("Enemy_Health_bar_slider").GetComponent<Slider>();
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

    public void ResetCharacterState()
    {
        GainLife(_maxHealth);
    }    

    public void SetUpFightScene(bool IsEnemy)
    {
        _gameCamera = GameObject.Find("GameCamera").GetComponent<Camera>();

        if (_gameCamera != null)
        {
            _savedPositionNonFightable = this.transform.position;
            this.transform.GetComponent<Rigidbody2D>().gravityScale = 0;
            this.transform.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
            this.transform.GetComponent<Rigidbody2D>().constraints |= RigidbodyConstraints2D.FreezeRotation;            
           
            Vector3 center;
            if (IsEnemy) {
                SetHealthBar();
                center = GetCenterOfCameraPart(0.5f, 1f);     
                StartCoroutine(MoveObject(this.transform.position, center + new Vector3(-1f, 0f, 0f), 2f));
            }
            else 
            {
                center = GetCenterOfCameraPart(0f, 0.5f);
                StartCoroutine(MoveObject(this.transform.position, center + new Vector3(1f, -4f, 0f), 2f));
            }
            StartCoroutine(RotateObject(this.transform.eulerAngles, center + new Vector3(0f, 0f, 0f), 2f));      
        }
        StartCoroutine(StartSetUpFightTimer(3f, true));
    }
    
    private Vector3 GetCenterOfCameraPart(float startX, float endX)
    {
        if (_gameCamera != null)
        {
            Vector3 bottomLeft = _gameCamera.ViewportToWorldPoint(new Vector3(startX, 0f, _gameCamera.nearClipPlane));
            Vector3 topRight = _gameCamera.ViewportToWorldPoint(new Vector3(endX, 1f, _gameCamera.nearClipPlane));
            return (bottomLeft + topRight) * 0.5f;
        }
        return this.transform.position;
    }

    public void EndUpFightScene(bool IsEnemy)
    { 
        if (_gameCamera != null)
        {
            Vector3 center;
            if (IsEnemy) {
                center = GetCenterOfCameraPart(0.5f, 1f);
            }
            else center = GetCenterOfCameraPart(0f, 0.5f);
    
            StartCoroutine(MoveObject(center + new Vector3(-1f, -4f, 0f), FindClosestGroundPoint(), 2f));
            this.transform.GetComponent<Rigidbody2D>().gravityScale = 50;
            this.transform.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            this.transform.GetComponent<Rigidbody2D>().constraints |= RigidbodyConstraints2D.FreezeRotation;
           
        }
        StartCoroutine(StartSetUpFightTimer(3f, true));
    }

    private IEnumerator RotateObject(Vector3 startRotation, Vector3 targetRotation, float duration)
    {
        if (startRotation == targetRotation) {
            float timeElapsed = 0f;

            while (timeElapsed < duration)
            {
                transform.eulerAngles = Vector3.Lerp(startRotation, targetRotation, timeElapsed / duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            transform.eulerAngles = targetRotation;
        }
        yield return null;
    }

    private IEnumerator MoveObject(Vector3 start, Vector3 end, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            this.transform.position = Vector2.Lerp(start, end, elapsedTime / 2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        this.transform.position = end;
    }

    private Vector3 FindClosestGroundPoint()
    {
        RaycastHit2D hit = Physics2D.Raycast(_savedPositionNonFightable, Vector2.down, Mathf.Infinity, _groundLayerMask);
        if (hit.collider != null)
        {
            return hit.point;
        }
        return _savedPositionNonFightable;
    }

    private IEnumerator StartSetUpFightTimer(float countdownTime, bool isfight)
    {
        yield return new WaitForSeconds(countdownTime);
    }

}
