using System;
using UnityEngine.InputSystem;

namespace Crafter.Input
{
    public class InputProjectManager : IInputMgr, IDisposable
    {
        private readonly GameControls _controls;
        public GameControls GameControls => _controls;

        private bool _padControl;
        public bool PadControl => _padControl;

        public event Action OnInputChange;

        private InputProjectManager()
        {
            _controls = new GameControls();
            _controls.Enable();

            InputSystem.onActionChange += InputActionChangeCallback;
        }

        public void Dispose()
        {
            _controls.Disable();
            InputSystem.onActionChange -= InputActionChangeCallback;
        }
        
        private void InputActionChangeCallback(object p_obj, InputActionChange p_change)
        {
            if (p_change == InputActionChange.ActionPerformed)
            {
                InputAction receivedInputAction = (InputAction) p_obj;
                InputDevice newDevice = receivedInputAction.activeControl.device;
                _padControl = newDevice is Gamepad;
                OnInputChange?.Invoke();
            }
        }
    }

    public interface IInputMgr
    {
        GameControls GameControls { get; }
        bool PadControl { get; }

        event Action OnInputChange;
    }
}