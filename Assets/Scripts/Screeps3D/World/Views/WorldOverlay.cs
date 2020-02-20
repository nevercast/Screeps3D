using Common;

namespace Screeps3D.World.Views
{
    // Q: Will we be using this for WorldMap overlays?
    public class WorldOverlay
    {
        public virtual string Type { get; set; }
        //public bool Shown { get; private set; }

        public WorldView View { get; private set; }

        public WorldOverlay()
        {
            Scheduler.Instance.Add(AssignView); // Should probably be done based on Show, like Room, this would allow us to show / hide objects based on how far a player can actually see and possible help with performance.
        }

        protected internal virtual void AssignView()
        {
            View = WorldViewFactory.GetInstance(this);
        }

        private void OnFinishedAnimation(bool isVisible)
        {
            if (isVisible)
                return;

            ////foreach (var component in components)
            ////{
            ////    component.Unload(RoomObject);
            ////}

            ////RoomObject.OnShow -= Show;
            ////RoomObject.DetachView();
            View = null;
            WorldViewFactory.Instance.AddToPool(this);
            ////RoomObject = null;
        }
    }
}