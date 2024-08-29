using Zenject;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;

namespace Crafter.UI
{
    using Input;
    using Inventory;
    using Inventory.Item;
    using Crafting;
    
    public class PlayerPanel : MonoBehaviour
    {
        [SerializeField, BoxGroup("Base Data")] private InventoryController _playerInventory;
        [SerializeField, BoxGroup("Base Data")] private CraftingController _playerCrafting;
        [SerializeField, BoxGroup("Base Data")] private ItemsDatabase _itemsDatabase;
        [SerializeField, BoxGroup("Inventory")] private InventorySlotUI[] _slots;
        [SerializeField, BoxGroup("Inventory/Slot Options")] private Button _dropAllButton;
        [SerializeField, BoxGroup("Inventory/Slot Options")] private Button _dropOneButton;
        [SerializeField, BoxGroup("Crafting")] private RectTransform _craftButtonsContent;
        [SerializeField, BoxGroup("Crafting")] private CraftButton _craftButtonPrefab;

        private IInputMgr _inputMgr;
        private readonly List<CraftButton> _createdCraftButtons = new();

        [Inject]
        public void Inject(IInputMgr p_inputMgr)
        {
            _inputMgr = p_inputMgr;

            _playerInventory.OnInventoryUpdated += UpdateInventory;

            foreach (var slot in _slots)
            {
                slot.AmountText.transform.parent.gameObject.SetActive(false);
                slot.ICO.gameObject.SetActive(false);
            }
            _dropAllButton.transform.parent.gameObject.SetActive(false);
            
            InitializeCrafting();
        }

        private void OnDestroy()
        {
            _playerInventory.OnInventoryUpdated -= UpdateInventory;
        }
        
        private void UpdateInventory()
        {
            for (uint i = 0; i < _playerInventory.Slots.Length; i++)
            {
                _slots[i].Button.onClick.RemoveAllListeners();
                
                if (_playerInventory.Slots[i] == null)
                {
                    _slots[i].AmountText.transform.parent.gameObject.SetActive(false);
                    _slots[i].ICO.gameObject.SetActive(false);
                    _slots[i].Button.onClick.AddListener(delegate
                    {
                        _dropAllButton.transform.parent.gameObject.SetActive(false);
                    });
                    continue;
                }

                ItemScheme itemScheme =
                    _itemsDatabase.Items.FirstOrDefault(x => x.ID == _playerInventory.Slots[i].ItemID);

                if (itemScheme == null)
                {
                    Debug.LogError($"Item with {_playerInventory.Slots[i].ItemID} not found!");
                    _slots[i].AmountText.transform.parent.gameObject.SetActive(false);
                    _slots[i].ICO.gameObject.SetActive(false);
                    continue;
                }
                
                _slots[i].AmountText.text = _playerInventory.Slots[i].Stack.ToString();
                _slots[i].AmountText.transform.parent.gameObject.SetActive(true);
                
                _slots[i].ICO.sprite = itemScheme.ICO;
                _slots[i].ICO.color = itemScheme.ICOColor;
                _slots[i].ICO.gameObject.SetActive(true);

                uint index = i;
                _slots[i].Button.onClick.AddListener(delegate
                {
                    _dropAllButton.onClick.RemoveAllListeners();
                    _dropAllButton.onClick.AddListener(delegate
                    {
                        SceneItem item = Instantiate(itemScheme.ScenePrefab);
                        item.ItemData.Stack = _playerInventory.Slots[index].Stack;
                        item.transform.position = _playerInventory.transform.position +
                                                  _playerInventory.transform.forward + Vector3.up;
                        
                        _playerInventory.RemoveItemFromSlot(index, _playerInventory.Slots[index].Stack);
                        _dropAllButton.transform.parent.gameObject.SetActive(false);
                    });
                    
                    _dropOneButton.onClick.RemoveAllListeners();
                    _dropOneButton.onClick.AddListener(delegate
                    {
                        SceneItem item = Instantiate(itemScheme.ScenePrefab);
                        item.ItemData.Stack = 1;
                        item.transform.position = _playerInventory.transform.position +
                                                  _playerInventory.transform.forward + Vector3.up;
                        
                        _playerInventory.RemoveItemFromSlot(index, 1);

                        if (_playerInventory.Slots[index] == null)
                        {
                            _dropAllButton.transform.parent.gameObject.SetActive(false);   
                        }
                    });

                    _dropAllButton.transform.parent.transform.position =
                        _slots[index].transform.position + Vector3.down * 5;
                    _dropAllButton.transform.parent.gameObject.SetActive(true);
                });
            }
        }

        private void InitializeCrafting()
        {
            foreach (var item in _itemsDatabase.Items)
            {
                if (item.ItemsToCraft is {Count:>0})
                {
                    CraftButton craftButton = Instantiate(_craftButtonPrefab, _craftButtonsContent);
                    craftButton.ICO.sprite = item.ICO;
                    craftButton.ICO.color = item.ICOColor;
                    craftButton.Name.StringReference = item.Name;
                    CreateCraftInfoAndSetButtonInteractable(craftButton, item.ID);
                    LocalizationSettings.SelectedLocaleChanged += delegate
                    {
                        CreateCraftInfoAndSetButtonInteractable(craftButton, item.ID);
                    };
                    _playerInventory.OnInventoryUpdated += delegate
                    {
                        CreateCraftInfoAndSetButtonInteractable(craftButton, item.ID);
                    };
                        
                    craftButton.Button.onClick.AddListener(delegate
                    {
                        _playerCrafting.Craft(item);
                    });
                        
                    _createdCraftButtons.Add(craftButton);
                }
            }
        }

        private void CreateCraftInfoAndSetButtonInteractable(CraftButton p_craftButton, uint p_itemID)
        {
            ItemScheme itemScheme = _itemsDatabase.Items.FirstOrDefault(x => x.ID == p_itemID);

            if (itemScheme == null)
            {
                p_craftButton.Button.interactable = false;
                p_craftButton.Info.text = $"Can't find item of id {p_itemID}";
                return;
            }

            string text = "";
            bool canCraft = true;
            
            foreach (var item in itemScheme.ItemsToCraft)
            {
                ItemScheme scheme = _itemsDatabase.Items.FirstOrDefault(x => x.ID == item.ItemID);
                
                if (scheme == null)
                {
                    text += $"NULL: {item.ItemID} ";
                    continue;
                }

                if (_playerInventory.GetItemAmount(item.ItemID) < item.Stack)
                {
                    text += $"{scheme.Name.GetLocalizedString()}: <color=red>{item.Stack}</color> ";
                    canCraft = false;
                    continue;
                }
                
                text += $"{scheme.Name.GetLocalizedString()}: <color=white>{item.Stack}</color> ";
            }

            text += $"\n{LocalizationSettings.StringDatabase.GetLocalizedString("Gameplay", "craft_chance")}: {itemScheme.CraftChance}%";
            p_craftButton.Info.text = text;
            p_craftButton.Button.interactable = canCraft;
        }
    }
}

