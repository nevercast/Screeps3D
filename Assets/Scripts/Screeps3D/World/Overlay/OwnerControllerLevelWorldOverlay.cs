using System;
using Assets.Scripts.Screeps3D;
using Screeps3D.Rooms;
using UnityEngine;

namespace Screeps3D.World.Views
{
    // We want an overlay manager of sorts that can initialized room specific overlay and toggle them on / off
    // we need to initialize worldoverlay views for each room,

    public class OwnerControllerLevelWorldOverlay : MonoBehaviour
    {
        // TODO: we will initialize a list of OwnerControllerLevelViewData
        public OwnerControllerLevelWorldOverlay()
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
    }
}