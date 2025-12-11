using UnityEngine;

namespace PROJECT_CUBE {
    public class MusicManager : MonoBehaviour {
        private static MusicManager _instance;
        private static bool _isQuitting = false;
        
        public static MusicManager Instance {
            get {
                if (_isQuitting) return null;
                
                if (_instance == null) {
                    _instance = FindObjectOfType<MusicManager>();
                    
                    if (_instance == null) {
                        GameObject go = new GameObject("MusicManager");
                        _instance = go.AddComponent<MusicManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }
        
        [Header("Music Tracks")]
        [SerializeField] private AudioClip _levelMusic;
        [SerializeField] private AudioClip _menuMusic;
        
        private AudioSource _audioSource;
        private AudioClip _currentClip;
        
        private void Awake() {
            if (_instance != null && _instance != this) {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            _isQuitting = false;
            DontDestroyOnLoad(gameObject);
            
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.loop = true;
            _audioSource.playOnAwake = false;
        }
        
        private void OnApplicationQuit() {
            _isQuitting = true;
        }
        
        private void OnDestroy() {
            if (_instance == this) {
                _instance = null;
            }
        }
        
        public void PlayLevelMusic() {
            PlayMusic(_levelMusic);
        }
        
        public void PlayMenuMusic() {
            PlayMusic(_menuMusic);
        }
        
        private void PlayMusic(AudioClip clip) {
            if (clip == null) return;
            
            // Se já está tocando a mesma música, não faz nada
            if (_currentClip == clip && _audioSource.isPlaying) return;
            
            _currentClip = clip;
            _audioSource.clip = clip;
            
            // Aplica o volume da música do AudioManager
            if (AudioManager.Instance != null) {
                _audioSource.volume = AudioManager.Instance.GetMusicVolume();
            }
            
            _audioSource.Play();
            
            PlayerDebugManager.Instance?.AddLine($"Tocando música: {clip.name}", "MusicManager");
        }
        
        public void StopMusic() {
            _audioSource.Stop();
            _currentClip = null;
        }
        
        public void SetVolume(float volume) {
            _audioSource.volume = Mathf.Clamp01(volume);
        }
        
        public void UpdateVolume() {
            if (AudioManager.Instance != null) {
                _audioSource.volume = AudioManager.Instance.GetMusicVolume();
            }
        }
    }
}
