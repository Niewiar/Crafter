using System;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

namespace Crafter.Inventory
{
    using Item;
    
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private ItemsDatabase _itemsDatabase;
        [SerializeField, Range(1, 100)] private uint _numberOfSlots = 1;

        [ShowInInspector] private ItemStack[] _slots;

        public ItemStack[] Slots
        {
            get
            {
                if (_slots == null)
                {
                    return new ItemStack[_numberOfSlots];
                }

                return _slots;
            }
        }

        public event Action OnInventoryUpdated;

        private void Start()
        {
            _slots = new ItemStack[_numberOfSlots];
            OnInventoryUpdated?.Invoke();
        }

        public virtual uint AddItem(uint p_itemID, uint p_amount)
        {
            ItemScheme scheme = _itemsDatabase.Items.FirstOrDefault(x => x.ID == p_itemID);

            if (scheme == null)
            {
                Debug.LogError($"No item with ID {p_itemID}!!! Adding to {transform.name} inventory failed!");
                return p_amount;
            }

            uint amountToAdd = p_amount;

            //First try add item to already existing same slots
            foreach (var slot in Slots.Where(x => x != null && x.ItemID == p_itemID && x.Stack < scheme.MaxStackAmount))
            {
                if (amountToAdd == 0)
                {
                    OnInventoryUpdated?.Invoke();
                    return 0;
                }
                
                uint addValue = amountToAdd + slot.Stack > scheme.MaxStackAmount ? scheme.MaxStackAmount - slot.Stack : amountToAdd;
                amountToAdd -= addValue;
                slot.Stack += addValue;
            }
            
            //Create new slots
            for (int i = 0; i < Slots.Length; i++)
            {
                if (amountToAdd == 0)
                {
                    OnInventoryUpdated?.Invoke();
                    return 0;
                }

                if (Slots[i] != null)
                {
                    continue;
                }
                
                uint addValue = amountToAdd <= scheme.MaxStackAmount ? amountToAdd : scheme.MaxStackAmount;
                amountToAdd -= addValue;
                ItemStack newSlot = new ItemStack
                {
                    ItemID = p_itemID,
                    Stack = addValue
                };
                Slots[i] = newSlot;
            }

            OnInventoryUpdated?.Invoke();
            return amountToAdd;
        }

        public virtual void RemoveItemFromSlot(uint p_slotNumber, uint p_amount)
        {
            if (p_slotNumber >= Slots.Length || Slots[p_slotNumber] == null)
            {
                return;
            }
            
            if ((int)Slots[p_slotNumber].Stack - p_amount <= 0)
            {
                Slots[p_slotNumber] = null;
            }
            else
            {
                Slots[p_slotNumber].Stack -= p_amount;
            }
            
            OnInventoryUpdated?.Invoke();
        }
        
        public virtual void RemoveItem(uint p_itemID, uint p_amount)
        {
            uint valueToRemove = p_amount;

            for (int i = 0; i < Slots.Length; i++)
            {
                if (valueToRemove == 0)
                {
                    break;
                }
                
                if (Slots[i] == null || Slots[i].ItemID != p_itemID)
                {
                    continue;
                }

                uint removeValue = (int) Slots[i].Stack - valueToRemove < 0 ? Slots[i].Stack : valueToRemove;
                valueToRemove -= removeValue;
                Slots[i].Stack -= removeValue;

                if (Slots[i].Stack == 0)
                {
                    Slots[i] = null;
                }
            }
            
            OnInventoryUpdated?.Invoke();
        }

        public uint GetItemAmount(uint p_itemID)
        {
            uint value = 0;

            foreach (var slot in Slots)
            {
                if (slot == null || slot.ItemID != p_itemID)
                {
                    continue;
                }

                value += slot.Stack;
            }
            
            return value;
        }
    }
}