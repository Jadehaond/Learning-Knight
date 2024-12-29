using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public List<EquipmentObject> inventory = new List<EquipmentObject>();
    private int limitStorage = 50;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            EquipmentLibrary library = UnityEditor.AssetDatabase.LoadAssetAtPath<EquipmentLibrary>("Assets/EquipmentLibrary.asset");

            addEquipmentToInventory(library.weapons[0]);
        }
    }

    void addEquipmentToInventory(EquipmentObject obj)
    {
        if(inventory.Count < limitStorage)
        {
            inventory.Add(obj);
        }
    }

}
