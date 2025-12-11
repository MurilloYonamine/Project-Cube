using System.Collections.Generic;
using UnityEngine;

namespace PROJECT_CUBE.CARDS {
    public class CardPickup : MonoBehaviour {
        [Header("Card Settings")]
        [SerializeField] private List<CardData> _availableCards;
        [SerializeField] private float _rotationSpeed = 50f;
        
        private bool _hasBeenUsed = false;
        
        private void Start() {
            InitializeCardProbabilities();
        }
        
        private void Update() {
            transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
        }
        
        private void InitializeCardProbabilities() {
            if (_availableCards == null || _availableCards.Count == 0) {
                _availableCards = new List<CardData> {
                    new CardData { type = CardType.Keyframe, probability = 17.5f, displayName = "Keyframe", description = "Double Jump" },
                    new CardData { type = CardType.Array, probability = 15f, displayName = "Array", description = "+1 Vida" },
                    new CardData { type = CardType.Subdivision, probability = 7f, displayName = "Subdivision", description = "Flutua por 3s" },
                    new CardData { type = CardType.Screw, probability = 10f, displayName = "Screw", description = "Pulo maior" },
                    new CardData { type = CardType.Skin, probability = 15f, displayName = "Skin", description = "Pausa avanço 1s" },
                    new CardData { type = CardType.Scale, probability = 8f, displayName = "Scale", description = "Muda tamanho" },
                    new CardData { type = CardType.Remesh, probability = 5f, displayName = "Remesh", description = "Invulnerável 5s" },
                    new CardData { type = CardType.Particle, probability = 17.5f, displayName = "Particle", description = "Velocidade+" },
                    new CardData { type = CardType.Mirror, probability = 5f, displayName = "Mirror", description = "Flip direção" }
                };
            }
        }
        
        private void OnTriggerEnter(Collider other) {
            if (_hasBeenUsed) return;
            
            PLAYER.PlayerController player = other.GetComponent<PLAYER.PlayerController>();
            if (player != null) {
                CardData selectedCard = SelectRandomCard();
                CardBuffManager cardManager = player.GetComponent<CardBuffManager>();
                
                if (cardManager != null) {
                    cardManager.ApplyCard(selectedCard);
                }
                
                _hasBeenUsed = true;
                Destroy(gameObject);
            }
        }
        
        private CardData SelectRandomCard() {
            float totalProbability = 0f;
            foreach (var card in _availableCards) {
                totalProbability += card.probability;
            }
            
            float randomValue = UnityEngine.Random.Range(0f, totalProbability);
            float cumulativeProbability = 0f;
            
            foreach (var card in _availableCards) {
                cumulativeProbability += card.probability;
                if (randomValue <= cumulativeProbability) {
                    return card;
                }
            }
            
            return _availableCards[0];
        }
    }
}
