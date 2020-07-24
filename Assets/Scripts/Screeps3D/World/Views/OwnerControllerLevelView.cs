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

        [SerializeField] private Image _badge;
        [SerializeField] private Image _badgeMask;
        [SerializeField] private TMP_Text _username;

        public void Init(object o)
        {
            this.data = o as OwnerControllerLevelData;
            // TODO: we need the room position and move it there
            //arcRenderer.point1.transform.position = Overlay.LaunchRoom.Position + new Vector3(25, 0, 25); // Center of the room, because we do not know where the nuke is, could perhaps scan for it and correct it?
            this.gameObject.transform.position = data.Room.Position + new Vector3(25, Constants.ShardHeight / 3, 25); // Center of the room

            var user = data.RoomInfo.User;
            if (user != null)
            {
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
                _badgeMask.gameObject.SetActive(false);
                _username.enabled = false;
            }

        }
    }
}
