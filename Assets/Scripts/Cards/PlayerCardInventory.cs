using System.Collections.Generic;
using UnityEngine;

namespace PROJECT_CUBE.CARDS {
    public class PlayerCardInventory : MonoBehaviour {
        [Header("Owned Cards (current run)")]
        [SerializeField] private List<CardData> _ownedCards = new List<CardData>();

        public IReadOnlyList<CardData> OwnedCards => _ownedCards;

        public bool HasCard(CardType type) {
            foreach (var c in _ownedCards) if (c != null && c.type == type) return true;
            return false;
        }

        public void AddCard(CardData card) {
            if (card == null) return;
            if (HasCard(card.type)) return;
            _ownedCards.Add(card);
        }

        public void Clear() {
            _ownedCards.Clear();
        }
    }
}
