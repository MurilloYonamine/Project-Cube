using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDebugManager : MonoBehaviour {
    public static PlayerDebugManager Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI _debugText;
    private readonly Dictionary<string, int> _debugSlots = new Dictionary<string, int>();
    private readonly List<string> _debugMessages = new List<string>();
    private int _nextSlotIndex = 0;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public void AddLine(string message, string classIdentifier) {
        if (_debugText == null) {
            Debug.LogWarning("Componente TextMeshProUGUI não atribuído no PlayerDebugManager.");
            return;
        }

        if (!_debugSlots.ContainsKey(classIdentifier)) {
            _debugSlots[classIdentifier] = _nextSlotIndex; // aloca
            _debugMessages.Add(message);
            _nextSlotIndex++;
        } else {
            int slotIndex = _debugSlots[classIdentifier]; // atualiza
            _debugMessages[slotIndex] = message;
        }

        _debugText.text = string.Join("\n", _debugMessages);
    }

    public void ClearAll() {
        _debugSlots.Clear();
        _debugMessages.Clear();
        _nextSlotIndex = 0;
        _debugText.text = "";
    }
}
