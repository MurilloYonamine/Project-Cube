using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

namespace PROJECT_CUBE {
    public class SceneInitializer : MonoBehaviour {
        public enum SceneType { 
            Level,      // Fases do jogo (Level1, Level2, etc)
            Menu        // Menu de seleção de fases
        }
        
        [Header("Scene Settings")]
        [SerializeField] private SceneType _sceneType = SceneType.Level;
        
        [Header("Video Settings")]
        [SerializeField] private VideoClip _customVideo; // Opcional - se quiser vídeo diferente
        [SerializeField] private bool _showVideo = true;

        [SerializeField] private AudioClip _backgroundMusic;
        
        private void Start() {
            InitializeScene();
        }
        
        private void InitializeScene() {
            // Inicializar Música
            InitializeMusic();
            
            // Inicializar Vídeo
            if (_showVideo) {
                InitializeVideo();
            }
        }
        
        private void InitializeMusic() {
            if (AudioManager.Instance == null) return;
            
            switch (_sceneType) {
                case SceneType.Level:
                    AudioManager.Instance.PlayTrack(_backgroundMusic);
                    break;
                case SceneType.Menu:
                    AudioManager.Instance.PlayTrack(_backgroundMusic);
                    break;
            }
        }
        
        private void InitializeVideo() {
            if (BackgroundVideoPlayer.Instance == null) return;
            
            // Se tem vídeo customizado, usa ele
            if (_customVideo != null) {
                BackgroundVideoPlayer.Instance.SetVideo(_customVideo);
            }
            
            BackgroundVideoPlayer.Instance.Show();
            BackgroundVideoPlayer.Instance.PlayVideo();
        }
    }
}
