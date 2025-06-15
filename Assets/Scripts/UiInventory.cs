using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles inventory UI, player sprite preview and basic accessory management.
/// </summary>
public class UiInventory : MonoBehaviour
{
    #region Inspector

    [Header("Scene References")]
    [SerializeField] private GameObject player;                     // Player GameObject in the scene
    [SerializeField] private EquipmentManager equipmentManager;     // Central equipment logic
    [SerializeField] private Transform contentPanel;                // Grid container for inventory buttons
    [SerializeField] private Transform KnightPanel;
    [SerializeField] private Image knightImage;                     // UI preview of the current player sprite

    [Header("Buttons")]
    [SerializeField] private Sprite equipmentItemBackground;        // Background sprite for inventory buttons
    [SerializeField] private int columns = 3;                       // Columns in the GridLayout
    [SerializeField] private Vector2 cellSize = new(75f, 75f);      // Size of each button (width, height)
    [SerializeField] private Vector2 cellSpacing = new(10f, 10f);   // Spacing between buttons (x, y)

    #endregion

    private KnightManager knightManager; 

    #region Unity lifecycle

    private void Awake()
    {
        // Cache the SpriteRenderer on the player.
        knightManager = player.GetComponent<KnightManager>();

        // Fallback: if knightImage wasn't dragged in the Inspector, try to find one in children.
        if (knightImage == null)
            knightImage = GetComponentInChildren<Image>();
    }

    private void Start()
    {
        DisplayKnight();   // Show current sprite on UI.
        PopulateInventory();
    }

    #endregion

    #region Knight preview

    /// <summary>
    /// Copies the live sprite on the player to the UI preview.
    /// </summary>
    public void DisplayKnight()
    {
        if (knightManager.GetSprite()  == null || knightImage == null) return;

        knightImage.sprite = knightManager.GetSprite();
        knightImage.preserveAspect = true;
    }

    /// <summary>
    /// Changes the player's SpriteRenderer and refreshes the UI preview.
    /// Call this after equipping a new item.
    /// </summary>
    public void ChangePlayerSprite(Sprite newSprite)
    {
        if (knightManager.GetSprite() == null || newSprite == null) return;
        knightManager.UpdateSprite(newSprite); 
        
        if (knightManager.IsChildSpriteAssigned(KnightPanel.gameObject, $"Accessory_{newSprite.name}")) {
            RemoveAccessory($"Accessory_{newSprite.name}");
        } 
        else 
        {
            AddAccessoryToPlayer(newSprite);
        }
        DisplayKnight();
    }

    #endregion

    #region Accessory helpers

    public void AddAccessoryToPlayer(Sprite accessorySprite)
    {
        if (accessorySprite == null) return;

        GameObject go = new($"Accessory_{accessorySprite.name}", typeof(SpriteRenderer));
        //go.transform.SetParent(player.transform, false);
        go.transform.SetParent(KnightPanel, false);
        var sr = go.GetComponent<SpriteRenderer>();
        sr.sprite = accessorySprite;

        // Ensure accessories render above the base sprite.
        //sr.sortingOrder = playerSpriteRenderer.sortingOrder + 1;
    }

    public void RemoveAccessory(string accessoryName)
    {
        Transform child = player.transform.Find($"Accessory_{accessoryName}");
        if (child != null) Destroy(child.gameObject);
    }

    #endregion

    #region Inventory UI

    /// <summary>
    /// Populates the UI grid with every unlocked equipment item.
    /// </summary>
    public void PopulateInventory()
    {
        ConfigureGrid();

        // Clear existing buttons.
        foreach (Transform child in contentPanel)
            Destroy(child.gameObject);

        // Collect unlocked items.
        List<EquipmentObject> items = new();
        items.AddRange(equipmentManager.GetUnlockedHelmets());
        items.AddRange(equipmentManager.GetUnlockedShields());
        items.AddRange(equipmentManager.GetUnlockedBoots());
        items.AddRange(equipmentManager.GetUnlockedArmors());
        items.AddRange(equipmentManager.GetUnlockedWeapons());

        // Create a button for each item.
        foreach (var equipment in items)
            CreateInventoryButton(equipment);

        // Resize content panel so the ScrollRect knows its bounds.
        int rows = Mathf.CeilToInt(items.Count / (float)columns);
        var rect = contentPanel.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, rows * (cellSize.y + cellSpacing.y));
    }

    private void ConfigureGrid()
    {
        var grid = contentPanel.GetComponent<GridLayoutGroup>();
        if (grid == null) return;

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
        grid.cellSize = cellSize;
        grid.spacing = cellSpacing;
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.padding = new RectOffset(5, 5, 5, 5);
    }

    private void CreateInventoryButton(EquipmentObject eq)
    {
        GameObject btnGO = new($"Btn_{eq.itemName}",
                              typeof(RectTransform),
                              typeof(Button),
                              typeof(Image),
                              typeof(LayoutElement));
        btnGO.transform.SetParent(contentPanel, false);
        btnGO.GetComponent<RectTransform>().sizeDelta = cellSize;

        // Background
        var bg = btnGO.GetComponent<Image>();
        bg.sprite = equipmentItemBackground;

        // Icon
        GameObject iconGO = new("Icon", typeof(Image));
        iconGO.transform.SetParent(btnGO.transform, false);
        var iconImg = iconGO.GetComponent<Image>();
        iconImg.sprite = eq.sprite;
        iconImg.rectTransform.sizeDelta = cellSize * 0.8f;
        iconImg.preserveAspect = true;

        // Click action
        btnGO.GetComponent<Button>().onClick.AddListener(() =>
        {
            equipmentManager.Equip(eq);   // Business logic
            ChangePlayerSprite(eq.sprite); // Visual feedback
        });
    }

    #endregion
}
