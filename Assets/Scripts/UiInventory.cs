using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiInventory : MonoBehaviour
{
    public Transform KnightPanel;
    public GameObject player;
    public Transform StatsPanel;

    public EquipmentManager equipmentManager;   // Référence à l'EquipmentManager
    public Sprite equipmentItemPrefab;          // Background du bouton - sprite
    public Transform contentPanel;              // Le parent dans lequel les éléments seront instanciés
    private int columns = 3;                     // Nombre de colonnes dans le GridLayout
    private float buttonWidth = 75f;            // Largeur des boutons
    private float buttonHeight = 75f;           // Hauteur des boutons

    public void DisplayKnight()
    {
        // Image du chevalier
    }

    public void DisplayStats() 
    {
        // Statistique du chevalier
    }

    public void PopulateInventory()
    {
        // Récupérer le GridLayoutGroup
        GridLayoutGroup gridLayout = contentPanel.GetComponent<GridLayoutGroup>();
        if (gridLayout != null)
        {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount; // Fixer le nombre de colonnes
            gridLayout.constraintCount = columns;  // Nombre de colonnes
            gridLayout.cellSize = new Vector2(buttonWidth, buttonHeight); // Taille des cellules
            gridLayout.spacing = new Vector2(10f, 10f); // Espacement entre les boutons
            gridLayout.childAlignment = TextAnchor.MiddleCenter; // Centrer les boutons dans chaque cellule
            gridLayout.padding = new RectOffset(5, 5, 5, 5); // Padding autour de la grille
        }

        // Vider la liste actuelle des équipements affichés
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // Ajouter chaque équipement à l'inventaire
        List<EquipmentObject> allEquipment = new List<EquipmentObject>();
        allEquipment.AddRange(equipmentManager.GetUnlockedHelmets());
        allEquipment.AddRange(equipmentManager.GetUnlockedShields());
        allEquipment.AddRange(equipmentManager.GetUnlockedBoots());
        allEquipment.AddRange(equipmentManager.GetUnlockedArmors());
        allEquipment.AddRange(equipmentManager.GetUnlockedWeapons());

        // Ajouter chaque équipement sous forme de bouton
        foreach (var equipment in allEquipment)
        {
            // Créer un nouveau bouton
            GameObject newButton = new GameObject($"Button_{equipment.itemName}", typeof(RectTransform), typeof(Button), typeof(Image), typeof(LayoutElement));
            newButton.transform.SetParent(contentPanel, false);
            RectTransform buttonRectTransform = newButton.GetComponent<RectTransform>();
            buttonRectTransform.sizeDelta = new Vector2(70f, 70f);

            // Utiliser le sprite comme fond du bouton
            Image buttonImage = newButton.GetComponent<Image>();
            buttonImage.sprite = equipmentItemPrefab;
            
            // Action au clic du bouton
            Button button = newButton.GetComponent<Button>();
            button.onClick.AddListener(() => equipmentManager.Equip(equipment));

            // Créer une image pour l'icône de l'équipement et la positionner sur le bouton
            GameObject iconObject = new GameObject("IconImage", typeof(Image));
            iconObject.transform.SetParent(newButton.transform, false);  // Parenté au bouton

            // Référence à l'image de l'icône
            Image iconImage = iconObject.GetComponent<Image>();
            iconImage.sprite = equipment.sprite;
            iconImage.rectTransform.sizeDelta = new Vector2(buttonWidth * 0.8f, buttonHeight * 0.8f);
            iconImage.preserveAspect = true;  // Activer Preserve Aspect pour éviter la distorsion

            // Centrer l'icône dans le bouton
            RectTransform iconRectTransform = iconObject.GetComponent<RectTransform>();
            iconRectTransform.anchoredPosition = Vector2.zero;
        }

        RectTransform contentRect = contentPanel.GetComponent<RectTransform>();
        int numberOfRows = Mathf.CeilToInt(allEquipment.Count / (float)columns); // Calcul du nombre de lignes
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, numberOfRows * (buttonHeight + 10f)); // Ajuster la hauteur du contenu
    }

    public void AddAccessoryToPlayer(Sprite accessorySprite)
    {
       // Ajouter un accessoire
    }

    public void RemovedAccessoryToPlayer(Sprite accessorySprite)
    {
       // Enlever un accessoire
    }

    public void ReplaceAccessoryToPlayer(Sprite accessorySprite)
    {
       // Remplacer un accessoire
    }

    public void ChangePlayerSprite(Sprite newSprite)
    {
        // reset de sprite
    }
}
