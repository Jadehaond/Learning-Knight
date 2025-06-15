using System.Collections;
using UnityEngine;
using TMPro;

public class KnightManager : MonoBehaviour
{
    #region Constants & Static Values

    // Position et rotation par défaut du personnage hors combat
    private static readonly Vector3 StandByPosition = new Vector3(175f, 75f, 0f);
    private static readonly Vector3 StandByRotation = Vector3.zero;

    // Valeurs de mouvement de base
    private const float DefaultSpeed = 30f;
    private const float SprintMultiplier = 2f;
    private const float JumpForce = 150f;

    #endregion

    #region Private Fields

    private float _currentSpeed = 0f;
    private GameManager _instance;

    // Accès au singleton GameManager
    private GameManager GameManager => _instance ??= GameManager.Instance;

    private BoxCollider2D _boxCollider;
    private Coroutine _activeCoroutine = null;

    // États possibles du personnage
    private enum CharacterState { Running, Jumping, Sliding, Sprinting, Fighting }
    private CharacterState _currentCharacterState = CharacterState.Running;

    private SpriteRenderer knightSprite;

    #endregion

    #region Inspector References

    [Header("Settings")]
    [SerializeField] private LayerMask _groundLayerMask; // Couches considérées comme sol
    [SerializeField] private TextMeshProUGUI _sprintCooldownText; // Texte UI cooldown sprint
    [SerializeField] private TextMeshProUGUI _slideCooldownText;  // Texte UI cooldown glissade

    #endregion

