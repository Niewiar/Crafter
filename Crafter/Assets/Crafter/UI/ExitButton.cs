using System;
using UnityEngine;
using UnityEngine.UI;

namespace Crafter.UI
{
    [RequireComponent(typeof(Button))]
    public class ExitButton : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(Application.Quit);
        }
    }
}