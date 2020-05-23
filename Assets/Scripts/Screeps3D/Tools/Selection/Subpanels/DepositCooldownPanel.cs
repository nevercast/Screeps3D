using System;
using Screeps3D.RoomObjects;
using Screeps_API;
using TMPro;
using UnityEngine;

namespace Screeps3D.Tools.Selection.Subpanels
{
    public class DepositCooldownPanel : LinePanel
    {
        [SerializeField] private TextMeshProUGUI _lastCooldownLabel;
        [SerializeField] private TextMeshProUGUI _cooldownLabel;

        private IDepositCooldown _cooldown;

        public override string Name
        {
            get { return "DepositCooldown"; }
        }

        public override Type ObjectType
        {
            get { return typeof(IDepositCooldown); }
        }

        public override void Load(RoomObject roomObject)
        {
            _cooldown = roomObject as IDepositCooldown;
            UpdateLabel();
            ScreepsAPI.OnTick += OnTick;
        }

        private void OnTick(long obj)
        {
            UpdateLabel();
        }

        public override void Unload()
        {
            _cooldown = null;
            ScreepsAPI.OnTick -= OnTick;
        }

        private void UpdateLabel()
        {
            if (_cooldown.CooldownTime == 0f)
            {
                Hide();
                return;
            }

            // https://github.com/screeps/engine/blob/master/src/game/deposits.js#L36
            _lastCooldownLabel.text = Math.Ceiling(Constants.DEPOSIT_EXHAUST_MULTIPLY * Math.Pow(_cooldown.Harvested, Constants.DEPOSIT_EXHAUST_POW)).ToString();
            var cooldown = _cooldown.CooldownTime - _cooldown.Room.GameTime;
            _cooldownLabel.text = cooldown > 0 ? string.Format("{0:n0}", cooldown) : string.Empty;
        }
    }
}