using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EquipmentManager : MonoBehaviour
{
    private GameManager _instance;
    private GameManager GameManager => _instance ??= GameManager.Instance;
    
    private GameObject equipmentContainer;
    public EquipmentLibrary equipmentLibrary;

    private GameObject Knight;
    
    public enum Type { Null, Helmet, Shield, Boots, Armor, Weapon }
    public enum Rarity { Null, Common, Rare, Epic, Legendary }
    
    public List<EquipmentObject> unlockedHelmets = new List<EquipmentObject>();
    public List<EquipmentObject> unlockedShields = new List<EquipmentObject>();
    public List<EquipmentObject> unlockedBoots = new List<EquipmentObject>();
    public List<EquipmentObject> unlockedArmors = new List<EquipmentObject>();
    public List<EquipmentObject> unlockedWeapons = new List<EquipmentObject>();

    void Start()
    {
        foreach (var helmet in equipmentLibrary.helmets)
        {
            if (helmet.isEnable) {
                unlockedHelmets.Add(helmet);
            }
        }

        foreach (var shield in equipmentLibrary.shields)
        {
            if (shield.isEnable)
                unlockedShields.Add(shield);
        }

        foreach (var boot in equipmentLibrary.boots)
        {
            if (boot.isEnable)
                unlockedBoots.Add(boot);
        }

        foreach (var armor in equipmentLibrary.armors)
        {
            if (armor.isEnable)
                unlockedArmors.Add(armor);
        }

        foreach (var weapon in equipmentLibrary.weapons)
        {
            if (weapon.isEnable)
                unlockedWeapons.Add(weapon);
        }
    }

    public void UnlockEquipment(EquipmentObject newEquipment)
    {
        switch (newEquipment.type)
        {
            case Type.Helmet:
                if (!unlockedHelmets.Contains(newEquipment))
                    unlockedHelmets.Add(newEquipment);
                break;
            case Type.Shield:
                if (!unlockedShields.Contains(newEquipment))
                    unlockedShields.Add(newEquipment);
                break;
            case Type.Boots:
                if (!unlockedBoots.Contains(newEquipment))
                    unlockedBoots.Add(newEquipment);
                break;
            case Type.Armor:
                if (!unlockedArmors.Contains(newEquipment))
                    unlockedArmors.Add(newEquipment);
                break;
            case Type.Weapon:
                if (!unlockedWeapons.Contains(newEquipment))
                    unlockedWeapons.Add(newEquipment);
                break;
        }
    }

    public void Equip(EquipmentObject newItem)
    {
        switch (newItem.type)
        {
            case Type.Helmet:
                EquipHelmet(newItem);
                break;
            case Type.Shield:
                EquipShield(newItem);
                break;
            case Type.Boots:
                EquipBoots(newItem);
                break;
            case Type.Armor:
                EquipArmor(newItem);
                break;
            case Type.Weapon:
                EquipArmor(newItem);
                break;
        }
    }

    public static Type GetTypeFromStringToEnum(String newItem)
    {
        switch (newItem)
        {
            case "Helmet":
                return Type.Helmet;
            case "Shield":
                return Type.Shield;
            case "Boots":
                return Type.Boots;
            case "Armor":
                return Type.Armor;
            case "Weapon":
                return Type.Weapon;
            default:
                return Type.Null;
        }
    }

    public static Rarity GetRarityFromStringToEnum(String newItem)
    {
        switch (newItem)
        {
            case "Common":
                return Rarity.Common;
            case "Rare":
                return Rarity.Rare;
            case "Epic":
                return Rarity.Epic;
            case "Legendary":
                return Rarity.Legendary;
            default:
                return Rarity.Null;
        }
    }
    
    public List<EquipmentObject> AllEquipment
    {
        get
        {
            List<EquipmentObject> all = new List<EquipmentObject>();
            all.AddRange(unlockedHelmets);
            all.AddRange(unlockedShields);
            all.AddRange(unlockedBoots);
            all.AddRange(unlockedArmors);
            all.AddRange(unlockedWeapons);
            return all;
        }
    }

    public List<EquipmentObject> GetEquipmentByType(Type type)
    {
        List<EquipmentObject> filteredList = new List<EquipmentObject>();
        
        if (type == Type.Helmet)
            filteredList.AddRange(equipmentLibrary.helmets);
        if (type == Type.Shield)
            filteredList.AddRange(equipmentLibrary.shields);
        if (type == Type.Boots)
            filteredList.AddRange(equipmentLibrary.boots);
        if (type == Type.Armor)
            filteredList.AddRange(equipmentLibrary.armors);
        if (type == Type.Weapon)
            filteredList.AddRange(equipmentLibrary.weapons);

        return filteredList;
    }


    private void EquipHelmet(EquipmentObject newHelmet)
    {
        Debug.Log("Équipé : " + newHelmet.itemName);
        Knight.GetComponent<CharacterManager>().Defense += newHelmet.bonusDefense;
        Knight.GetComponent<CharacterManager>().Damage += newHelmet.bonusDamage;
    }

    private void EquipShield(EquipmentObject newShield)
    {
        Debug.Log("Équipé : " + newShield.itemName);
        Knight.GetComponent<CharacterManager>().Defense += newShield.bonusDefense;
        Knight.GetComponent<CharacterManager>().Damage += newShield.bonusDamage;
    }

    private void EquipBoots(EquipmentObject newBoots)
    {
        Debug.Log("Équipé : " + newBoots.itemName);
        Knight.GetComponent<CharacterManager>().Defense += newBoots.bonusDefense;
        Knight.GetComponent<CharacterManager>().Damage += newBoots.bonusDamage;
    }

    private void EquipArmor(EquipmentObject newArmor)
    {
        Debug.Log("Équipé : " + newArmor.itemName);
        Knight.GetComponent<CharacterManager>().Defense += newArmor.bonusDefense;
        Knight.GetComponent<CharacterManager>().Damage += newArmor.bonusDamage;
    }

    private void EquipWeapon(EquipmentObject newWeapon)
    {
        Debug.Log("Équipé : " + newWeapon.itemName);
        Knight.GetComponent<CharacterManager>().Defense += newWeapon.bonusDefense;
        Knight.GetComponent<CharacterManager>().Damage += newWeapon.bonusDamage;
    }

     // Getter pour unlockedHelmets
    public List<EquipmentObject> GetUnlockedHelmets()
    {
        return unlockedHelmets;
    }

    // Getter pour unlockedShields
    public List<EquipmentObject> GetUnlockedShields()
    {
        return unlockedShields;
    }

    // Getter pour unlockedBoots
    public List<EquipmentObject> GetUnlockedBoots()
    {
        return unlockedBoots;
    }

    // Getter pour unlockedArmors
    public List<EquipmentObject> GetUnlockedArmors()
    {
        return unlockedArmors;
    }

    // Getter pour unlockedWeapons
    public List<EquipmentObject> GetUnlockedWeapons()
    {
        return unlockedWeapons;
    }

}
