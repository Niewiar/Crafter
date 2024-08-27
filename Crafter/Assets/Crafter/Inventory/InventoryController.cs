using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Crafter.Inventory
{
    using Interactable.Item;
    
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private ItemsDatabase _itemsDatabase;
        [SerializeField, Range(1, 100)] private uint _numberOfSlots = 1;

        [ShowInInspector] private ItemStack[] _slots;
        public IEnumerable<ItemStack> Slots => _slots;

        public event Action OnInventoryUpdated;

        private void Awake()
        {
            _slots = new ItemStack[_numberOfSlots];
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
            foreach (var slot in _slots.Where(x => x != null && x.ItemID == p_itemID && x.Stack < scheme.MaxStackAmount))
            {
                if (amountToAdd == 0)
                {
                    OnInventoryUpdated?.Invoke();
                    return 0;
                }
                
                uint addValue = scheme.MaxStackAmount - slot.Stack;
                amountToAdd -= addValue;
                slot.Stack += addValue;
            }
            
            //Create new slots
            for (int i = 0; i < _slots.Length; i++)
            {
                if (amountToAdd == 0)
                {
                    OnInventoryUpdated?.Invoke();
                    return 0;
                }

                if (_slots[i] != null)
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
                _slots[i] = newSlot;
            }

            OnInventoryUpdated?.Invoke();
            return amountToAdd;
        }

        public virtual uint RemoveItem(uint p_itemID, uint p_amount)
        {
            uint valueToRemove = p_amount;

            for (int i = 0; i < _slots.Length; i++)
            {
                if (valueToRemove == 0)
                {
                    OnInventoryUpdated?.Invoke();
                    return 0;
                }

                if (_slots[i] != null && _slots[i].ItemID == p_itemID)
                {
                    uint removeValue = valueToRemove <= _slots[i].Stack ? valueToRemove : _slots[i].Stack;
                    valueToRemove -= removeValue;
                    _slots[i].Stack -= removeValue;

                    if (_slots[i].Stack == 0)
                    {
                        _slots[i] = null;
                    }
                }
            }

            OnInventoryUpdated?.Invoke();
            return valueToRemove;
        }
    }
}