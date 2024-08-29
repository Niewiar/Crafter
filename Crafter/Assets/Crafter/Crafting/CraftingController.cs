using UnityEngine;

namespace Crafter.Crafting
{
    using Inventory;
    using Inventory.Item;
    
    public class CraftingController : MonoBehaviour
    {
        [SerializeField] private InventoryController _destinationInventory;
        [SerializeField] private InventoryController[] _resourcesInventories;

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
                DoSuccessBehaviours();
                return;
            }
            
            DoFailureBehaviours();
        }

        private void DoSuccessBehaviours()
        {
            
        }
        
        private void DoFailureBehaviours()
        {
            
        }
    }
}