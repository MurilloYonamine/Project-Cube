using System;
using UnityEngine;

namespace PROJECT_CUBE.CARDS {
    [Serializable]
    public class CardData {
        public CardType type;
        public float probability;
        public string displayName;
        public string description;
        public Sprite icon;
    }
}
