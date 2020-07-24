using Assets.Scripts.Common.SettingsManagement;
using Common;
using Screeps_API;
using Screeps3D;
using Screeps3D.World.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TODO: fix odd pixelation / white edges on badges

namespace Assets.Scripts.Screeps3D.World.Views
{
    public class OwnerControllerLevelView : MonoBehaviour, IWorldOverlayViewComponent
    {
        private static float overlayHeight = 10f;
        [Setting("Overlay/Owner View", "Camera Height Threshold")]
        public static float overlayCameraHeightThreshold = -250;
        [Setting("Overlay/Owner View", "Camera Angle Threshold")]
        public static float overlayCameraAngleThreshold = 0.25f;

        private OwnerControllerLevelData data;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private GameObject _badgeScaledContent;
        [SerializeField] private Image _badge;
        [SerializeField] private Image _badgeMask;
        [SerializeField] private TMP_Text _username;
        [SerializeField] private TMP_Text _roomName;
        [SerializeField] private TMP_Text _controllerLevel;

        public void Init(object o)
        {
            this.data = o as OwnerControllerLevelData;
            // TODO: we need the room position and move it there
            //arcRenderer.point1.transform.position = Overlay.LaunchRoom.Position + new Vector3(25, 0, 25); // Center of the room, because we do not know where the nuke is, could perhaps scan for it and correct it?
            this.gameObject.transform.position = data.Room.Position + new Vector3(25, overlayHeight, 25); // Center of the room
            _roomName.text = data.RoomInfo.RoomName;
            _controllerLevel.text = data.RoomInfo.Level?.ToString() ?? string.Empty;
            var user = data.RoomInfo.User;
            float scale = 1f;
            int level = data.RoomInfo.Level ?? 1;
            if (data.RoomInfo.IsReserved)
            {
                level = 1;
                //_badge.color.a = 0.5f; // TODO: make it alpha when reserved
            }

            switch (level)
            {
                case 7: scale = 0.9f; break;
                case 6: scale = 0.8f; break;
                case 5: scale = 0.75f; break;
                case 4: scale = 0.7f; break;
                case 3: scale = 0.6f; break;
                case 2: scale = 0.55f; break;
                case 1: scale = 0.5f; break;
                default:
                    break;
            }

            _badgeScaledContent.transform.localScale = new Vector3(scale, scale, 1f);

            // TODO: do zomething based on player height / zoom level

            if (OverlayShouldBeShown(user))
            {
                _canvas.gameObject.SetActive(true);
                // TODO: should we cache the sprite? the badge will exist multiple places, 
                _badge.sprite = Sprite.Create(user.Badge,
                        new Rect(0.0f, 0.0f, BadgeManager.BADGE_SIZE, BadgeManager.BADGE_SIZE), new Vector2(.5f, .5f));

                _username.text = user.Username;
            }
            else
            {
                _canvas.gameObject.SetActive(false);
            }

        }

        private bool OverlayShouldBeShown(ScreepsUser user)
        {
            return user != null && data.Room.Shown;
        }

        private void Update()
        {
            // if pivot.localRotation.x is 0.70 we are looking top down, 0 is at room level / in the ground, we need to determine some sort of treshhold
            // boom.position.z needs to be < - 200
            //Debug.Log(CameraRig.Position);
            

            if (OverlayShouldBeShown(data.RoomInfo.User) && CameraHasCorrectHeight())
            {
                _canvas.gameObject.SetActive(true);
            }
            else
            {

                _canvas.gameObject.SetActive(false);
            }
        }

        private static bool CameraHasCorrectHeight()
        {
            return CameraRig.PivotLocalRotation.x > overlayCameraAngleThreshold && CameraRig.BoomLocalPosition.z < overlayCameraHeightThreshold;
        }

        // TODO: we need an update trigger for when new data is recieved.
    }
}
