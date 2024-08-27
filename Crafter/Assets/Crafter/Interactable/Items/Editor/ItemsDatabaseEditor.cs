#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Crafter.Interactable.Item
{
    public class ItemsDatabaseEditor : EditorWindow
    {
        private ItemsDatabase _database;
        private readonly Dictionary<Button, ItemScheme> _schemes = new();

        private ScrollView _scrollView;
        private VisualElement _content;
        private VisualTreeAsset _itemButtonsRef;
        
        private Editor _currentEdit;

        [MenuItem("Crafter/Items Database")]
        private static void CreateWindow()
        {
            EditorWindow w = GetWindow<ItemsDatabaseEditor>("Items Database");
            w.minSize = new Vector2(700, 500);
            w.maxSize = new Vector2(700, 1000);
        }

        private void CreateGUI()
        {
            string[] guids = AssetDatabase.FindAssets("Items_Database");

            if (guids.Length == 0)
            {
                rootVisualElement.Add(new Label
                {
                    text = "Items Database not found!!!",
                    style = { color = Color.red }
                });
                return;
            }

            _database = AssetDatabase.LoadAssetAtPath<ItemsDatabase>(AssetDatabase.GUIDToAssetPath(guids[0]));
            
            VisualTreeAsset vt = 
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
                    ("Assets/Crafter/Interactable/Items/Editor/ItemsDatabaseVisualTree.uxml");
            vt.CloneTree(rootVisualElement);
            
            _itemButtonsRef = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
                ("Assets/Crafter/Interactable/Items/Editor/ItemButtonsVisualTree.uxml");

            _scrollView = rootVisualElement.Q<ScrollView>("ScrollView");
            _content = rootVisualElement.Q<VisualElement>("content");
            
            rootVisualElement.Q<Button>("create").clicked += CreateNewItem;
            
            CreateItemsButtons();
        }

        private void OnGUI()
        {
            SetupItemsButtonsNames();
        }

        private void CreateItemsButtons()
        {
            _schemes.Clear();
            _scrollView.Clear();

            foreach (var itemScheme in _database.Items)
            {
                VisualElement itemButtons = _itemButtonsRef.CloneTree();
                
                string name = !itemScheme.Name.IsEmpty ? itemScheme.Name.GetLocalizedString() : "Item";

                Button b = itemButtons.Q<Button>("item");
                b.text = $"{name} (ID: {itemScheme.ID})";
                b.clicked += delegate
                {
                    OpenScheme(itemScheme);
                };
                
                itemButtons.Q<Button>("delete").clicked += delegate
                {
                    _content.Clear();
                    AssetDatabase.RemoveObjectFromAsset(itemScheme);
                    _database.Items.Remove(itemScheme);
                    AssetDatabase.SaveAssets();
                    CreateItemsButtons();
                };
                
                _schemes.Add(b, itemScheme);
                _scrollView.Add(itemButtons);
            }
        }

        private void SetupItemsButtonsNames()
        {
            foreach (var b in _schemes.Keys)
            {
                string name = !_schemes[b].Name.IsEmpty ? _schemes[b].Name.GetLocalizedString() : "Item";
                b.text = $"{name} (ID: {_schemes[b].ID})";
            }
        }

        private void OpenScheme(ItemScheme p_scheme)
        {
            _content.Clear();
            DestroyImmediate(_currentEdit);
            _currentEdit = Editor.CreateEditor(p_scheme);
            IMGUIContainer inspector = new IMGUIContainer();
            inspector.onGUIHandler += delegate
            {
                if (_currentEdit == null)
                {
                    return;
                }
                
                _currentEdit.OnInspectorGUI();
            };
            _content.Add(inspector);
        }

        private void CreateNewItem()
        {
            _content.Clear();
            ItemScheme tempScheme = CreateInstance<ItemScheme>();
            tempScheme.ID = _database.GenerateNewItemID();
            tempScheme.name = $"Item {tempScheme.ID}";
            AssetDatabase.AddObjectToAsset(tempScheme, _database);
            AssetDatabase.SaveAssets();
            
            _database.Items.Add(tempScheme);
            CreateItemsButtons();
            OpenScheme(tempScheme);
        }
    }
}
#endif