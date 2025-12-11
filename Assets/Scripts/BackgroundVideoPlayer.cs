using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

namespace PROJECT_CUBE {
    public class BackgroundVideoPlayer : MonoBehaviour {
        private static BackgroundVideoPlayer _instance;
        private static bool _isQuitting = false;
        
        public static BackgroundVideoPlayer Instance {
            get {
                if (_isQuitting) return null;
                
                if (_instance == null) {
                    _instance = FindObjectOfType<BackgroundVideoPlayer>();
                    
                    if (_instance == null) {
                        GameObject go = new GameObject("BackgroundVideoPlayer");
                        _instance = go.AddComponent<BackgroundVideoPlayer>();
                        //DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }
        
        [Header("Video Settings")]
        [SerializeField] private VideoClip _backgroundVideo;
        [SerializeField] private bool _playOnAwake = true;
        
        private VideoPlayer _videoPlayer;
        private RawImage _renderImage;
        private RenderTexture _renderTexture;
        private Canvas _canvas;
        
        private void Awake() {
            if (_instance != null && _instance != this) {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            _isQuitting = false;
            //DontDestroyOnLoad(gameObject);
            
            SetupVideoPlayer();
            
            if (_playOnAwake) {
                PlayVideo();
            }
        }
        
        private void OnApplicationQuit() {
            _isQuitting = true;
        }
        
        private void OnDestroy() {
            if (_instance == this) {
                _instance = null;
            }
            
            if (_renderTexture != null) {
                _renderTexture.Release();
            }
        }
        
        private void SetupVideoPlayer() {
            // Criar Canvas
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = -1000; // Coloca bem atrás
            
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // Criar RawImage para renderizar o vídeo
            GameObject imageObj = new GameObject("VideoDisplay");
            imageObj.transform.SetParent(transform);
            
            _renderImage = imageObj.AddComponent<RawImage>();
            RectTransform rectTransform = _renderImage.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            
            // Criar RenderTexture
            _renderTexture = new RenderTexture(1920, 1080, 0);
            _renderImage.texture = _renderTexture;
            
            // Criar VideoPlayer
            _videoPlayer = gameObject.AddComponent<VideoPlayer>();
            _videoPlayer.playOnAwake = false;
            _videoPlayer.isLooping = true;
            _videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            _videoPlayer.targetTexture = _renderTexture;
            _videoPlayer.audioOutputMode = VideoAudioOutputMode.None; // Sem áudio do vídeo
        }
        
        public void SetVideo(VideoClip clip) {
            _backgroundVideo = clip;
            _videoPlayer.clip = clip;
        }
        
        public void PlayVideo() {
            if (_backgroundVideo != null) {
                _videoPlayer.clip = _backgroundVideo;
            }
            
            if (_videoPlayer.clip != null) {
                _videoPlayer.Play();
                PlayerDebugManager.Instance?.AddLine("Vídeo de fundo iniciado", "BackgroundVideoPlayer");
            }
        }
        
        public void StopVideo() {
            _videoPlayer.Stop();
        }
        
        public void PauseVideo() {
            _videoPlayer.Pause();
        }
        
        public void SetAlpha(float alpha) {
            Color color = _renderImage.color;
            color.a = Mathf.Clamp01(alpha);
            _renderImage.color = color;
        }
        
        public void Show() {
            _canvas.enabled = true;
        }
        
        public void Hide() {
            _canvas.enabled = false;
        }
    }
}
