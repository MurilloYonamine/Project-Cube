using UnityEngine;
using System.Runtime.InteropServices;

namespace PROJECT_CUBE.LEVEL {
    /// <summary>
    /// Zona de morte que faz o player voltar ao último rewind quando cair.
    /// Adicione este script a um GameObject com Collider (IsTrigger = true).
    /// </summary>
    public class DeathZone : MonoBehaviour {
        #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int MessageBox(System.IntPtr hWnd, string text, string caption, uint type);
        #endif
        
        private float lastAlertTime = -999f;
        private const float ALERT_COOLDOWN = 2f; // Mínimo de 2 segundos entre alertas
        
        private void OnTriggerEnter(Collider other) {
            PLAYER.PlayerController player = other.GetComponent<PLAYER.PlayerController>();
            
            if (player != null && player.PlayerRewind != null) {
                // Verifica se passou tempo suficiente desde o último alerta
                if (Time.time - lastAlertTime < ALERT_COOLDOWN) {
                    PlayerDebugManager.Instance?.AddLine("Alert em cooldown, ignorando...", "DeathZone");
                    return;
                }
                
                lastAlertTime = Time.time;
                PlayerDebugManager.Instance?.AddLine("Player caiu! Fazendo rewind...", "DeathZone");
                
                // Simula o input de rewind
                StartCoroutine(DeathSequence(player));
            }
        }
        
        private System.Collections.IEnumerator DeathSequence(PLAYER.PlayerController player) {
            // Fade para branco
            yield return StartCoroutine(FadeToWhite());
            
            // Mostra alerta do Windows
            ShowWindowsAlert();
            
            // Trigger rewind
            yield return StartCoroutine(TriggerRewindSafely(player));
            
            // Fade de volta
            yield return StartCoroutine(FadeFromWhite());
        }
        
        private UnityEngine.UI.Image fadeImage;
        private Canvas fadeCanvas;
        
        private System.Collections.IEnumerator FadeToWhite() {
            // Cria canvas overlay para o fade
            if (fadeCanvas == null) {
                GameObject canvasObj = new GameObject("FadeCanvas");
                fadeCanvas = canvasObj.AddComponent<Canvas>();
                fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                fadeCanvas.sortingOrder = 9999;
                
                canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                
                GameObject imageObj = new GameObject("FadeImage");
                imageObj.transform.SetParent(canvasObj.transform, false);
                
                fadeImage = imageObj.AddComponent<UnityEngine.UI.Image>();
                fadeImage.color = new Color(1, 1, 1, 0);
                
                RectTransform rt = fadeImage.GetComponent<RectTransform>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.sizeDelta = Vector2.zero;
            }
            
            fadeCanvas.gameObject.SetActive(true);
            
            float duration = 0.3f;
            float elapsed = 0f;
            
            while (elapsed < duration) {
                elapsed += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(0, 1, elapsed / duration);
                fadeImage.color = new Color(1, 1, 1, alpha);
                yield return null;
            }
            
            fadeImage.color = Color.white;
        }
        
        private System.Collections.IEnumerator FadeFromWhite() {
            if (fadeImage == null) yield break;
            
            float duration = 0.3f;
            float elapsed = 0f;
            
            while (elapsed < duration) {
                elapsed += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(1, 0, elapsed / duration);
                fadeImage.color = new Color(1, 1, 1, alpha);
                yield return null;
            }
            
            fadeImage.color = new Color(1, 1, 1, 0);
            fadeCanvas.gameObject.SetActive(false);
        }
        
        private void ShowWindowsAlert() {
            #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            // MB_OK | MB_ICONERROR = 0x00000000 | 0x00000010 = 0x10
            MessageBox(
                System.IntPtr.Zero,
                "A problem caused the player to stop working correctly.\nReturning to last checkpoint...",
                "Player Controller",
                0x10
            );
            #endif
        }
        
        private System.Collections.IEnumerator TriggerRewindSafely(PLAYER.PlayerController player) {
            // Aguarda um frame
            yield return null;
            
            // Verifica se o player ainda existe
            if (player == null || player.PlayerRewind == null) {
                PlayerDebugManager.Instance?.AddLine("Player foi destruído antes do rewind", "DeathZone");
                yield break;
            }
            
            // Volta para o PRIMEIRO checkpoint
            player.PlayerRewind.RewindToFirstCheckpoint();
        }
    }
}
