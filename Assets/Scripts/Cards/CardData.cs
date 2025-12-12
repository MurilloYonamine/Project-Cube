using System;
using UnityEngine;

namespace PROJECT_CUBE.CARDS {
    [Serializable]
    public class CardData {
        public CardType type;
        public float probability;
        // duração em segundos (0 = imediato/sem tempo limitado)
        public float duration = 0f;
        // se true, efeito é permanente para a run atual
        public bool isPermanent = false;
        public string displayName;
        public string description;
        public Sprite icon;
    }
}
