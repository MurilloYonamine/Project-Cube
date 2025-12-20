using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices;

namespace PROJECT_CUBE.LEVEL
{
    /// <summary>
    /// Sistema de "crash" falso para aparecer quando o jogador completa a última fase.
    /// Usa MessageBox nativo do Windows como o DeathZone.
    /// </summary>
    public class FakeCrashSystem : MonoBehaviour
    {
        #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int MessageBox(System.IntPtr hWnd, string text, string caption, uint type);
        #endif

        [Header("Audio")]
        [SerializeField] private string errorSoundPath = "Sounds/dano";

        private bool isRunning = false;
        private Image fadeImage;
        private Canvas fadeCanvas;

        public static FakeCrashSystem Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"FakeCrashSystem: Trigger entered by {other.gameObject.name} with tag {other.tag}");
            
            if (other.CompareTag("Player") && !isRunning)
            {
                Debug.Log("FakeCrashSystem: Starting fake crash!");
                TriggerFakeCrash();
            }
        }

        /// <summary>
        /// Inicia a sequência completa do crash falso.
        /// </summary>
        public void TriggerFakeCrash()
        {
            if (!isRunning)
                StartCoroutine(CrashSequence());
        }

        private IEnumerator CrashSequence()
        {
            isRunning = true;

            // Fade para branco
            yield return StartCoroutine(FadeToWhite());

            // Toca som de erro
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySound(errorSoundPath, volume: 0.7f);

            // Primeira mensagem: "Procurando solução"
            ShowProgressMessage();

            // Pequeno delay para simular processamento
            yield return new WaitForSecondsRealtime(3f);

            // Segunda mensagem: Crash
            ShowCrashMessage();

            // Fade de volta
            yield return StartCoroutine(FadeFromWhite());

            isRunning = false;

            // Fecha o jogo após o crash falso
            Debug.Log("Fake crash finished - Closing game!");
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        private IEnumerator FadeToWhite()
        {
            // Cria canvas overlay para o fade
            if (fadeCanvas == null)
            {
                GameObject canvasObj = new GameObject("FadeCanvas_FakeCrash");
                fadeCanvas = canvasObj.AddComponent<Canvas>();
                fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                fadeCanvas.sortingOrder = 9999;

                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();

                GameObject imageObj = new GameObject("FadeImage");
                imageObj.transform.SetParent(canvasObj.transform, false);

                fadeImage = imageObj.AddComponent<Image>();
                fadeImage.color = new Color(1, 1, 1, 0);

                RectTransform rt = fadeImage.GetComponent<RectTransform>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.sizeDelta = Vector2.zero;
            }

            fadeCanvas.gameObject.SetActive(true);

            float duration = 0.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(0, 1, elapsed / duration);
                fadeImage.color = new Color(1, 1, 1, alpha);
                yield return null;
            }

            fadeImage.color = Color.white;
        }

        private IEnumerator FadeFromWhite()
        {
            if (fadeImage == null) yield break;

            float duration = 0.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(1, 0, elapsed / duration);
                fadeImage.color = new Color(1, 1, 1, alpha);
                yield return null;
            }

            fadeImage.color = new Color(1, 1, 1, 0);
            fadeCanvas.gameObject.SetActive(false);
        }

        private void ShowProgressMessage()
        {
            #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            // MB_OK | MB_ICONINFORMATION = 0x00000000 | 0x00000040 = 0x40
            MessageBox(
                System.IntPtr.Zero,
                "Searching for a solution to the problem...",
                "Blender",
                0x40
            );
            #endif
        }

        private void ShowCrashMessage()
        {
            #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            // MB_OK | MB_ICONERROR = 0x00000000 | 0x00000010 = 0x10
            MessageBox(
                System.IntPtr.Zero,
                "Blender has stopped working\n\n" +
                "A problem has caused the program to stop working correctly.\n" +
                "If you know the steps to reproduce this issue, please submit a bug report.\n\n" +
                "The crash log can be found at:\n" +
                "C:\\Users\\LEGION~1\\AppData\\Local\\..\\blender.crash.txt",
                "Blender",
                0x10
            );
            #endif
        }

        private void OnDestroy()
        {
            // Limpa o fade canvas se existir
            if (fadeCanvas != null)
                Destroy(fadeCanvas.gameObject);
        }
    }
}
