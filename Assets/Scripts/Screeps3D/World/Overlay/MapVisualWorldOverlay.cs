using System;
using System.Collections.Generic;
using Assets.Scripts.Screeps3D;
using Assets.Scripts.Screeps3D.World.Views;
using Common;
using Screeps_API;
using Screeps3D.Player;
using Screeps3D.Rooms;
using Screeps3D.Rooms.Views;
using UnityEngine;

namespace Screeps3D.World.Views
{
    // On official, we subscribe to each shard. 
    // Should probably only render visual for selected shard, unless we do intershard visuals? 

    public class MapVisualWorldOverlay : MonoBehaviour
    {
        private Queue<JSONObject> queue = new Queue<JSONObject>();

        private Dictionary<string, WorldView> _views = new Dictionary<string, WorldView>();

        // TODO: we will initialize a list of OwnerControllerLevelViewData
        public MapVisualWorldOverlay()
        {

            // TODO: we want to initialize 1 view, this view is then responsible for world wide rendering
            // inside this view, we might want object pooling, to render "sub views" over each room, in this case an OwnerControllerLevelPrefab for each room
            // we should be able to control how many of theese sub views we render, for example based on distance from player.
            // other kind of world views would be some sort of strategic view, intel, map visuals and such.


            // TODO: for each view, or well actually inside the view
            //_label.text = string.Format("{0}", _selected.Owner.Username);
            //_badge.sprite = Sprite.Create(_selected.Owner.Badge,
            //    new Rect(0.0f, 0.0f, BadgeManager.BADGE_SIZE, BadgeManager.BADGE_SIZE), new Vector2(.5f, .5f));
        }

        private void Start()
        {
            if (ScreepsAPI.Cache.Official)
            {
                // TODO: Handle shard change. 
                ScreepsAPI.Socket.Subscribe(string.Format("mapVisual:{0}/{1}", ScreepsAPI.Me.UserId, PlayerPosition.Instance.ShardName), RecieveData);
            }
            else
            {
                ScreepsAPI.Socket.Subscribe(string.Format("mapVisual:{0}", ScreepsAPI.Me.UserId), RecieveData);
            }
        }

        private void RecieveData(JSONObject data)
        {
            queue.Enqueue(data);
            //Debug.LogError(this.GetInstanceID() + " MapVisuals:RecieveData:" + queue.Count);
        }

        public const float OverlayDistance = 200;


        private enum LookingDirection
        {
            North,
            East,
            South,
            West
        }

        private LookingDirection _lookingDirection = LookingDirection.North;

        private void Update()
        {
            //Debug.LogError(this.GetInstanceID() + "MapVisuals:Update:" + queue.Count);
            if (queue.Count > 0)
            {
                UnpackMapVisuals(queue.Dequeue());
            }

            //Debug.Log(CameraRig.Position);
            //Debug.Log(PlayerPosition.Instance.transform.position);
            //Debug.Log(Vector3.Angle(CameraRig.Position, PlayerPosition.Instance.transform.position));
            //Debug.Log(CameraRig.Rotation);
            // dpending if you rotate left or right, the y value will be negative or positive
            // it increases and increases and increases in either direction
            if (CameraRig.Rotation.y >= -50 && CameraRig.Rotation.y <= 50)
            {
                _lookingDirection = LookingDirection.North;
            }
            else if (CameraRig.Rotation.y >= 50 && CameraRig.Rotation.y <= 145)
            {
                _lookingDirection = LookingDirection.East;
            }
            else if (CameraRig.Rotation.y >= 145 || CameraRig.Rotation.y <= 225)
            {
                _lookingDirection = LookingDirection.South;
            }
            else if (CameraRig.Rotation.y >= 225 || CameraRig.Rotation.y <= 320)
            {
                _lookingDirection = LookingDirection.West;
            }
            //Debug.Log(_lookingDirection);

            //LoadViewsByPlayerPosition();
        }

