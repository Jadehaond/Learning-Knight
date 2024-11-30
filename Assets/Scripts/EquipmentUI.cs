using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    public EquipmentManager equipmentManager;

    public Transform helmetsSection;
    public Transform shieldsSection;
    public Transform bootsSection;
    public Transform armorsSection;
    public Transform weaponsSection;

    public GameObject equipmentSlotPrefab;

    public void UpdateUI()
    {
        // Vider chaque section
        foreach (Transform child in helmetsSection) Destroy(child.gameObject);
        foreach (Transform child in shieldsSection) Destroy(child.gameObject);
        foreach (Transform child in bootsSection) Destroy(child.gameObject);
        foreach (Transform child in armorsSection) Destroy(child.gameObject);

        // Ajouter les équipements débloqués
        foreach (var helmet in equipmentManager.unlockedHelmets)
            CreateEquipmentSlot(helmet, helmetsSection);
        foreach (var shield in equipmentManager.unlockedShields)
            CreateEquipmentSlot(shield, shieldsSection);
        foreach (var boot in equipmentManager.unlockedBoots)
            CreateEquipmentSlot(boot, bootsSection);
        foreach (var armor in equipmentManager.unlockedArmors)
            CreateEquipmentSlot(armor, armorsSection);
    }

    private void CreateEquipmentSlot(EquipmentObject equipment, Transform parent)
    {
        /*var slot = Instantiate(equipmentSlotPrefab, parent);
        slot.GetComponentInChildren<Image>().sprite = equipment.icon;
        slot.GetComponentInChildren<Text>().text = equipment.itemName;

        // Ajouter des interactions
        var button = slot.GetComponent<Button>();
        button.onClick.AddListener(() => equipmentManager.Equip(equipment));*/
    }
}
