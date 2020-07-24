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

namespace Assets.Scripts.Screeps3D.World.Views
{
    public class OwnerControllerLevelView : MonoBehaviour, IWorldOverlayViewComponent
    {
        private OwnerControllerLevelData data;
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
            this.gameObject.transform.position = data.Room.Position + new Vector3(25, Constants.ShardHeight / 3, 25); // Center of the room
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
                case 7: scale = 0.8f; break;
                case 6: scale = 0.6f; break;
                case 5: scale = 0.5f; break;
                case 4: scale = 0.4f; break;
                case 3: scale = 0.3f; break;
                case 2: scale = 0.2f; break;
                case 1: scale = 0.1f; break;
                default:
                    break;
            }

            _badgeScaledContent.transform.localScale = new Vector3(scale, scale, 1f);

            if (user != null)
            {
                _roomName.gameObject.SetActive(true);
                _controllerLevel.gameObject.SetActive(true);
                _badgeMask.gameObject.SetActive(true);
                _badge.enabled = true;
                // TODO: should we cache the sprite?
                _badge.sprite = Sprite.Create(user.Badge,
                        new Rect(0.0f, 0.0f, BadgeManager.BADGE_SIZE, BadgeManager.BADGE_SIZE), new Vector2(.5f, .5f));

                _username.enabled = true;
                _username.text = user.Username;
            }
            else
            {
                _roomName.gameObject.SetActive(false);
                _controllerLevel.gameObject.SetActive(false);
                _badgeMask.gameObject.SetActive(false);
                _username.enabled = false;
            }

        }
    }
}
