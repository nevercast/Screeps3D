using Screeps3D.RoomObjects.Views;
using UnityEngine;

namespace Screeps3D.RoomObjects
{
    /*
     * https://docs.screeps.com/api/#Deposit
        {
	        "_id": "5ec7fcb6c57ac60118c7508d",
	        "type": "deposit",
	        "depositType": "silicon",
	        "x": 22,
	        "y": 16,
	        "room": "W31N20",
	        "harvested": 7584,
	        "decayTime": 1,
	        850442E+07,
	        "cooldownTime": 1,
	        845446E+07
        }

        Cooldown	0.001 * totalHarvested ^ 1.2	
        Decay	50,000 ticks after appearing or last harvest operation

        We could show the number of times harvested by calculating backwards from harvested and comparing with cooldownTime?
     */
    public class Deposit : Structure, ICooldownObject /*not sure if cooldownTime is the same as cooldown*/, IDecay /*TODO: harvested panel*/
    {
        public string DepositType { get; set; }

        public float NextDecayTime { get; set; }

        public float Cooldown { get; set; }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);

            if (initial)
            {
                var depositType = data["depositType"];
                if (depositType != null)
                {
                    DepositType = depositType.str;
                }

                Initialized = true;
            }

            UnpackUtility.Cooldown(this, data);
            // TODO: Decay is a max of 50k ticks, we can render a progressbar based on this info. and if no "max decay" is specified. it can do what it normally does.
            UnpackUtility.Decay(this, data);
            // CooldownTime seems to behave more like Decay.
            ////var coolDownTimeData = data["cooldownTime"];
            ////if (coolDownTimeData != null)
            ////{
            ////    obj.Cooldown = coolDownTimeData.n;
            ////    return;
            ////}

        }

        protected internal override void AssignView()
        {
            if (Shown && View == null)
            {
                var type = this.Type;

                string alternative = null;

                switch (this.DepositType)
                {
                    case Constants.BaseDeposit.Silicon:
                        alternative = "silicon";
                        break;
                    case Constants.BaseDeposit.Metal:
                        alternative = "metal";
                        break;
                    case Constants.BaseDeposit.Biomass:
                        alternative = "biomass";
                        break;
                    case Constants.BaseDeposit.Mist:
                        alternative = "mist";
                        break;
                }

                if (alternative == null)
                {
                    this.Type = "placeholder";
                }
                else
                {
                    this.Type = $"Deposits/{alternative}";
                }

                View = ObjectViewFactory.Instance.NewView(this);

                this.Type = type;

                if (View)
                {
                    View.Load(this);
                }
            }
        }
    }
}