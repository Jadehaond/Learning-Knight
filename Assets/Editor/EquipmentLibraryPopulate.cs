using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public class EquipmentLibraryPopulator : MonoBehaviour
{
    [MenuItem("Tools/Populate Equipment Library From CSV")]
    public static void PopulateEquipmentLibraryFromCSV()
    {
        string filePath = "Assets/Resources/equipements.csv";

        if (!File.Exists(filePath))
        {
            Debug.LogError("Le fichier CSV est introuvable à : " + filePath);
            return;
        }

        List<EquipmentObject> equipmentDataList = ReadCSV(filePath);

        if (equipmentDataList == null)
        {
            Debug.LogError("equipmentDataList is null!");
            return;
        }

        EquipmentLibrary library = AssetDatabase.LoadAssetAtPath<EquipmentLibrary>("Assets/EquipmentLibrary.asset");

        if (library == null)
        {
            Debug.LogError("EquipmentLibrary asset not found. Check the path!");
            return;
        }

        // Effacer les anciennes entrées
        library.helmets.Clear();
        library.shields.Clear();
        library.boots.Clear();
        library.armors.Clear();
        library.weapons.Clear();

        // Ajouter les nouveaux éléments
        foreach (var newItem in equipmentDataList)
        {
            if (newItem != null)
            {
                // Ajouter à la bonne liste en fonction du type
                switch (newItem.type)
                {
                    case EquipmentManager.Type.Helmet:
                        if (!library.helmets.Contains(newItem))
                        {
                            library.helmets.Add(newItem);
                        }
                        break;
                    case EquipmentManager.Type.Shield:
                        if (!library.shields.Contains(newItem))
                        {
                            library.shields.Add(newItem);
                        }
                        break;
                    case EquipmentManager.Type.Boots:
                        if (!library.boots.Contains(newItem))
                        {
                            library.boots.Add(newItem);
                        }
                        break;
                    case EquipmentManager.Type.Armor:
                        if (!library.armors.Contains(newItem))
                        {
                            library.armors.Add(newItem);
                        }
                        break;
                    case EquipmentManager.Type.Weapon:
                        if (!library.weapons.Contains(newItem))
                        {
                            library.weapons.Add(newItem);
                        }
                        break;
                }
            }
        }

        // Sauvegarder les changements
        EditorUtility.SetDirty(library);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Equipment Library successfully populated from CSV!");
    }

    private static List<EquipmentObject> ReadCSV(string filePath)
    {
        // Charger tous les sprites de l'atlas
        string spriteSheetPath = "Assets/Prefabs/PackCastle01.png";
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheetPath).OfType<Sprite>().ToArray();
        if (sprites.Length == 0)
        {
            Debug.LogError("Aucun sprite trouvé dans la texture PackCastle01.png !");
            return null;
        }

        string[] csvLines = File.ReadAllLines(filePath);
        List<EquipmentObject> equipmentList = new List<EquipmentObject>();

        // Vérifier s'il y a des lignes dans le CSV
        if (csvLines.Length <= 1) return equipmentList;

        // Créer un format de culture pour forcer l'utilisation du séparateur décimal point
        System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");

        for (int i = 0; i < csvLines.Length; i++)
        {
            // name,sprite,type,isEnable,rarity,bonusDamage,bonusDefense,bonusSpeed
            string[] values = csvLines[i].Split(',');

            // Créer un nouveau EquipmentObject
            EquipmentObject equipment = ScriptableObject.CreateInstance<EquipmentObject>();

            // Assignation des valeurs
            equipment.itemName = values[0].Trim();
            equipment.sprite = System.Array.Find(sprites, sprite => sprite.name == values[1].Trim());

            if (equipment.sprite == null)
            {
                Debug.LogWarning($"Sprite '{values[1].Trim()}' introuvable !");
            }

            // Assigner le type et la rareté en utilisant les méthodes de conversion
            equipment.type = EquipmentManager.GetTypeFromStringToEnum(values[2].Trim());
            equipment.isEnable = bool.Parse(values[3].Trim());
            equipment.rarity = EquipmentManager.GetRarityFromStringToEnum(values[4].Trim());

            // Utilisation de TryParse pour éviter l'exception et forcer la culture
            if (float.TryParse(values[5].Trim(), System.Globalization.NumberStyles.Any, cultureInfo, out float bonusDamage))
            {
                equipment.bonusDamage = bonusDamage;
            }
            else
            {
                Debug.LogError($"Erreur de format pour bonusDamage à la ligne {i + 1}: {values[5].Trim()}");
            }

            if (float.TryParse(values[6].Trim(), System.Globalization.NumberStyles.Any, cultureInfo, out float bonusDefense))
            {
                equipment.bonusDefense = bonusDefense;
            }
            else
            {
                Debug.LogError($"Erreur de format pour bonusDefense à la ligne {i + 1}: {values[6].Trim()}");
            }

            if (float.TryParse(values[7].Trim(), System.Globalization.NumberStyles.Any, cultureInfo, out float bonusSpeed))
            {
                equipment.bonusSpeed = bonusSpeed;
            }
            else
            {
                Debug.LogError($"Erreur de format pour bonusSpeed à la ligne {i + 1}: {values[7].Trim()}");
            }

            // Créer un fichier asset pour cet objet
            string assetPath = $"Assets/EquipmentObjects/{equipment.itemName}.asset";
            AssetDatabase.CreateAsset(equipment, assetPath);

            // Ajouter à la liste d'équipements
            equipmentList.Add(equipment);
        }

        return equipmentList;
    }
}