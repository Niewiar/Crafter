using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;

namespace Crafter.UI
{
    public class LanguageChanger : MonoBehaviour
    {
        [SerializeField] private Locale _polish;
        [SerializeField] private Locale _english;
        [SerializeField] private TMP_Dropdown _dropdown;

        private void Awake()
        {
            _dropdown.options = new List<TMP_Dropdown.OptionData>()
            {
                new ("English"),
                new ("Polski")
            };
            
            _dropdown.onValueChanged.AddListener(delegate(int v)
            {
                switch (v)
                {
                    case 1:
                        LocalizationSettings.SelectedLocale = _polish;
                        break;
                    default:
                        LocalizationSettings.SelectedLocale = _english;
                        break;
                }    
            });
            
            LocalizationSettings.SelectedLocale = _english;
        }
    }
}