using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Equipment")]
public class EquipmentObject : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public EquipmentManager.EquipmentType type;
    public int bonusDamage;
    public int bonusDefense;
    public bool isDefault; // Est-ce que cet équipement est débloqué par défaut ? --> faire la liste des équipements
}