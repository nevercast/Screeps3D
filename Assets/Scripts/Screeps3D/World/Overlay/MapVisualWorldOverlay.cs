using System;
using System.Collections.Generic;
using Assets.Scripts.Screeps3D;
using Assets.Scripts.Screeps3D.World.Views;
using Common;
using Screeps_API;
using Screeps3D.Player;
using Screeps3D.Rooms;
using Screeps3D.Rooms.Views;
using UnityEditor;
using UnityEngine;

namespace Screeps3D.World.Views
{
    // On official, we subscribe to each shard. 
    // Should probably only render visual for selected shard, unless we do intershard visuals? 

    public class MapVisualWorldOverlay : MonoBehaviour
    {
        private static Transform _parent;

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
            _parent = new GameObject("MapVisual").transform;
            _parent.SetParent(this.gameObject.transform);

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

        public const float OverlayHeight = 20;


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
            ////// debug to allow testing
            ////if (_parent.transform.childCount > 0)
            ////{
            ////    return;
            ////}

            foreach (Transform child in _parent.transform)
            {
                Destroy(child.gameObject);
            }

            // https://en.wikipedia.org/wiki/JSON_streaming is used for map visuals it appears to be newline delimted.
            // 
            //data = new JSONObject(data.str)

            //Debug.LogError("UnpackMapVisuals: " + data.ToString());
            var list = data.str.Replace("\\\"", "\"").Split(new string[] { "}\\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var visual in list)
            {
                //Debug.LogError($"visual: {visual}");
                var item = new JSONObject(visual + "}" /* add end of json object back */);
                //Debug.LogError($"{item.ToString()}");
                var type = item["t"]; // l = line, c = circle, r = rectangle ,p = poly, t = text

                // TODO: being able to generate a visual room object is something that should be extracted out and utilized for room visuals as well
                // We might need a general purpose "View" factory, that ObjectViewFactory and WorldViewFactory could inherit from
                switch (type.str)
                {
                    case "l": // line

                        DrawLine(item);

                        break;
                    case "c": // circle
                        DrawCircle(item);

                        break;
                    case "r": // rectangle
                        break;
                    case "p": // poly
                        break;
                    case "t": // text
                        break;
                }
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

        private static void DrawCircle(JSONObject item)
        {
            Debug.LogError("circle: " + item.ToString());
            var x = item["x"];
            var y = item["y"];
            var n = item["n"]; // for some reason n is removed should contain W2N4

            var room = RoomManager.Instance.Get(n.str, PlayerPosition.Instance.ShardName);
            var pos = PosUtility.Convert((int)x.n, (int)y.n, room);

            var style = item["s"]; // optional
            var radiusObject = style != null ? style["radius"] : null; // number, default is 10
            var fill = style != null ? style["fill"] : null;  // hex color code, default is #ffffff
            var opacity = style != null ? style["opacity"] : null; // number, default is 0.5
            var stroke = style != null ? style["stroke"] : null;  // hex color code, default is undefined (no stroke)
            var strokeWidth = style != null ? style["strokeWidth"] : null; // number, default is 0.5
            var lineStyle = style != null ? style["lineStyle"] : null; // string, undefined = solid line, dashed or dotted, default is undefined.

            var go = new GameObject($"circle-{n.str}-{x.n}-{y.n}-{room.Position.ToString()}", typeof(LineRenderer)); // TODO: should probably render it with something else than a line renderer to get support for fill and stroke.
            go.transform.SetParent(_parent);
            go.transform.position = room.Position + new Vector3(25, OverlayHeight, 25); // Center of the room;

            var line = go.GetComponent<LineRenderer>();

            if (stroke == null  || !ColorUtility.TryParseHtmlString(stroke.str, out var strokeColor))
            {
                strokeColor = Color.white; // default should be no stroke
            }

            line.startColor = strokeColor;
            line.endColor = strokeColor;

            //float lineWidth = 1f;
            float radius = radiusObject != null ? radiusObject.n : 10f;

            // Calculate points in circle
            var segments = 360;
            line.useWorldSpace = false; // make it relative to the gameobject.
            //line.startWidth = lineWidth;
            //line.endWidth = lineWidth;
            line.positionCount = segments + 1;

            var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
            var points = new Vector3[pointCount];

            for (int i = 0; i < pointCount; i++)
            {
                var rad = Mathf.Deg2Rad * (i * 360f / segments);
                points[i] = new Vector3((Mathf.Sin(rad) * radius), 0, (Mathf.Cos(rad) * radius));
            }


            line.SetPositions(points);
        }

        private static void DrawLine(JSONObject item)
        {
            var x1 = item["x1"]; // X
            var y1 = item["y1"]; // Y
            var n1 = item["n1"]; // roomName
            var room1 = RoomManager.Instance.Get(n1.str, PlayerPosition.Instance.ShardName);
            var pos1 = PosUtility.Convert((int)x1.n, (int)y1.n, room1) + Vector3.up * OverlayHeight;
            var x2 = item["x2"]; // X
            var y2 = item["y2"]; // Y
            var n2 = item["n2"]; // roomName

            var room2 = RoomManager.Instance.Get(n2.str, PlayerPosition.Instance.ShardName);
            var pos2 = PosUtility.Convert((int)x2.n, (int)y2.n, room2) + Vector3.up * OverlayHeight;
            var style = item["s"]; // optional
            var width = style != null ? style["width"] : null; // number, default is 0.1
            var color = style != null ? style["color"] : null; // hex color code, default is #ffffff
            var opacity = style != null ? style["opacity"] : null; // number, default is 0.5
            var lineStyle = style != null ? style["lineStyle"] : null; // string, undefined = solid line, dashed or dotted, default is undefined.

            var go = new GameObject($"line", typeof(LineRenderer));
            go.transform.SetParent(_parent);
            go.transform.position = room1.Position + new Vector3(25, OverlayHeight, 25); // Center of the room

            var line = go.GetComponent<LineRenderer>();
            //line.useWorldSpace = false; // make it relative to the gameobject.
            line.SetPositions(new Vector3[] { pos1, pos2 });
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