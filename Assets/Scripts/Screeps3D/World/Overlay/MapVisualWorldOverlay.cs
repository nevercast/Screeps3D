using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Scripts.Screeps_API.ConsoleClientAbuse;
using Assets.Scripts.Screeps3D;
using Assets.Scripts.Screeps3D.World.Views;
using Common;
using Screeps_API;
using Screeps3D.Player;
using Screeps3D.Rooms;
using Screeps3D.Rooms.Views;
using Unity.VectorGraphics;
using UnityEditor;
using UnityEngine;

namespace Screeps3D.World.Views
{
    // On official, we subscribe to each shard. 
    // Should probably only render visual for selected shard, unless we do intershard visuals? 

    public class MapVisualWorldOverlay : MonoBehaviour
    {
        private static Shader _lineShader;
        private static Transform _parent;

        private Queue<JSONObject> queue = new Queue<JSONObject>();

        private Dictionary<string, WorldView> _views = new Dictionary<string, WorldView>();

        [SerializeField] private Material _AlphaCutMaterial = default;

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
            _lineShader = Shader.Find("HDRP/Lit");
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

        public const float OverlayHeight = 10;


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
            // debug to allow testing
            if (_parent.transform.childCount > 0)
            {
                return;
            }

            foreach (Transform child in _parent.transform)
            {
                Destroy(child.gameObject);
            }

            // https://en.wikipedia.org/wiki/JSON_streaming is used for map visuals it appears to be newline delimted.
            // 
            //data = new JSONObject(data.str)

            //Debug.LogError("UnpackMapVisuals: " + data.ToString());
            var list = data?.str?.Replace("\\\"", "\"")?.Split(new string[] { "}\\n" }, StringSplitOptions.RemoveEmptyEntries);

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
                        DrawRectangle(item);
                        break;
                    case "p": // poly
                        DrawPoly(item);
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

        private static void DrawPoly(JSONObject item)
        {
            var pointsObject = item["points"];

            var style = item["s"]; // optional
            var fill = style != null ? style["fill"] : null;  // hex color code, default is #ffffff
            var opacity = style != null ? style["opacity"] : null; // number, default is 0.5
            var stroke = style != null ? style["stroke"] : null;  // hex color code, default is undefined (no stroke)
            var strokeWidth = style != null ? style["strokeWidth"] : null; // number, default is 0.5
            var lineStyle = style != null ? style["lineStyle"] : null; // string, undefined = solid line, dashed or dotted, default is undefined.


            var points = new List<Vector3>();

            foreach (var point in pointsObject.list)
            {
                var x = point["x"];
                var y = point["y"];
                var n = point["n"];

                var room = RoomManager.Instance.Get(n.str, PlayerPosition.Instance.ShardName);
                var pos = PosUtility.Convert((int)x.n, (int)y.n, room) + Vector3.up * OverlayHeight;
                points.Add(pos);
            }
            var firstPoint = points.FirstOrDefault();
            points.Add(firstPoint);

            var go = new GameObject($"poly-", typeof(LineRenderer)); // TODO: should probably render it with something else than a line renderer to get support for fill and stroke.
            go.transform.SetParent(_parent);
            //go.transform.position = firstPoint + Vector3.up * OverlayHeight;
            //go.transform.position = room.Position + new Vector3(25, OverlayHeight, 25); // Center of the room;

            var line = go.GetComponent<LineRenderer>();
            line.material = new Material(_lineShader);

            if (stroke == null || !ColorUtility.TryParseHtmlString(stroke.str, out var strokeColor))
            {
                strokeColor = Color.white; // default should be no stroke
            }

            line.startColor = strokeColor;
            line.endColor = strokeColor;

            line.material.SetColor(ShaderKeys.HDRPLit.Color, strokeColor);

            //line.useWorldSpace = false; // make it relative to the gameobject.

            

            line.positionCount = points.Count;
            line.SetPositions(points.ToArray());
        }


