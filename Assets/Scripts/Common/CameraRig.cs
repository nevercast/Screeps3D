using Assets.Scripts.Common.SettingsManagement;
using Screeps3D.Menus.Options;
using System;
using UnityEngine;

namespace Common
{
    public class CameraRig : BaseSingleton<CameraRig>
    {
        [SerializeField] private Transform _boom = default;
        [SerializeField] private Transform _pivot = default;
        [SerializeField] private int _rigLayer = default;
        [SerializeField] private float _defaultZoom = default;
        [SerializeField] private float _defaultAngle = default;
        [Setting("Gameplay/Camera", "Mouse Scrolling Zoom Speed", "The speed when scrolling")]
        [SerializeField] private static float _zoomSpeed = 5;
        [SerializeField] private float _minZoom = 1;
        [SerializeField] private float _maxZoom = 400;

        public Action OnTargetReached;

        public static Vector3 Rotation
        {
            get { return Instance._targetRotation; }
            set { Instance._targetRotation = value; }
        }

        public static Vector3 BoomLocalPosition
        {
            get { return Instance._boom.localPosition; }
            set { Instance._boom.localPosition = value; }
        }

        public static Quaternion PivotLocalRotation
        {
            get { return Instance._pivot.localRotation; }
            set { Instance._pivot.localRotation = value; }
        }

        public static Vector3 Position
        {
            get { return Instance._targetPosition; }
            set { Instance._targetPosition = value; }
        }

        public static float Zoom
        {
            get { return Instance._targetZoom; }
            set { Instance._targetZoom = value; }
        }
        
        public static bool ClickToPan { get; set; }
        
        private float _targetZoom;
        private Vector3 _targetRotation;
        private Vector3 _targetPosition;
        private float _zoomRef;
        private Vector3 _posRef;

        [Setting("Gameplay/Camera keyboard control", "Room Keyboard Move Speed(WASD/ArrowKeys)", "The speed when using WASD or arrow keys in room mode (for position)")]
        private static float _keyboardSpeedMove = 5;
        [Setting("Gameplay/Camera keyboard control", "Room Keyboard Rotation Speed(Q/E)", "The speed when using Q/E in room mode (for rotation)")]
        private static float _keyboardSpeedRotation = 5;
        [Setting("Gameplay/Camera keyboard control", "Room Keyboard Rotation Speed(Z/X)", "The speed when using Z/X in room mode (for zoom)")]
        private static float _keyboardSpeedZoom = 5;
        [Setting("Gameplay/Camera keyboard control", "Room Keyboardspeed decay ratio", "Speed decay ratio after you stop keyboard controlling rotation/zoom")]
        private static float _keyboardSpeedmomentum = 0.95f;
        [Setting("Gameplay/Camera keyboard control", "World Keyboard Speed", "The speed when using WASD or arrow keys in world mode")]
        private static float _worldKeyboardSpeed = 5; // TODO: implement

        private Vector3 _clickPos;

        private void Start()
        {
            _targetZoom = _defaultZoom;
            _targetRotation = Vector3.right * _defaultAngle;
        }

        private void Update()
        {
            MouseControl();
            KeyboardControl();
            
            UpdateZoom();
            UpdateRotation();
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (transform.position != _targetPosition)
            {
                transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _posRef, .1f);

                // close enough
                float distanceToTargetPos = (transform.position - _targetPosition).magnitude;
                if (distanceToTargetPos < 10.0f)
                {
                    if (OnTargetReached != null) OnTargetReached.Invoke();
                }
            }
            
        }

        private void UpdateZoom()
        {
            var pos = _boom.localPosition;
            pos.z = Mathf.SmoothDamp(pos.z, -_targetZoom, ref _zoomRef, 0.2f);
            _boom.localPosition = pos;
        }

        private void UpdateRotation()
        {
            _targetRotation.x = Mathf.Clamp(_targetRotation.x, 0, 90);
            var target = Quaternion.Euler(_targetRotation.x, _targetRotation.y, 0);
            // transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime);
            _pivot.rotation = target;
        }

