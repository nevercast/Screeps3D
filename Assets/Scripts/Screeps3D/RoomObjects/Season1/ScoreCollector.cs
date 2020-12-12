using System.Collections.Generic;
using System.Diagnostics;

namespace Screeps3D.RoomObjects
{
    /*{
        "_id":"59469528473155f701d2e6d1",
        "type":"scoreCollector",
    }*/

    public class ScoreCollector : StoreStructure
    {
        public ScoreCollector()
        {
            OverrideTypePath = "Season1/";
        }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);
        }
    }
}