using UnityEngine;

namespace PROJECT_CUBE {
    public class VolumeSlider : MonoBehaviour {
        public enum VolumeType { Master, SFX, Music }
        
        [SerializeField] private VolumeType _volumeType;
        private UnityEngine.UI.Slider _slider;
        
        private void Awake() {
            _slider = GetComponent<UnityEngine.UI.Slider>();
        }
        
        private void Start() {
            if (_slider != null) {
                switch (_volumeType) {
                    case VolumeType.Master:
                        _slider.value = AudioManager.Instance.GetMasterVolume();
                        _slider.onValueChanged.AddListener(OnMasterVolumeChanged);
                        break;
                    case VolumeType.SFX:
                        _slider.value = AudioManager.Instance.GetSFXVolume();
                        _slider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
                        break;
                    case VolumeType.Music:
                        _slider.value = AudioManager.Instance.GetMusicVolume();
                        _slider.onValueChanged.AddListener(OnMusicVolumeChanged);
                        break;
                }
            }
        }
        
        private void OnMasterVolumeChanged(float volume) {
            AudioManager.Instance.SetMasterVolume(volume);
            MusicManager.Instance?.UpdateVolume();
        }
        
        private void OnMusicVolumeChanged(float volume) {
            AudioManager.Instance.SetMusicVolume(volume);
            MusicManager.Instance?.UpdateVolume();
        }
        
        private void OnDestroy() {
            if (_slider != null) {
                _slider.onValueChanged.RemoveAllListeners();
            }
        }
    }
}
