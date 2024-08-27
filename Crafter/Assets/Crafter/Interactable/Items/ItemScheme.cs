using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Localization;
using System.Collections.Generic;

namespace Crafter.Interactable.Item
{
    public class ItemScheme : ScriptableObject
    {
        [ReadOnly, BoxGroup("Base Data")] public uint ID;
        [BoxGroup("Base Data")] public LocalizedString Name;
        [BoxGroup("Base Data")] public LocalizedString Dsc;
        [PreviewField, BoxGroup("Base Data")] public Sprite ICO;
        [BoxGroup("Base Data")] public Color ICOColor = Color.white;
        [PreviewField, BoxGroup("Base Data")] public SceneItem ScenePrefab;
        [Range(1, 100), BoxGroup("Base Data")] public uint MaxStackAmount = 10;
        
        [BoxGroup("Crafting")] public List<ItemStack> ItemsToCraft;
        [BoxGroup("Crafting"), Range(1, 100), ShowIf("@ItemsToCraft.Count>0")] public int CraftChance = 1;
        [BoxGroup("Crafting"), Range(1, 100), ShowIf("@ItemsToCraft.Count>0")] public uint ReturnQuality = 50;
    }

    [Serializable]
    public class ItemStack
    {
#if UNITY_EDITOR
        [ValueDropdown(nameof(GetItemsList))]
#endif
        public uint ItemID;
        [Range(0, 100)] public uint Stack = 1;

#if UNITY_EDITOR
        private List<ValueDropdownItem<uint>> GetItemsList()
        {
            List<ValueDropdownItem<uint>> result = new List<ValueDropdownItem<uint>>();

            string[] guids = UnityEditor.AssetDatabase.FindAssets("Items_Database");

            if (guids.Length > 0)
            {
                ItemsDatabase itemsDatabase = UnityEditor.AssetDatabase.LoadAssetAtPath<ItemsDatabase>(
                    UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]));
                foreach (var item in itemsDatabase.Items)
                {
                    result.Add(new ValueDropdownItem<uint>()
                    {
                        Value = item.ID,
                        Text = item.Name.IsEmpty ? $"Item (ID: {item.ID})" : $"{item.Name.GetLocalizedString()} (ID: {item.ID})"
                    });
                }
            }
            
            return result;
        }
#endif
    }
}

