using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public enum EquipmentType { Helmet, Shield, Boots, Armor, Weapon }
    public EquipmentLibrary equipmentLibrary;

    public List<EquipmentObject> unlockedHelmets = new List<EquipmentObject>();
    public List<EquipmentObject> unlockedShields = new List<EquipmentObject>();
    public List<EquipmentObject> unlockedBoots = new List<EquipmentObject>();
    public List<EquipmentObject> unlockedArmors = new List<EquipmentObject>();
    public List<EquipmentObject> unlockedWeapons = new List<EquipmentObject>();

    void Start()
    {
        // --> Ajouter dans unlock les équipements par défauts
        // Débloquer certains équipements de départ
        foreach (var helmet in equipmentLibrary.helmets)
        {
            if (helmet.isDefault)
                unlockedHelmets.Add(helmet);
        }

        foreach (var shield in equipmentLibrary.shields)
        {
            if (shield.isDefault)
                unlockedShields.Add(shield);
        }

        foreach (var boot in equipmentLibrary.boots)
        {
            if (boot.isDefault)
                unlockedBoots.Add(boot);
        }

        foreach (var armor in equipmentLibrary.armors)
        {
            if (armor.isDefault)
                unlockedArmors.Add(armor);
        }

        foreach (var weapon in equipmentLibrary.weapons)
        {
            if (weapon.isDefault)
                unlockedWeapons.Add(weapon);
        }
    }

    public void UnlockEquipment(EquipmentObject newEquipment)
    {
        switch (newEquipment.type)
        {
            case EquipmentType.Helmet:
                if (!unlockedHelmets.Contains(newEquipment))
                    unlockedHelmets.Add(newEquipment);
                break;
            case EquipmentType.Shield:
                if (!unlockedShields.Contains(newEquipment))
                    unlockedShields.Add(newEquipment);
                break;
            case EquipmentType.Boots:
                if (!unlockedBoots.Contains(newEquipment))
                    unlockedBoots.Add(newEquipment);
                break;
            case EquipmentType.Armor:
                if (!unlockedArmors.Contains(newEquipment))
                    unlockedArmors.Add(newEquipment);
                break;
            case EquipmentType.Weapon:
                if (!unlockedWeapons.Contains(newEquipment))
                    unlockedWeapons.Add(newEquipment);
                break;
        }
    }

    public void Equip(EquipmentObject newItem)
    {
        switch (newItem.type)
        {
            case EquipmentType.Helmet:
                EquipHelmet(newItem);
                break;
            case EquipmentType.Shield:
                EquipShield(newItem);
                break;
            case EquipmentType.Boots:
                EquipBoots(newItem);
                break;
            case EquipmentType.Armor:
                EquipArmor(newItem);
                break;
            case EquipmentType.Weapon:
                EquipArmor(newItem);
                break;
        }
    }

    private void EquipHelmet(EquipmentObject newHelmet)
    {
        Debug.Log("Équipé : " + newHelmet.itemName);
        // Remplacez l'ancien casque ou appliquez les bonus ici
    }

    private void EquipShield(EquipmentObject newShield)
    {
        Debug.Log("Équipé : " + newShield.itemName);
    }

    private void EquipBoots(EquipmentObject newBoots)
    {
        Debug.Log("Équipé : " + newBoots.itemName);
    }

    private void EquipArmor(EquipmentObject newArmor)
    {
        Debug.Log("Équipé : " + newArmor.itemName);
    }

    private void EquipWeapon(EquipmentObject newWeapon)
    {
        Debug.Log("Équipé : " + newWeapon.itemName);
    }

}
