using Common;
using System.Collections.Generic;
using UnityEngine;

namespace Screeps3D.World.Views
{
    public class WorldViewFactory : BaseSingleton<WorldViewFactory>
    {
        private static Transform _parent;

        private const string Path = "Prefabs/WorldView/";

        private Dictionary<string, Stack<WorldOverlay>> _pools = new Dictionary<string, Stack<WorldOverlay>>();

        public static WorldView GetInstance(WorldOverlay overlay)
        {
            if (_parent == null)
            {
                _parent = new GameObject("WorldViews").transform;
            }

            var go = PrefabLoader.Load(string.Format("{0}{1}", Path, overlay.Type), _parent);
            var view = go.GetComponent<WorldView>();
            //view.gameObject.name = overlay.Name;
            //view.transform.localPosition = overlay.Position;
            view.Init(overlay);
            return view;
        }

        private WorldOverlay GetFromPool(string type)
        {
            var pool = GetPool(type);
            if (pool.Count > 0)
            {
                return pool.Pop();
            }
            else
            {
                return null;
            }
        }
        private Stack<WorldOverlay> GetPool(string type)
        {
            if (!_pools.ContainsKey(type))
            {
                _pools[type] = new Stack<WorldOverlay>();
            }
            return _pools[type];
        }

        public void AddToPool(WorldOverlay overlay)
        {
            var pool = GetPool(overlay.Type);
            pool.Push(overlay);
        }

    }
}