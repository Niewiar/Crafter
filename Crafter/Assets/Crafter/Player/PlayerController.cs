using System;
using Zenject;
using DG.Tweening;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

namespace Crafter.Player
{
    using Input;

    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField, BoxGroup("Camera")] private Transform _lookAtObj;
        [SerializeField, BoxGroup("Camera"), Range(1,10)] private float _cameraRotationSpeed = 3;
        
        [SerializeField, BoxGroup("Movement"), Range(.1f, 1f)] private float _moveTransitionSpeed = .3f;
        [SerializeField, BoxGroup("Movement"), Range(10, 100)] private float _rotationSpeed = 5f;
        
        private IInputMgr _inputMgr;
        private Animator _animator;

        private Vector2 _currentValue;
        private Vector2 _exceptedValue;
        private Tween _tween;

        private bool _rmb;
        private Vector2 _cameraRotationAxis;

        [Inject]
        public void Inject(IInputMgr p_inputMgr)
        {
            _animator = GetComponent<Animator>();
            
            _inputMgr = p_inputMgr;

            _inputMgr.GameControls.Player.Movement.performed += Move;
            _inputMgr.GameControls.Player.Camera.performed += RotateCamera;
            _inputMgr.GameControls.Player.RMB.performed += EnableRMB;
            _inputMgr.GameControls.Player.RMB.canceled += DisableRMB;
            _inputMgr.GameControls.Player.Interact.performed += Interact;
            _inputMgr.GameControls.Player.Enable();
        }

        private void OnDestroy()
        {
            _inputMgr.GameControls.Player.Disable();
            _inputMgr.GameControls.Player.Movement.performed -= Move;
            _inputMgr.GameControls.Player.Camera.performed -= RotateCamera;
            _inputMgr.GameControls.Player.Interact.performed -= Interact;

            if (_tween != null)
            {
                _tween.onUpdate = null;
                _tween.Kill();
            }
        }

        private void OnTriggerEnter(Collider p_other)
        {
            Debug.Log(p_other.name);
        }
        
        private void OnTriggerExit(Collider p_other)
        {
            Debug.Log(p_other.name);
        }

        private void OnAnimatorMove()
        {
            Vector3 newPos = _lookAtObj.transform.localPosition + Vector3.up * _cameraRotationAxis.y * 
                Time.fixedDeltaTime * _cameraRotationSpeed;
            newPos.x = Mathf.Clamp(newPos.x, -1, 1);
            newPos.y = Mathf.Clamp(newPos.y, -1, 1);
            _lookAtObj.transform.localPosition = newPos;
            
            transform.position = _animator.rootPosition;
            transform.rotation = Quaternion.Euler(transform.eulerAngles + Vector3.up * _cameraRotationAxis.x * 
                Time.fixedDeltaTime * _rotationSpeed);
        }

        private void Move(InputAction.CallbackContext p_ctx)
        {
            if (_exceptedValue == p_ctx.ReadValue<Vector2>())
            {
                return;
            }
            
            _exceptedValue = p_ctx.ReadValue<Vector2>();
            
            if (_tween != null)
            {
                _tween.onUpdate = null;
                _tween.Kill();
            }
            
            _tween = DOTween.To(() => _currentValue, x => _currentValue = x, _exceptedValue, _moveTransitionSpeed);
            _tween.onUpdate += delegate
            {
                _animator.SetFloat("Y_Axis", _currentValue.y);
                _animator.SetFloat("X_Axis", _currentValue.x);
            };
        }
        
        private void RotateCamera(InputAction.CallbackContext p_ctx)
        {
            Debug.Log($"Pad {_inputMgr.PadControl}");
            if (!_inputMgr.PadControl && !_rmb)
            {
                _cameraRotationAxis = Vector2.zero;
                return;
            }

            _cameraRotationAxis = p_ctx.ReadValue<Vector2>().normalized;
        }

        private void Interact(InputAction.CallbackContext p_ctx)
        {
            _animator.SetTrigger("PickUp");
        }

        private void EnableRMB(InputAction.CallbackContext p_ctx) => _rmb = true;
        private void DisableRMB(InputAction.CallbackContext p_ctx) => _rmb = false;
    }
}