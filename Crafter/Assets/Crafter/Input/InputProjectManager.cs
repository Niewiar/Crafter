using System;

namespace Crafter.Input
{
    public class InputProjectManager : IInputMgr, IDisposable
    {
        private readonly GameControls _controls;
        public GameControls GameControls => _controls;

        private InputProjectManager()
        {
            _controls = new GameControls();
            _controls.Enable();
        }

        public void Dispose()
        {
            _controls.Disable();
        }
    }

    public interface IInputMgr
    {
        GameControls GameControls { get; }
    }
}