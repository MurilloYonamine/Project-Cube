using System;
using UnityEngine;

namespace PROJECT_CUBE.CARDS {
    [Serializable]
    public class CardData {
        public CardType type;
        public float probability;
        
        [Header("Duração do Buff")]
        // Tipo de duração do buff
        public BuffDurationType durationType = BuffDurationType.Temporary;
        // duração em segundos (só usado se durationType == Temporary)
        public float duration = 5f;
        
        public string displayName;
        public string description;
        public Sprite icon;
    }
    
    public enum BuffDurationType
    {
        Temporary,  // Dura X segundos (padrão 5s)
        Permanent   // Dura a cena inteira
    }
}
