﻿using System;
using Screeps3D.RoomObjects;
using Screeps3D.Tools.Selection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Screeps3D.Selection
{
    public class NamePanel : Subpanel
    {
        [SerializeField] private TextMeshProUGUI _label;

        private INamedObject _selected;

        public override string Name
        {
            get { return "name"; }
        }

        public override Type ObjectType
        {
            get { return typeof(INamedObject); }
        }

        public override void Load(RoomObject roomObject)
        {
            _selected = roomObject as INamedObject;
            _label.text = string.Format("{0}", _selected.Name);
        }

        public override void Unload()
        {
            _selected = null;
        }
    }
}