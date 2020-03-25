using System.Collections.Generic;

namespace Screeps3D.RoomObjects
{
    /*{
      "_id": "5a0da460ab17fd00012bf0e9",
      "type": "spawn",
      "room": "E2S7",
      "x": 15,
      "y": 19,
      "name": "Spawn1",
      "user": "5a0da017ab17fd00012bf0e7",
      "energy": 9,
      "energyCapacity": 300,
      "hits": 5000,
      "hitsMax": 5000,
      "spawning": {
        "name": "E2S7s_E1S7_swarmMiner0_60",
        "needTime": 21,
        "remainingTime": 12
      },
      "notifyWhenAttacked": true,
      "off": false
    }*/

    public class Spawn : OwnedStoreStructure, INamedObject //, IEnergyObject
    {
        public string SpawningName { get; set; }
        public float SpawningNeedTime { get; set; }

        public int SpawningSpawnTime { get; set; }
        public string Name { get; set; }

        internal override void Unpack(JSONObject data, bool initial)
        {
            if (data.keys.Count == 0)
            {
                return;
            }

            base.Unpack(data, initial);

            UnpackUtility.Name(this, data);

            if (data.HasField("spawning"))
            {
                var spawningData = data["spawning"];

                if (spawningData.IsNull)
                {
                    SpawningName = null;
                    SpawningNeedTime = 0;
                    SpawningSpawnTime = 0;
                    return;
                }

                if (spawningData.HasField("name"))
                    SpawningName = spawningData["name"].str;

                if (spawningData.HasField("needTime"))
                    SpawningNeedTime = spawningData["needTime"].n;

                if (spawningData.HasField("spawnTime"))
                {
                    SpawningSpawnTime = (int)spawningData["spawnTime"].n;
                }
            }
        }
    }
}