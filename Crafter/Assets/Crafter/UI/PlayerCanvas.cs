using TMPro;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Crafter.UI
{
    using Inventory.Item;
    using Player;
    
    [RequireComponent(typeof(Canvas))]
    public class PlayerCanvas : MonoBehaviour
    {
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private ItemsDatabase _itemsDatabase;
        [SerializeField] private TextMeshProUGUI _info;
        
        private Camera _camera;
        
        private void Awake()
        {
            _camera = Camera.main;
            GetComponent<Canvas>().worldCamera = _camera;
        }

        private void Update()
        {
            _info.gameObject.SetActive(_playerController.PossibleInteractions.Any());

            if (!_info.gameObject.activeInHierarchy)
            {
                return;
            }

            if (_playerController.PossibleInteractions.First() is SceneItem item)
            {
                ItemScheme itemScheme = _itemsDatabase.Items.First(x => x.ID == item.ItemData.ItemID);
                _info.text = $"{LocalizationSettings.StringDatabase.GetLocalizedString("Gameplay", "take")} {item.ItemData.Stack} {itemScheme.Name.GetLocalizedString()}";
            }
            else
            {
                _info.text = "";
            }
            
            transform.rotation = _camera.transform.rotation;
        }
    }
}