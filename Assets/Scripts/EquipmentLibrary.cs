using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EquipmentLibrary", menuName = "Equipment/Library")]
public class EquipmentLibrary : ScriptableObject
{
    //Remplir ces listes pour les utiliser dans EquipmentManger
    public List<EquipmentObject> helmets;   // Liste des casques
    public List<EquipmentObject> shields;  // Liste des boucliers
    public List<EquipmentObject> boots;    // Liste des bottes
    public List<EquipmentObject> armors;   // Liste des armures
}