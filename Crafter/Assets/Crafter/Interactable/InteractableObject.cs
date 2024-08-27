using UnityEngine;

namespace Crafter.Interactable
{
    [RequireComponent(typeof(Collider))]
    public abstract class InteractableObject : MonoBehaviour
    {
        public abstract void Interact();
    }
}