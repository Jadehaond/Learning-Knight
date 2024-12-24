using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentLibrary", menuName = "Equipment/EquipmentLibrary")]
public class EquipmentLibrary : ScriptableObject
{
    public List<EquipmentObject> helmets;
    public List<EquipmentObject> shields;
    public List<EquipmentObject> boots;
    public List<EquipmentObject> armors;
    public List<EquipmentObject> weapons;
}