        private static void DrawRectangle(JSONObject item)
        {
            var x = item["x"];
            var y = item["y"];
            var n = item["n"];
            var w = item["w"];
            var h = item["h"];

            var room = RoomManager.Instance.Get(n.str, PlayerPosition.Instance.ShardName);
            var pos = PosUtility.Convert((int)x.n, (int)y.n, room);

            var style = item["s"]; // optional
            var fill = style != null ? style["fill"] : null;  // hex color code, default is #ffffff
            var opacity = style != null ? style["opacity"] : null; // number, default is 0.5
            var stroke = style != null ? style["stroke"] : null;  // hex color code, default is undefined (no stroke)
            var strokeWidth = style != null ? style["strokeWidth"] : null; // number, default is 0.5
            var lineStyle = style != null ? style["lineStyle"] : null; // string, undefined = solid line, dashed or dotted, default is undefined.

            var go = new GameObject($"rect-{n.str}-{x.n}-{y.n}", typeof(LineRenderer)); // TODO: should probably render it with something else than a line renderer to get support for fill and stroke.
            go.transform.SetParent(_parent);
            go.transform.position = pos + Vector3.up * OverlayHeight;
            //go.transform.position = room.Position + new Vector3(25, OverlayHeight, 25); // Center of the room;

            var line = go.GetComponent<LineRenderer>();
            line.material = new Material(_lineShader);

            if (stroke == null || !ColorUtility.TryParseHtmlString(stroke.str, out var strokeColor))
            {
                strokeColor = Color.white; // default should be no stroke
            }

            line.startColor = strokeColor;
            line.endColor = strokeColor;

            line.material.SetColor(ShaderKeys.HDRPLit.Color, strokeColor);

            line.useWorldSpace = false; // make it relative to the gameobject.

            var points = new Vector3[5];
            points[0] = new Vector3(0, 0, 0);
            points[1] = new Vector3(w.n, 0, 0);
            points[2] = new Vector3(w.n, 0, -h.n);
            points[3] = new Vector3(0, 0, -h.n);
            points[4] = new Vector3(0, 0, 0);

            line.positionCount = points.Length;
            line.SetPositions(points);
        }

        private void DrawCircle(JSONObject item)
        {
            var x = item["x"];
            var y = item["y"];
            var n = item["n"];

            var room = RoomManager.Instance.Get(n.str, PlayerPosition.Instance.ShardName);
            var pos = PosUtility.Convert((int)x.n, (int)y.n, room);

            var style = item["s"]; // optional
            var radiusObject = style != null ? style["radius"] : null; // number, default is 10
            var fill = style != null ? style["fill"] : null;  // hex color code, default is #ffffff
            var opacity = style != null ? style["opacity"] : null; // number, default is 0.5
            var stroke = style != null ? style["stroke"] : null;  // hex color code, default is undefined (no stroke)
            var strokeWidth = style != null ? style["strokeWidth"] : null; // number, default is 0.5
            var lineStyle = style != null ? style["lineStyle"] : null; // string, undefined = solid line, dashed or dotted, default is undefined.

            var go = new GameObject($"circle-{n.str}-{x.n}-{y.n}-{room.Position.ToString()}", typeof(MeshFilter), typeof(MeshRenderer));
            go.transform.SetParent(_parent);

            //float lineWidth = 1f;
            float radius = radiusObject != null ? radiusObject.n : 10f;
            go.transform.position = pos + new Vector3(-radius, OverlayHeight, -radius);
            // TODO: figure out proper positions

            var w = radius*2;
            var h = radius*2;

            string svgStrokeColor = GetSvgColor(stroke);
            string svgStroke = string.IsNullOrEmpty(svgStrokeColor) ? "" : $" stroke=\"{svgStrokeColor}\"";

            // TODO: stroke-width

            string svgFillColor = GetSvgColor(fill);
            string svgFill = string.IsNullOrEmpty(svgFillColor) ? "" : $" fill=\"{svgFillColor}\"";

            // TODO: we have an odd issue where the spaces between lines gets further and further
            string svgLineStyle = GetSvgLineStyle(lineStyle);

            string xml = $@"<svg height=""{w*2}"" width=""{h*2}"">
              <circle cx=""{radius}"" cy=""{radius}"" r=""{radius}""{svgStroke}{svgFill}{svgLineStyle} />
            </svg>";

            //Debug.LogError(xml);

            var meshFilter = go.GetComponent<MeshFilter>();

            // TODO: generate it in such a way that we can position the quad and it is relative to the position (center i think)
            Mesh mesh = GenerateQuad(w, h);

            meshFilter.mesh = mesh;
            SetTexture(go, xml);
        }

        private void DrawLine(JSONObject item)
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

