using Screeps_API;
using System;
using UnityEngine;

namespace Screeps3D.Rooms
{
    public class RoomFactory
    {
        public Room Generate(string roomName, string shardName)
        {
            string xDir, yDir;
            int xCoord, yCoord, index1, index2, shard;

            roomName = roomName.ToUpperInvariant();
            if (roomName.Contains("E"))
            {
                index1 = roomName.IndexOf("E", StringComparison.Ordinal);
                xDir = "E";
            } else if (roomName.Contains("W"))
            {
                index1 = roomName.IndexOf("W", StringComparison.Ordinal);
                xDir = "W";
            } else
            {
                return null;
            }

            if (roomName.Contains("N"))
            {
                index2 = roomName.IndexOf("N", StringComparison.Ordinal);
                yDir = "N";
            } else if (roomName.Contains("S"))
            {
                index2 = roomName.IndexOf("S", StringComparison.Ordinal);
                yDir = "S";
            } else
            {
                return null;
            }

            var parsed = int.TryParse(roomName.Substring(index1 + 1, index2 - index1 - 1), out xCoord);
            if (!parsed)
                return null;
            parsed = int.TryParse(roomName.Substring(index2 + 1), out yCoord);
            if (!parsed)
                return null;

            //parsed = int.TryParse(shardName.Substring(5), out shard);
            //if (!parsed)
            //    return null;

            // TODO: this needs to be redesigned, shardinfo should be fetched during connection, this is a temp fix for allowing seasonal to load rooms.
            var shardLevel = 0;
            var found = false;
            foreach (var key in ScreepsAPI.ShardInfo.ShardInfo.Keys)
            {
                if (key == shardName)
                {
                    found = true;
                    break;
                }

                shardLevel++;
            }

            if (!found)
            {
                // we assume there is only 1 shardlevel on ps if not found
                shardLevel = 0;
            }

            //Debug.LogError($"{roomName} {shardName} shardLevel {shardLevel}");

            return new Room(roomName, shardName, xDir, yDir, shardLevel, xCoord, yCoord);
        }
    }
}