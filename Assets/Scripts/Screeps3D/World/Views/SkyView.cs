using Assets.Scripts.Common.SettingsManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Screeps3D.World.Views
{
    public class SkyView : MonoBehaviour
    {
        [SerializeField] public Volume _volume;
        [SerializeField] public Light _globalLight;
        [SerializeField] public Gradient _gradient;
        SkySettings _skySettings;
        bool _sunRise;
        bool _sunSet;
        bool _night;
        bool _day;
        float _dayExposition = 1f;
        float _nightExposition = -6f;
        float _expositionChange = 0.001f;

        // day-night lux in range of .04 to 0.01

        [Setting("Gameplay/Day & Night", "Day Brightness", "")]
        static float _dayLux = 0.04f;
        [Setting("Gameplay/Day & Night", "Night Brightness", "")]
        static float _nightLux = 0.01f;

        float _luxChange = 0.0001f;

        // day-night length anywhere between 1 and 1 billion
        [Setting("Gameplay/Day & Night", "Night Length", "")]
        static float _nightLength = 10f;
        [Setting("Gameplay/Day & Night", "Day Length", "")]
        static float _dayLength = 5f;

        float _currentNightProgress = 0f;
        float _currentDayProgress = 0f;
        float _nightProgress = 0.005f;
        float _skyRotation = 0.005f;

        [Setting("Gameplay/Fog", "Fog Attenuation Distance", "")]
        static float _fogAttenuationDistance = 5f;

        [Setting("Gameplay/Fog", "Maximum Height", "")]
        static float _fogMaximumHeight = 15f;

        [Setting("Gameplay/Fog", "Max Fog Distance", "")]
        static float _fogMaxDistance = 0f;

        [Setting("Gameplay/Lighting", "Sun Rotation Speed", "")]
        static float _rotSpeed = 0.01f;

        [Setting("Gameplay/Lighting", "Sun Max Brightness", "")]
        static float _maxSunIntensity = 0.1f;

        [Setting("Gameplay/Lighting", "Ambient Brightness", "")]
        static float _ambientBrightness = 0.01f;


        private Fog _fog;

        void Start()
        {            
            Volume volume = GetComponent<Volume>();
            
            if (volume.profile.TryGet<HDRISky>(out HDRISky tempSkySett))
            {
                _skySettings = tempSkySett;
            }

            if (volume.profile.TryGet<Fog>(out var fog))
            {
                _fog = fog;
                UpdateFogSettings();
            }

            _sunRise = false;
            _day = true;
            _sunSet = false;
            _night = false;
        }

        private void UpdateFogSettings()
        {
            _fog.maximumHeight.value = _fogMaximumHeight;
            _fog.maxFogDistance.value = _fogMaxDistance;
            _fog.meanFreePath.value = _fogAttenuationDistance;
        }

        private void rotateSky() {
            _skySettings.rotation.value += _skyRotation;
            if(_skySettings.rotation.value == 360) {
                _skySettings.rotation.value = 0;
            }
        }

        private void rotateGlobalLight() {
            // tie this value to tick rate somehow   

            float intensity = Mathf.Min(_maxSunIntensity, Mathf.Pow(Mathf.Abs(180 - _globalLight.transform.rotation.eulerAngles.y) * 0.003f, 2)); 
            // from above formula we get states y-rotation:
            // 0 = midday, 180 = midnight, 360 = midday
            // for that we have custom gradient passed as argument with color-ramp aligned to above order
            // 0% = midday, 50% = midnight, 100% = midday
            float state = _globalLight.transform.rotation.eulerAngles.y / 360f;
            _globalLight.color = _gradient.Evaluate(state);

            _globalLight.transform.Rotate(0f, _rotSpeed, 0f, Space.World);
            _globalLight.intensity = intensity;
        }
        void Update()
        {
            rotateGlobalLight();
            
            if ((long)Time.time % 2 != 0) {
                return;
            }
            _skySettings.desiredLuxValue.value = _ambientBrightness;
            rotateSky();
            UpdateFogSettings();
        }
    }
}
