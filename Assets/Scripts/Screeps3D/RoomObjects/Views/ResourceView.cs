using UnityEngine;
using Common;
using System.Collections.Generic;

namespace Screeps3D.RoomObjects.Views
{
    internal class ResourceView : ObjectView
    {
        [SerializeField] private Renderer _renderer = default;
        [SerializeField] private ScaleAxes _scale = default;

        [SerializeField] private Material[] _materials = default;
        private const int energyMaterialIndex = 0;
        private const int powerMaterialIndex = 1;
        private const int commonMaterialIndex = 2;

        private bool _initialized;
        private Resource _resource;

        internal override void Load(RoomObject roomObject)
        {
            base.Load(roomObject);
            _initialized = false;
            _resource = roomObject as Resource;
            _scale.SetVisibility(1.0f);
        }

        internal override void Delta(JSONObject data)
        {
            base.Delta(data);
        }

        private void Update()
        {
            if (_resource == null)
                return;
            
            if (!_initialized)
            {
                if (_resource.ResourceType.Equals("energy"))
                    _renderer.material = _materials[energyMaterialIndex];
                else if (_resource.ResourceType.Equals("power"))
                    _renderer.material = _materials[powerMaterialIndex];
                else
                    _renderer.material = _materials[commonMaterialIndex];

                _initialized = true;
            }

            if (_resource.ResourceType.Equals("energy"))
                _scale.SetVisibility(0.6f * Mathf.Min(1000.0f, _resource.ResourceAmount) / 1000.0f);
            else
                _scale.SetVisibility(0.92f * Mathf.Min(1500.0f, _resource.ResourceAmount) / 1500.0f);
        }
    }
}