using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Crafter.Crafting
{
    using Inventory;
    using Inventory.Item;
    
    public class CraftingController : MonoBehaviour
    {
        [SerializeField, BoxGroup("Base Data")] private InventoryController _destinationInventory;
        [SerializeField, BoxGroup("Base Data")] private InventoryController[] _resourcesInventories;

        [BoxGroup("Events")] public UnityEvent OnCraftSuccess;
        [BoxGroup("Events")] public UnityEvent OnCraftFailure;

        public void Craft(ItemScheme p_item)
        {
            foreach (var item in p_item.ItemsToCraft)
            {
                uint amount = item.Stack;
                
                foreach (var inventory in _resourcesInventories)
                {
                    uint possibleAmount = inventory.GetItemAmount(item.ItemID);
                    uint x = possibleAmount >= amount ? amount : possibleAmount;
                    amount -= x;
                    inventory.RemoveItem(item.ItemID, x);
                }   
            }

            int random = Random.Range(0, 100);

            if (random <= p_item.CraftChance)
            {
                _destinationInventory.AddItem(p_item.ID, 1);
                OnCraftSuccess?.Invoke();
                return;
            }
            
            OnCraftFailure?.Invoke();
        }
    }
}