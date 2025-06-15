using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EquipmentObject", menuName = "Equipment/EquipmentObject", order = 1)]
public class EquipmentObject : ScriptableObject
{
    public string itemName; // Nom de l'objet
    public Sprite sprite; // Icône affichée dans l'interface
    //public GameObject prefab; // Icône affichée dans la scène unity
    public EquipmentManager.Type type; // Type de l'équipement (défini ailleurs)
    public bool isEnable; // L'équipement est-il débloqué
    public EquipmentManager.Rarity rarity; // Type de rareté (défini ailleurs)
    public float bonusDamage; // Bonus de dégâts
    public float bonusDefense; // Bonus de défense
    public float bonusSpeed; // Bonus de vitesse
}