using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Crafter.Interactable.Item
{
    public class ItemsDatabase : ScriptableObject
    {
        public List<ItemScheme> Items = new();
        
        internal uint GenerateNewItemID()
        {
            uint id = 0;

            while (Items.Any(x => x.ID == id))
            {
                id++;
            }

            return id;
        }
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Crafter/Database/Items", priority = 0)]
        public static void CreateInstaller()
        {
            string directory = "Assets/";

            if (UnityEditor.Selection.objects.Length > 0)
            {
                directory = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.objects[0]);
            }
            
            ItemsDatabase instance = CreateInstance<ItemsDatabase>();
            instance.name = "Items_Database";
            UnityEditor.AssetDatabase.CreateAsset(instance, $"{directory}/{instance.name}.asset");
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}