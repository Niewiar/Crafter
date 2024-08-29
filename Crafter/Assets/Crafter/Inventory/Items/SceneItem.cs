using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Crafter.Inventory.Item
{
    using Interactable;
    
    public class SceneItem : InteractableObject
    {
        [HideLabel] public ItemStack ItemData = new();
        
        public override void Interact()
        {
            Destroy(gameObject);
        }
        
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