        private void UnpackMapVisuals(JSONObject data)
        {

            // https://en.wikipedia.org/wiki/JSON_streaming is used for map visuals it appears to be newline delimted.
            // 
            //data = new JSONObject(data.str)

            Debug.LogError("UnpackMapVisuals: " + data.ToString());
            var list = data.str.Replace("\\\"", "\"").Split(new string[] { "}\\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var visual in list)
            {
                var item = new JSONObject(visual);
                Debug.LogError($"{item.ToString()} ");
                var type = item["t"]; // l = line, c = circle, r = rectangle ,p = poly, t = text
                //Debug.LogError($"{type.str} => {item.ToString()} ");
            }


            // TODO: should probably produce a hash of the data to detect if the visual is already present. this would also mean we would have to clean up the visuals that we did not recieve though.
            /* We seem to be recieving a "list" of objects, that are not seperated.
             * 
             * {
	                "t": "l",
	                "x1": 25,
	                "y1": 25,
	                "n1": "W8N1",
	                "x2": 25,
	                "y2": 25,
	                "n2": "W8N2",
	                "s": {
		                "color": "#ff0000",
		                "lineStyle": "dashed"
	                }
                }{
	                "t": "c",
	                "x": 25,
	                "y": 25,
	                "n": "W2N4"
                }{
	                "t": "c",
	                "x": 25,
	                "y": 25,
	                "n": "W8N1",
	                "s": {
		                "fill": "transparent",
		                "radius": 500,
		                "stroke": "#ff0000"
	                }
                }{
	                "t": "r",
	                "x": 13,
	                "y": 2,
	                "n": "W1N4",
	                "w": 11,
	                "h": 11,
	                "s": {
		                "fill": "transparent",
		                "stroke": "#ff0000"
	                }
                }{
	                "t": "p",
	                "points": [{
			                "x": 25,
			                "y": 25,
			                "n": "W8N1"
		                }, {
			                "x": 25,
			                "y": 25,
			                "n": "W8N2"
		                }, {
			                "x": 20,
			                "y": 21,
			                "n": "W1N1"
		                }
	                ],
	                "s": {
		                "fill": "aqua"
	                }
                }{
	                "t": "t",
	                "text": "Target💥",
	                "x": 11,
	                "y": 14,
	                "n": "W2N4",
	                "s": {
		                "color": "#FF0000",
		                "fontSize": 10
	                }
                }

             * */
        }

        ////private void LoadViewsByPlayerPosition()
        ////{
        ////    var playerPosition = PlayerPosition.Instance.transform.position;
        ////    foreach (var col in Physics.OverlapSphere(playerPosition, OverlayDistance, 1 << 10))
        ////    {
        ////        var roomView = col.GetComponent<RoomView>();
        ////        if (roomView == null || roomView.Room == null || Vector3.Distance(playerPosition, roomView.Room.Position) > OverlayDistance)
        ////        {
        ////            continue;
        ////        }

        ////        var roomInfo = MapStatsUpdater.Instance.GetRoomInfo(PlayerPosition.Instance.ShardName, roomView.Room.RoomName);

        ////        if (roomInfo == null)
        ////        {
        ////            continue;
        ////        }

        ////        if (!_views.TryGetValue(roomInfo.RoomName, out var view))
        ////        {
        ////            _views[roomInfo.RoomName] = null; // Add it with a null value to prevent repated queueing

        ////            Scheduler.Instance.Add(() =>
        ////            {
        ////                var room = RoomManager.Instance.Get(roomInfo.RoomName, PlayerPosition.Instance.ShardName);
        ////                var data = new OwnerControllerLevelData(room, roomInfo);
        ////                var o = WorldViewFactory.GetInstance(data);
        ////                o.name = $"{roomInfo.RoomName}:RoomOwnerInfoView";
        ////                _views[roomInfo.RoomName] = o;
        ////            });
        ////        }

        ////        if (view != null)
        ////        {
        ////        // TODO: trigger an update?

        ////        }
        ////    }
        ////}
    }

    ////public class OwnerControllerLevelData : WorldViewData
    ////{
    ////    public OwnerControllerLevelData(Room room, RoomInfo roomInfo)
    ////    {
    ////        Type = "RoomOwnerInfo";
    ////        Room = room;
    ////        RoomInfo = roomInfo;
    ////    }

    ////    public Room Room { get; }
    ////    public RoomInfo RoomInfo { get; }
    ////}
}