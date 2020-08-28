using Common;
using Screeps_API;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class RampartView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private Renderer _top = default;
        [SerializeField] private Renderer _left = default;
        [SerializeField] private Renderer _right = default;
        [SerializeField] private Renderer _forward = default;
        [SerializeField] private Renderer _backward = default;
        private Rampart _rampart;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _rampart = roomObject as Rampart;

            var owner = _rampart?.Owner;

            if (_rampart == null)
            {
                var ownedObject = roomObject as IOwnedObject;
                owner = ownedObject?.Owner;
            }

            bool ownedByMe = owner.Username == ScreepsAPI.Me.Username; // TODO: isNPC?;

            // var emissionColor = new Color(1.000f, 0f, 0.297f, 0.053f);
            var emissionColor = new Color32(255, 111, 111, 0); // enemy

            if (ownedByMe)
            {
                // emissionColor = new Color(0f, 1.000f, 0.297f, 0.053f); // me
                emissionColor = new Color32(65, 140, 65, 0); // me
            }

            if (_rampart == null)
            {
                // Should only happen for ruins
                //renderer.material.SetColor("_Color", Color.grey);
                //renderer.material.SetColor("_EmissionColor", Color.grey);
            }
            _top.materials[0].SetColor("EmissionColor", emissionColor);
            _left.materials[0].SetColor("EmissionColor", emissionColor);
            _right.materials[0].SetColor("EmissionColor", emissionColor);
            _forward.materials[0].SetColor("EmissionColor", emissionColor);
            _backward.materials[0].SetColor("EmissionColor", emissionColor);

            var forward = new Vector2(_rampart.X, _rampart.Y - 1);
            var backward = new Vector2(_rampart.X, _rampart.Y + 1);
            var left = new Vector2(_rampart.X - 1, _rampart.Y);
            var right = new Vector2(_rampart.X + 1, _rampart.Y);

            var renderForward = true;
            var renderBackward = true;
            var renderLeft = true;
            var renderRight = true;

            foreach (var o in _rampart?.Room.Objects.Values)
            {
                if (o.Type == Constants.TypeRampart)
                {
                    var position = new Vector2(o.X, o.Y);

                    if (renderForward && position == forward)
                    {
                        renderForward = false;
                    }

                    if (renderBackward && position == backward)
                    {
                        renderBackward = false;
                    }

                    if (renderLeft && position == left)
                    {
                        renderLeft = false;
                    }

                    if (renderRight && position == right)
                    {
                        renderRight = false;
                    }
                }
            }

            _forward.enabled = renderForward;
            _backward.enabled = renderBackward;
            _left.enabled = renderLeft;
            _right.enabled = renderRight;
        }

        public void Delta(JSONObject data)
        {
        }

        public void Unload(RoomObject roomObject)
        {
        }
    }
}