    #region Unity Events

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        knightSprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ResetCharacterState();
    }

    private void Update()
    {
        if (CanUpdateCharacter())
        {
            HandleInput();
        }
    }

    private void FixedUpdate()
    {
        if (CanUpdateCharacter())
        {
            HandleMovement();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si on entre en collision avec un ennemi et qu'on n'est pas déjà en combat
        if ((collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss"))
            && !GameManager.LevelManager.IsCurrentLevelState("Fighting"))
        {
            GameManager.UiManager.DisplayQuestions();
            GameManager.LevelManager.SetEnemy(collision.gameObject);
            GameManager.LevelManager.ChangeLevelStateByString("Fighting");
        }
    }

    #endregion

    #region Movement & State

    // Vérifie si le personnage peut être contrôlé (pas en pause et dans le bon état de niveau)
    private bool CanUpdateCharacter() =>
        !GameManager.PauseManager.IsPaused && GameManager.LevelManager.IsCurrentLevelState("Running");

    // Gère le mouvement horizontal
    private void HandleMovement()
    {
        float moveDistance = _currentSpeed * Time.deltaTime;
        transform.position += new Vector3(moveDistance, 0f, 0f);
    }

    // Gère les entrées claviers pour les actions
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A) && CanDoInput() && _sprintCooldownText.text == "")
        {
            _activeCoroutine = StartCoroutine(StartSprinting());
        }

        if (Input.GetKeyDown(KeyCode.Space) && CanDoInput())
        {
            _activeCoroutine = StartCoroutine(StartJumping());
        }

        if (Input.GetKeyDown(KeyCode.Z) && CanDoInput() && _slideCooldownText.text == "")
        {
            _activeCoroutine = StartCoroutine(StartSliding());
        }
    }

    // Détermine si le personnage peut faire une action
    private bool CanDoInput() => IsGrounded() && _currentCharacterState == CharacterState.Running;

    // Coroutine de sprint
    private IEnumerator StartSprinting()
    {
        ChangeState(CharacterState.Sprinting);
        SetSpeed(DefaultSpeed * SprintMultiplier);
        yield return new WaitForSeconds(1f);

        SetSpeed(DefaultSpeed);
        StartCoroutine(DisplayCooldown(_sprintCooldownText, 5f));
        ChangeState(CharacterState.Running);
    }

    // Coroutine de saut
    private IEnumerator StartJumping()
    {
        ChangeState(CharacterState.Jumping);
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        yield return null;
        ChangeState(CharacterState.Running);
    }

    // Coroutine de glissade
    private IEnumerator StartSliding()
    {
        ChangeState(CharacterState.Sliding);
        transform.eulerAngles += new Vector3(0f, 0f, 90f);
        StartCoroutine(DisplayCooldown(_slideCooldownText, 3f));

        yield return new WaitForSeconds(0.75f);
        yield return StartCoroutine(WaitForSpaceAbove());

        transform.eulerAngles -= new Vector3(0f, 0f, 90f);
        ChangeState(CharacterState.Running);
    }

    // Affiche un cooldown sur un texte pendant un certain temps
    private IEnumerator DisplayCooldown(TextMeshProUGUI text, float duration)
    {
        while (duration > 0)
        {
            text.text = duration.ToString("0");
            duration -= 1f;
            yield return new WaitForSeconds(1f);
        }
        text.text = "";
    }

    // Attend qu'il y ait de l'espace au-dessus du joueur (utile pour se relever d'une glissade)
    private IEnumerator WaitForSpaceAbove()
    {
        while (!CheckSpaceAbove())
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    // Vérifie via raycast s'il y a un obstacle au-dessus
    private bool CheckSpaceAbove()
    {
        float rayLength = 75f;
        Vector2 origin = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, rayLength);
        return hit.collider == null;
    }

    // Vérifie si le personnage est au sol via un BoxCast
    private bool IsGrounded()
    {
        float edgeRadius = _boxCollider.edgeRadius;
        Vector2 boxSize = _boxCollider.bounds.size + new Vector3(edgeRadius * 2, edgeRadius * 2, 0);
        return Physics2D.BoxCast(_boxCollider.bounds.center, boxSize, 0f, Vector2.down, 0.1f, _groundLayerMask);
    }

    // Met à jour la vitesse du personnage
    private void SetSpeed(float speed) => _currentSpeed = speed;

    // Change l'état du personnage
    private void ChangeState(CharacterState newState) => _currentCharacterState = newState;

    // Interrompt l'action en cours si une coroutine est active
    public void InterruptState()
    {
        if (_activeCoroutine != null)
        {
            StopCoroutine(_activeCoroutine);
            _activeCoroutine = null;
        }
    }

    // Réinitialise l'état du personnage
    public void ResetCharacterState()
    {
        SetSpeed(DefaultSpeed);
        transform.eulerAngles = StandByRotation;
        transform.position = StandByPosition;

        CharacterManager cm = GetComponent<CharacterManager>();
        cm.GainLife(cm.MaxHealth);
    }

    // Prépare le personnage pour une scène de combat
    public void SetUpFightScene()
    {
        InterruptState();
        ChangeState(CharacterState.Fighting);
    }

    // Termine la scène de combat
    public void EndUpFightScene()
    {
        ChangeState(CharacterState.Running);
    }

    #endregion

    #region Visual Management


    /// <summary>
    /// Modifie le sprite affiché par le SpriteRenderer.
    /// </summary>
    /// <param name="newSprite">Le nouveau sprite à afficher.</param>
    public void SetSprite(Sprite newSprite)
    {
        // Si le SpriteRenderer est bien présent, applique le sprite
        if (knightSprite != null)
            knightSprite.sprite = newSprite;
    }

    /// <summary>
    /// Retourne le sprite actuellement affiché.
    /// </summary>
    /// <returns>Sprite actuel ou null si non disponible.</returns>
    public Sprite GetSprite()
    {
        // Retourne le sprite actuel si possible
        return knightSprite != null ? knightSprite.sprite : null;
    }

    public void UpdateSprite(Sprite newSprite)
    {
        if (IsChildSpriteAssigned(gameObject, $"Accessory_{newSprite.name}")) {
            RemoveChildByName(gameObject, $"Accessory_{newSprite.name}");
        } 
        else 
        {
            CreateChildSpriteObject(gameObject, $"Accessory_{newSprite.name}", newSprite, new Vector3(0f, 1f, 0f), 5);
        }
    }

    /// <summary>
    /// Crée un GameObject enfant avec un SpriteRenderer au-dessus du personnage.
    /// </summary>
    /// <param name="parent">Le GameObject parent (ex. ton personnage)</param>
    /// <param name="childName">Le nom du GameObject enfant (ex. "Helmet")</param>
    /// <param name="sprite">Le sprite à afficher (ex. un casque)</param>
    /// <param name="localPosition">Position locale par rapport au parent</param>
    /// <param name="sortingOrder">Ordre de rendu (au-dessus du personnage)</param>
    /// <returns>Le SpriteRenderer du nouvel objet</returns>
    public SpriteRenderer CreateChildSpriteObject(
        GameObject parent,
        string childName,
        Sprite sprite,
        Vector3 localPosition,
        int sortingOrder = 1)
    {
        // Crée un nouvel objet
        GameObject child = new GameObject(childName);
        
        // Le rend enfant du parent
        child.transform.SetParent(parent.transform);

        // Position locale (ex: au-dessus de la tête)
        child.transform.localPosition = localPosition;
        
        // Ajoute le SpriteRenderer
        SpriteRenderer renderer = child.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = sortingOrder;

        return renderer;
    }

    /// <summary>
    /// Supprime un GameObject enfant d’un parent en fonction de son nom.
    /// </summary>
    /// <param name="parent">Le GameObject parent (ex: le personnage)</param>
    /// <param name="childName">Le nom du GameObject enfant à supprimer (ex: "Helmet")</param>
    public void RemoveChildByName(GameObject parent, string childName)
    {
        Transform child = parent.transform.Find(childName);
        if (child != null)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Vérifie si un enfant avec un SpriteRenderer contient déjà un sprite assigné.
    /// </summary>
    /// <param name="parent">Le GameObject parent</param>
    /// <param name="childName">Le nom de l’enfant à vérifier</param>
    /// <returns>True si l’enfant existe et a un sprite assigné, sinon false</returns>
    public bool IsChildSpriteAssigned(GameObject parent, string childName)
    {
        Transform child = parent.transform.Find(childName);
        if (child != null)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            return sr != null && sr.sprite != null;
        }

        return false;
    }

    #endregion
}
