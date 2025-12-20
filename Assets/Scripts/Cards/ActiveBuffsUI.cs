using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace PROJECT_CUBE.CARDS
{
    /// <summary>
    /// Sistema de UI para mostrar buffs ativos no canto inferior direito da tela.
    /// Suporta buffs permanentes e temporários (5s) com countdown.
    /// </summary>
    public class ActiveBuffsUI : MonoBehaviour
    {
        [Header("UI Settings")]
        [SerializeField] private RectTransform buffContainer;
        [SerializeField] private float buffSpacing = 30f;
        [SerializeField] private TMP_FontAsset orbitronFont;

        private Dictionary<string, BuffDisplayItem> activeBuffs = new Dictionary<string, BuffDisplayItem>();
        private Canvas canvas;

        public static ActiveBuffsUI Instance { get; private set; }

        /// <summary>
        /// Garante que a instância existe, criando se necessário.
        /// </summary>
        public static ActiveBuffsUI GetOrCreate()
        {
            if (Instance == null)
            {
                GameObject managerObj = new GameObject("ActiveBuffsUI_Manager");
                Instance = managerObj.AddComponent<ActiveBuffsUI>();
                DontDestroyOnLoad(managerObj);
            }
            return Instance;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateUI();
            LoadFont();
        }

        private void CreateUI()
        {
            // Cria Canvas se não existir
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("ActiveBuffsCanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 100;

                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();

                // Cria container para os buffs (canto inferior direito)
                GameObject containerObj = new GameObject("BuffContainer");
                containerObj.transform.SetParent(canvas.transform, false);

                buffContainer = containerObj.AddComponent<RectTransform>();
                buffContainer.anchorMin = new Vector2(1, 0); // Canto inferior direito
                buffContainer.anchorMax = new Vector2(1, 0);
                buffContainer.pivot = new Vector2(1, 0);
                buffContainer.anchoredPosition = new Vector2(-20, 20); // Offset da borda

                // Layout vertical
                VerticalLayoutGroup layoutGroup = containerObj.AddComponent<VerticalLayoutGroup>();
                layoutGroup.childAlignment = TextAnchor.LowerRight;
                layoutGroup.spacing = buffSpacing;
                layoutGroup.childControlHeight = false;
                layoutGroup.childControlWidth = false;
                layoutGroup.childForceExpandHeight = false;
                layoutGroup.childForceExpandWidth = false;
            }
        }

        private void LoadFont()
        {
            if (orbitronFont == null)
            {
                orbitronFont = Resources.Load<TMP_FontAsset>("Fonts/Orbitron-VariableFont_wght SDF");
                if (orbitronFont == null)
                {
                    Debug.LogError("Fonte Orbitron não encontrada em Resources/Fonts!");
                }
            }
        }

        /// <summary>
        /// Adiciona um buff permanente (dura a cena inteira).
        /// </summary>
        public void AddPermanentBuff(string buffName)
        {
            GetOrCreate(); // Garante que existe
            
            if (activeBuffs.ContainsKey(buffName))
            {
                // Já existe, não adiciona duplicado
                return;
            }

            BuffDisplayItem item = CreateBuffItem(buffName, true, 0f);
            activeBuffs.Add(buffName, item);
        }

        /// <summary>
        /// Adiciona um buff temporário (5 segundos).
        /// </summary>
        public void AddTemporaryBuff(string buffName, float duration = 5f)
        {
            GetOrCreate(); // Garante que existe
            
            if (activeBuffs.ContainsKey(buffName))
            {
                // Reseta o timer se já existir
                activeBuffs[buffName].ResetTimer(duration);
                return;
            }

            BuffDisplayItem item = CreateBuffItem(buffName, false, duration);
            activeBuffs.Add(buffName, item);
        }

        private BuffDisplayItem CreateBuffItem(string buffName, bool isPermanent, float duration)
        {
            GameObject itemObj = new GameObject($"Buff_{buffName}");
            itemObj.transform.SetParent(buffContainer, false);

            TextMeshProUGUI text = itemObj.AddComponent<TextMeshProUGUI>();
            text.font = orbitronFont;
            text.fontSize = 18;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.BottomRight;
            text.enableAutoSizing = false;

            // Adiciona outline para melhor legibilidade
            text.outlineWidth = 0.2f;
            text.outlineColor = Color.black;

            RectTransform rt = text.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(250, 30);

            BuffDisplayItem item = itemObj.AddComponent<BuffDisplayItem>();
            item.Initialize(buffName, text, isPermanent, duration, this);

            return item;
        }

        /// <summary>
        /// Remove um buff da UI.
        /// </summary>
        public void RemoveBuff(string buffName)
        {
            if (activeBuffs.ContainsKey(buffName))
            {
                BuffDisplayItem item = activeBuffs[buffName];
                activeBuffs.Remove(buffName);
                Destroy(item.gameObject);
            }
        }

        /// <summary>
        /// Limpa todos os buffs (útil ao mudar de cena).
        /// </summary>
        public void ClearAllBuffs()
        {
            foreach (var item in activeBuffs.Values)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
            activeBuffs.Clear();
        }

        private void OnDestroy()
        {
            ClearAllBuffs();
        }
    }

    /// <summary>
    /// Componente individual de cada buff exibido na UI.
    /// </summary>
    public class BuffDisplayItem : MonoBehaviour
    {
        private string buffName;
        private TextMeshProUGUI text;
        private bool isPermanent;
        private float remainingTime;
        private ActiveBuffsUI manager;

        public void Initialize(string name, TextMeshProUGUI textComponent, bool permanent, float duration, ActiveBuffsUI managerRef)
        {
            buffName = name;
            text = textComponent;
            isPermanent = permanent;
            remainingTime = duration;
            manager = managerRef;

            UpdateText();
        }

        public void ResetTimer(float duration)
        {
            remainingTime = duration;
            UpdateText();
        }

        private void Update()
        {
            if (isPermanent)
            {
                // Buffs permanentes não mudam
                return;
            }

            // Atualiza countdown
            remainingTime -= Time.deltaTime;

            if (remainingTime <= 0f)
            {
                // Tempo acabou, remove o buff
                manager.RemoveBuff(buffName);
            }
            else
            {
                UpdateText();
            }
        }

        private void UpdateText()
        {
            if (text == null) return;

            if (isPermanent)
            {
                text.text = $"{buffName}: <color=#FFD700>∞</color>"; // Dourado para permanente
            }
            else
            {
                int seconds = Mathf.CeilToInt(remainingTime);
                text.text = $"{buffName}: <color=#00FF00>{seconds}s</color>"; // Verde para temporário
            }
        }
    }
}
