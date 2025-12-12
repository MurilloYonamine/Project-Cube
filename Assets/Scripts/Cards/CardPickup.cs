using System;
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
                PlayerCardInventory inventory = player.GetComponent<PlayerCardInventory>();
                CardData selectedCard = SelectRandomCardExcludingInventory(inventory);
                CardBuffManager cardManager = player.GetComponent<CardBuffManager>();
                _hasBeenUsed = true;

                // Immediately hide visuals and disable interaction so the pickup disappears right away
                HidePickupVisuals();

                // Request UI spinner via event. UI listener should run visual and call the provided callback when done.
                if (CardUIEvents.OnSpinnerStartRequested != null) {
                    CardUIEvents.OnSpinnerStartRequested.Invoke(_availableCards, selectedCard, () => {
                        if (cardManager != null) cardManager.ApplyCard(selectedCard);
                        // add to inventory after application
                        if (inventory != null) inventory.AddCard(selectedCard);
                        Destroy(gameObject);
                    });
                    return;
                }

                // Fallback: no UI listener, apply immediately
                if (cardManager != null) cardManager.ApplyCard(selectedCard);
                if (inventory != null) inventory.AddCard(selectedCard);
                Destroy(gameObject);
            }
        }

        private void HidePickupVisuals() {
            // stop rotating
            _rotationSpeed = 0f;

            // disable all renderers under this object
            var renderers = GetComponentsInChildren<Renderer>(true);
            foreach (var r in renderers) r.enabled = false;

            // disable collider to prevent further triggers
            var col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            // optionally disable particle systems
            var particles = GetComponentsInChildren<ParticleSystem>(true);
            foreach (var p in particles) p.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        
        public CardData SelectRandomCard() {
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

        private CardData SelectRandomCardExcludingInventory(PlayerCardInventory inventory) {
            if (inventory == null) return SelectRandomCard();

            List<CardData> filtered = new List<CardData>();
            foreach (var card in _availableCards) {
                if (card == null) continue;
                if (!inventory.HasCard(card.type)) filtered.Add(card);
            }

            if (filtered.Count == 0) return SelectRandomCard(); // fallback: allow repeats

            float totalProb = 0f;
            foreach (var c in filtered) totalProb += c.probability;
            float randomValue = UnityEngine.Random.Range(0f, totalProb);
            float cumulative = 0f;
            foreach (var c in filtered) {
                cumulative += c.probability;
                if (randomValue <= cumulative) return c;
            }
            return filtered[0];
        }
    }
}