        private void KeyboardControl()
        {
            if (InputMonitor.InputFieldActive)
                return;

            KeyboardPosition();
            KeyboardZoom();
            KeyboardRotation();
        }

        // save the current speen to calculate
        private static float _keyboardZoomPre = 1.0f;
        private static float _keyboardRotatePre = 1.0f;
        // control the rotation/zoom direction
        private static float _keyboardZoomDir = 1.0f;   
        private static float _keyboardRotateDir = 1.0f;

        private void KeyboardPosition()
        {
            var cameraRotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
            var input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            var heightFactor = Mathf.Log10(-_boom.localPosition.z + 1);
            _targetPosition += cameraRotation * input * _keyboardSpeedMove * heightFactor * 5 * Time.deltaTime;
        }

        private void KeyboardZoom()
        {
            // if Z/X is pressed, use dir * speed, else use dir*momentum*speed to slow down smoothly
            if (Input.GetKey(KeyCode.Z)){
                _keyboardZoomPre = _keyboardSpeedZoom;
                _keyboardZoomDir = 1.0f;
            }
            else if(Input.GetKey(KeyCode.X)){
                _keyboardZoomPre = _keyboardSpeedZoom;
                _keyboardZoomDir = -1.0f;
            }
            else{
                _keyboardZoomPre = _keyboardZoomPre * _keyboardSpeedmomentum;
            }
            _targetZoom += 0.003f * _targetZoom / 2 * _keyboardZoomPre * _keyboardZoomDir;
            _targetZoom = Mathf.Clamp(_targetZoom, _minZoom, _maxZoom);
        }

        private void KeyboardRotation()
        {
            // if Q/E is pressed, use dir * speed, else use dir*momentum*speed to slow down smoothly
            if (Input.GetKey(KeyCode.Q)){
                _keyboardRotatePre = _keyboardSpeedRotation;
                _keyboardRotateDir = 1.0f;
            }
            else if(Input.GetKey(KeyCode.E)){
                _keyboardRotatePre = _keyboardSpeedRotation;
                _keyboardRotateDir = -1.0f;
            }
            else{
                _keyboardRotatePre = _keyboardRotatePre * _keyboardSpeedmomentum;
            }
            _targetRotation.y += 5 * Time.deltaTime * _keyboardRotatePre * _keyboardRotateDir;
        }

        private void MouseControl()
        {
            MouseRotation();
            
            if (InputMonitor.OverUI)
                return;
            
            MouseZoom();
            if (ClickToPan)
                MousePosition();
        }

        private void MousePosition()
        {
            if (!Input.GetMouseButton(0))
            {
                _clickPos = Vector3.zero;
                return;
            }
                
            RaycastHit hitInfo;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = Physics.Raycast(ray, out hitInfo, 1000, 1 << _rigLayer);
            if (hit)
            {
                var localPoint = hitInfo.point - transform.position;
                if (_clickPos == Vector3.zero)
                    _clickPos = localPoint;

                if (InputMonitor.IsDragging)
                {
                    var delta = _clickPos - localPoint;
                    _clickPos = localPoint;
                    _targetPosition += delta;
                }
            }
        }

        private void MouseZoom()
        {
            _targetZoom -= Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed * _targetZoom / 2;
            _targetZoom = Mathf.Clamp(_targetZoom, _minZoom, _maxZoom);
        }

        private void MouseRotation()
        {
            if (!Input.GetMouseButton(1))
                return;

            _targetRotation.y += Input.GetAxis("Mouse X") * 5;
            _targetRotation.x -= Input.GetAxis("Mouse Y") * 5;
        }

        public void SetTargetRotation(Vector2 rotation)
        {
            _targetRotation.y += rotation.x;
            _targetRotation.x -= rotation.y;
        }

        public void SetTargetZoom(float targetZoom)
        {
            _targetZoom = Mathf.Clamp(targetZoom, _minZoom, _maxZoom);
        }
    }
}