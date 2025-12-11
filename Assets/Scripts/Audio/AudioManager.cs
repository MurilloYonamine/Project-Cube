using System;
using System.Collections.Generic;
using UnityEngine;

namespace PROJECT_CUBE {
    [Serializable]
    public class Sound {
        public string name;
        public AudioClip clip;
        
        [Range(0f, 1f)]
        public float volume = 1f;
        
        [Range(0.1f, 3f)]
        public float pitch = 1f;
        
        public bool loop = false;
        
        [HideInInspector]
        public AudioSource source;
    }
    
    public class AudioManager : MonoBehaviour {
        private static AudioManager _instance;
        private static bool _isQuitting = false;
        
        public static AudioManager Instance {
            get {
                if (_isQuitting) return null;
                
                if (_instance == null) {
                    _instance = FindObjectOfType<AudioManager>();
                    
                    if (_instance == null) {
                        GameObject go = new GameObject("AudioManager");
                        _instance = go.AddComponent<AudioManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }
        
        [Header("Sound Library")]
        [SerializeField] private List<Sound> _sounds = new List<Sound>();
        
        [Header("Volume Settings")]
        [SerializeField] [Range(0f, 1f)] private float _masterVolume = 1f;
        [SerializeField] [Range(0f, 1f)] private float _sfxVolume = 1f;
        [SerializeField] [Range(0f, 1f)] private float _musicVolume = 1f;
        
        private const string MASTER_VOLUME_KEY = "MasterVolume";
        private const string SFX_VOLUME_KEY = "SFXVolume";
        private const string MUSIC_VOLUME_KEY = "MusicVolume";
        
        private void Awake() {
            if (_instance != null && _instance != this) {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            _isQuitting = false;
            
            // Garante que o GameObject é raiz antes de usar DontDestroyOnLoad
            if (gameObject.transform.parent != null) {
                gameObject.transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject);
            
            InitializeSounds();
            LoadVolumeSettings();
        }
        
        private void OnApplicationQuit() {
            _isQuitting = true;
        }
        
        private void OnDestroy() {
            if (_instance == this) {
                _instance = null;
            }
        }
        
        private void InitializeSounds() {
            if (_sounds.Count == 0) {
                _sounds = new List<Sound> {
                    new Sound { name = "Jump", volume = 0.7f },
                    new Sound { name = "Fall", volume = 0.5f },
                    new Sound { name = "DashFall", volume = 0.6f },
                    new Sound { name = "BlueTerrain", volume = 0.4f, loop = true },
                    new Sound { name = "RedTerrain", volume = 0.4f, loop = true },
                    new Sound { name = "OrangeTerrain", volume = 0.5f },
                    new Sound { name = "Cannon", volume = 0.8f },
                    new Sound { name = "Walk", volume = 0.3f, loop = true },
                    new Sound { name = "TakeDamage", volume = 0.7f },
                    new Sound { name = "ButtonSelect", volume = 0.5f },
                    new Sound { name = "Portal", volume = 0.6f }
                };
            }
            
            foreach (Sound sound in _sounds) {
                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.clip;
                sound.source.volume = sound.volume * _sfxVolume * _masterVolume;
                sound.source.pitch = sound.pitch;
                sound.source.loop = sound.loop;
            }
        }
        
        public void PlaySound(string soundName) {
            Sound sound = _sounds.Find(s => s.name == soundName);
            if (sound == null) {
                PlayerDebugManager.Instance?.AddLine($"Som '{soundName}' não encontrado!", "AudioManager");
                return;
            }
            
            if (sound.source != null && sound.clip != null) {
                sound.source.volume = sound.volume * _sfxVolume * _masterVolume;
                sound.source.Play();
            }
        }
        
        public void StopSound(string soundName) {
            Sound sound = _sounds.Find(s => s.name == soundName);
            if (sound != null && sound.source != null) {
                sound.source.Stop();
            }
        }
        
        public void SetMasterVolume(float volume) {
            _masterVolume = Mathf.Clamp01(volume);
            UpdateAllVolumes();
            SaveVolumeSettings();
        }
        
        public void SetSFXVolume(float volume) {
            _sfxVolume = Mathf.Clamp01(volume);
            UpdateAllVolumes();
            SaveVolumeSettings();
        }
        
        public void SetMusicVolume(float volume) {
            _musicVolume = Mathf.Clamp01(volume);
            UpdateAllVolumes();
            SaveVolumeSettings();
        }
        
        private void UpdateAllVolumes() {
            foreach (Sound sound in _sounds) {
                if (sound.source != null) {
                    sound.source.volume = sound.volume * _sfxVolume * _masterVolume;
                }
            }
        }
        
        private void SaveVolumeSettings() {
            PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, _masterVolume);
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, _sfxVolume);
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, _musicVolume);
            PlayerPrefs.Save();
        }
        
        private void LoadVolumeSettings() {
            _masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);
            _sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
            _musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
            UpdateAllVolumes();
        }
        
        public float GetMasterVolume() => _masterVolume;
        public float GetSFXVolume() => _sfxVolume;
        public float GetMusicVolume() => _musicVolume;
    }
}