            var go = new GameObject($"line", typeof(MeshFilter), typeof(MeshRenderer));
            go.transform.SetParent(_parent);

            var w = Math.Abs(pos1.x - pos2.x);
            var h = Math.Abs(pos1.z - pos2.z);
            go.transform.position = room1.Position + new Vector3(-(w) + 25, OverlayHeight, 25);

            string svgHexColor = GetSvgColor(color);

            // TODO: we have an odd issue where the spaces between lines gets further and further
            string svgLineStyle = GetSvgLineStyle(lineStyle);

            string xml = $@"<svg height=""{w}"" width=""{h}"">
              <line x1=""0"" y1=""0"" x2=""{w}"" y2=""{h}"" {svgLineStyle} style=""stroke:{svgHexColor};stroke-width:2"" />
            </svg>";

            //Debug.LogError(xml);

            var meshFilter = go.GetComponent<MeshFilter>();

            Mesh mesh = GenerateQuad(w, h);

            meshFilter.mesh = mesh;
            SetTexture(go, xml);
        }

        private void SetTexture(GameObject go, string xml)
        {
            var texture = Texturize(xml);

            var rend = go.GetComponent<MeshRenderer>();
            var mat = new Material(_AlphaCutMaterial);

            mat.SetTexture(ShaderKeys.HDRPLit.Texture, texture);
            rend.material = mat;
        }

        private static string GetSvgLineStyle(JSONObject lineStyle)
        {
            var svgLineStyle = "";

            if (lineStyle == null)
            {
                return svgLineStyle;
            }

            // TODO: we have an odd issue where dash-array stretches further and further the longer it runs
            switch (lineStyle.str)
            {
                case "dashed":
                    svgLineStyle = " stroke-dasharray=\"5,5\"";
                    break;
                case "dotted":
                    svgLineStyle = " stroke-dasharray=\"2,2\"";
                    break;
            }

            return svgLineStyle;
        }

        private static string GetSvgColor(JSONObject color)
        {
            var svgHexColor = color != null ? color.str : string.Empty;

            if (string.IsNullOrEmpty(svgHexColor))
            {
                // hex color code, default is #ffffff
                return "#ffffff";
            }

            if (!svgHexColor.StartsWith("#") && svgHexColor != "transparent")
            {
                var colorInfo = ColorToHex.Parse(svgHexColor);
                if (colorInfo != null)
                {
                    svgHexColor = colorInfo.Hex;
                }
            }

            return svgHexColor;
        }

        private static Mesh GenerateQuad(float w, float h)
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(0, 0, 0),
                new Vector3(w, 0, 0),
                new Vector3(0, 0, h),
                new Vector3(w, 0, h)
            };
            mesh.vertices = vertices;

            int[] tris = new int[6]
            {
                // lower left triangle
                0, 2, 1,
                // upper right triangle
                2, 3, 1
            };
            mesh.triangles = tris;

            Vector3[] normals = new Vector3[4]
            {
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward
            };
            mesh.normals = normals;

            Vector2[] uv = new Vector2[4]
            {
                  new Vector2(0, 0),
                  new Vector2(1, 0),
                  new Vector2(0, 1),
                  new Vector2(1, 1)
            };
            mesh.uv = uv;
            return mesh;
        }

        private static Texture2D Texturize(string xml)
        {
            // From BadgeManager
            using (var reader = new StringReader(xml))
            {
                var sceneInfo = SVGParser.ImportSVG(reader);

                var geometry = VectorUtils.TessellateScene(sceneInfo.Scene, new VectorUtils.TessellationOptions
                {
                    // https://docs.unity3d.com/Packages/com.unity.vectorgraphics@1.0/manual/index.html
                    // Theese options needs to be set, else it fails
                    StepDistance = 10f,
                    SamplingStepSize = 100f,
                    MaxCordDeviation = 0.5f,
                    MaxTanAngleDeviation = 0.1f
                });

                var sprite = VectorUtils.BuildSprite(geometry, 1, VectorUtils.Alignment.Center, Vector2.zero, 0, true);

                // var mat = new Material(Shader.Find("Unlit/VectorGradient"));
                var mat = new Material(Shader.Find("Unlit/Vector"));

                return VectorUtils.RenderSpriteToTexture2D(sprite, 1024, 1024, mat);
            }
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