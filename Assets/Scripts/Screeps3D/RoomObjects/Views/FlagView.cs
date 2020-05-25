using Assets.Scripts.Screeps3D;
using UnityEngine;

namespace Screeps3D.RoomObjects.Views
{
    public class FlagView : MonoBehaviour, IObjectViewComponent
    {
        [SerializeField] private MeshRenderer rend = default;
        private Flag _flag;

        public void Init()
        {
        }

        public void Load(RoomObject roomObject)
        {
            _flag = roomObject as Flag;
        }

        public void Delta(JSONObject data)
        {
        }

        public void Unload(RoomObject roomObject)
        {
            _flag = null;
        }

        private void Update()
        {
            if (_flag == null)
                return;

            var primary = Constants.FlagColors[_flag.PrimaryColor];
            var secondary = Constants.FlagColors[_flag.SecondaryColor];

            var currentPrimaryColor = rend.materials[0].GetColor(ShaderKeys.HDRPLit.Color);
            var currentSecondaryColor = rend.materials[1].GetColor(ShaderKeys.HDRPLit.Color);

            // TODO: compare if we need to update the color
            // https://docs.unity3d.com/ScriptReference/Mathf.Approximately.html

            rend.materials[0].SetColor(ShaderKeys.HDRPLit.Color, Color.Lerp(currentPrimaryColor, primary, Time.deltaTime));
            rend.materials[1].SetColor(ShaderKeys.HDRPLit.Color, Color.Lerp(currentSecondaryColor, secondary, Time.deltaTime));
        }
    }
}