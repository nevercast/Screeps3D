﻿using System;
using Screeps3D.RoomObjects;
using Screeps3D.Tools.Selection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screeps3D.Selection
{
    public class TypePanel : Subpanel
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Button _button;
        private RoomObject _selected;

        public override string Name
        {
            get { return "type"; }
        }

        public override Type ObjectType
        {
            get { return typeof(RoomObject); }
        }

        private void Start()
        {
            _button.onClick.AddListener(OnClick);
        }

        public override void Load(RoomObject roomObject)
        {
            _label.text = roomObject.Type;
            _selected = roomObject;
        }

        public override void Unload()
        {
            _selected = null;
        }

        private void OnClick()
        {
            if (_selected == null) return;
            Tools.Selection.Selection.Instance.DeselectObject(_selected);
        }
    }
}