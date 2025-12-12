using System;
using System.Collections.Generic;
using UnityEngine;

namespace PROJECT_CUBE.CARDS {
    public class CardUIManager : MonoBehaviour {
        [Header("Spinner Prefab")]
        [SerializeField] private GameObject _spinnerPrefab;
        [Header("Parent Canvas (optional)")]
        [SerializeField] private Canvas _parentCanvas;

        private void OnEnable() {
            CardUIEvents.OnSpinnerStartRequested += OnSpinnerStartRequested;
        }

        private void OnDisable() {
            CardUIEvents.OnSpinnerStartRequested -= OnSpinnerStartRequested;
        }

        private void OnSpinnerStartRequested(List<CardData> availableCards, CardData finalCard, Action onComplete) {
            if (_spinnerPrefab == null) {
                // no prefab assigned: immediately invoke complete
                onComplete?.Invoke();
                return;
            }

            Transform parent = _parentCanvas != null ? _parentCanvas.transform : null;
            GameObject go = Instantiate(_spinnerPrefab, parent);
            CardSpinnerUI spinner = go.GetComponent<CardSpinnerUI>();
            if (spinner != null) {
                spinner.StartSpin(availableCards, finalCard, onComplete);
            } else {
                // no spinner component: cleanup and call complete
                Destroy(go);
                onComplete?.Invoke();
            }
        }
    }
}
