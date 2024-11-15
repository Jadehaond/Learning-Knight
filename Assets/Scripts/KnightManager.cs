using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KnightManager : MonoBehaviour
{
    #region Variables
    private static readonly Vector3 _standByPosition = new Vector3(175f, 75f, 0f);
    private static readonly Vector3 _standByRotation = new Vector3(0f, 0f, 0f);
    private const float _defaultSpeed = 0.1f;
    private const float _sprintMultiplier = 1.8f;
    private const float _jumpForce = 150f;

    private float _currentSpeed = 0f;
    private GameManager _instance;
    private GameManager GameManager => _instance ??= GameManager.Instance;

    [Header("Settings")]
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private TextMeshProUGUI _sprintCooldownText;
    [SerializeField] private TextMeshProUGUI _slideCooldownText;

    [Header("State Variables")]
    private BoxCollider2D _boxCollider;

    private Coroutine _activeCoroutine = null;
    private enum CharacterState { Running, Jumping, Sliding, Sprinting, Fighting }
    private CharacterState _currentCharacterState = CharacterState.Running;
    private Vector3 _characterNonFightPlace;
    #endregion

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        ///ResetCharacterState();
        SetSpeed(_defaultSpeed);
    }

    public void ResetCharacterState()
    {
        SetSpeed(_defaultSpeed);
        this.gameObject.transform.eulerAngles = _standByRotation;
        this.gameObject.transform.position = _standByPosition;
        this.gameObject.GetComponent<CharacterManager>().GainLife(this.gameObject.GetComponent<CharacterManager>().MaxHealth);
    }    

    private void Update()
    {
        if (CanUpdateCharacter())
        {
            HandleMovement();
            HandleInput();
        }
    }

    private bool CanUpdateCharacter()
    {
        return !GameManager.PauseManager.IsPaused && GameManager.LevelManager.IsCurrentLevelState("Running");
    }

    private void HandleMovement()
    {
        transform.position = new Vector2(transform.position.x + _currentSpeed, transform.position.y);
        /*
        if (IsGrounded())
        {
            Vector3 currentRotation = transform.eulerAngles;
            if (currentRotation != Vector3.zero)
            {
                transform.eulerAngles = Vector3.zero;
            }
        }
        */
    }

    public void SetUpFightScene()
    {
        InterruptState();
        ChangeState(CharacterState.Fighting);
    }

    public void EndUpFightScene()
    {
        ChangeState(CharacterState.Running);
    }

    private bool canDoInput()
    {
        return (IsGrounded() && _currentCharacterState == CharacterState.Running);
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A) && canDoInput() && _sprintCooldownText.text == "")
        {
            _activeCoroutine = StartCoroutine(StartSprinting());
        }

        if (Input.GetKeyDown(KeyCode.Space) && canDoInput())
        {
            _activeCoroutine = StartCoroutine(StartJumping());
        }

        if (Input.GetKeyDown(KeyCode.Z) && canDoInput() && _slideCooldownText.text == "")
        {
            _activeCoroutine = StartCoroutine(StartSliding());
        }
    }

    private IEnumerator StartSprinting()
    {
        ChangeState(CharacterState.Sprinting);
        SetSpeed(_defaultSpeed * _sprintMultiplier);
        yield return new WaitForSeconds(1f);

        SetSpeed(_defaultSpeed);
        StartCoroutine(DisplayCooldown(_sprintCooldownText, 5f));
        ChangeState(CharacterState.Running);
    }

    private IEnumerator DisplayCooldown(TextMeshProUGUI text, float duration)
    {
        while (duration > 0)
        {
            text.text = duration.ToString();
            duration -= 1f;
            yield return new WaitForSeconds(1f);
        }
        text.text = "";
        ChangeState(CharacterState.Running);
    }

    private IEnumerator StartSliding()
    {
        ChangeState(CharacterState.Sliding);
        this.transform.eulerAngles += new Vector3(0f, 0f, 90f);
        StartCoroutine(DisplayCooldown(_slideCooldownText, 3f));

        yield return new WaitForSeconds(0.75f);

        yield return StartCoroutine(WaitForSpaceAbove());

        this.transform.eulerAngles += new Vector3(0f, 0f, -90f);
        ChangeState(CharacterState.Running);
    }

    private IEnumerator StartJumping()
    {
        ChangeState(CharacterState.Jumping);
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        yield return null;

        ChangeState(CharacterState.Running);
    }

    public void InterruptState()
    {
        if (_activeCoroutine != null) {
            StopCoroutine(_activeCoroutine);
            _activeCoroutine = null;
            if (_currentCharacterState == CharacterState.Jumping && _activeCoroutine != null)
            {
                GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            }
        }
    }

    private IEnumerator WaitForSpaceAbove()
    {
        // Raycast ou BoxCast vers le haut pour vérifier l'espace disponible
        bool isSpaceAbove = false;
        float checkInterval = 0.1f;  // Intervalle de vérification en secondes

        while (!isSpaceAbove)
        {
            // Vérifier s'il y a de la place au-dessus de la tête
            isSpaceAbove = CheckSpaceAbove();

            if (!isSpaceAbove)
            {
                yield return new WaitForSeconds(checkInterval);
            }
        }
    }

    private bool CheckSpaceAbove()
    {
        // Taille du personnage pour déterminer combien de place il lui faut
        float characterHeight = 75f; // Hauteur à vérifier
        Vector2 rayStart = new Vector2(transform.position.x, transform.position.y);
        Vector2 rayDirection = Vector2.up; 
        float rayLength = characterHeight;
        RaycastHit2D hit = Physics2D.Raycast(rayStart, rayDirection, rayLength);

        // Si le Raycast ne touche rien, alors il y a de l'espace pour se relever
        return hit.collider == null;
    }

    private bool IsGrounded()
    {
        // Obtenez le rayon du bord (Edge Radius) du BoxCollider2D
        float edgeRadius = _boxCollider.edgeRadius;
        Vector2 boxSizeWithEdgeRadius = _boxCollider.bounds.size + new Vector3(edgeRadius * 2, edgeRadius * 2, 0);
        return Physics2D.BoxCast(_boxCollider.bounds.center, boxSizeWithEdgeRadius, 0f, Vector2.down, 0.1f, _groundLayerMask);
        //return Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f, Vector2.down, 0.1f, _groundLayerMask);
    }

    private void SetSpeed(float speed)
    {
        _currentSpeed = speed;
    }

    private void ChangeState(CharacterState newState)
    {
        _currentCharacterState = newState;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !GameManager.LevelManager.IsCurrentLevelState("Fighting"))
        {
            GameManager.UiManager.DisplayQuestions();
            GameManager.LevelManager.SetEnemy(collision.gameObject);
            GameManager.LevelManager.ChangeLevelStateByString("Fighting");
        }
    }
}
