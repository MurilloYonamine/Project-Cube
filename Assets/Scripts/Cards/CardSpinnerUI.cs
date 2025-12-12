using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PROJECT_CUBE.CARDS {
    public class CardSpinnerUI : MonoBehaviour {
        [Header("References")]
        [SerializeField] private CanvasGroup _rootGroup;
        [SerializeField] private Image _displayImage;

        [Header("Spin Settings")]
        [SerializeField] private float _spinDuration = 2f;
        [SerializeField] private float _minInterval = 0.03f;
        [SerializeField] private float _maxInterval = 0.12f;
        [SerializeField] private float _finalDisplayTime = 1.5f;

        private Coroutine _spinRoutine;

        public void StartSpin(List<CardData> availableCards, CardData finalCard, Action onComplete) {
            if (_spinRoutine != null) StopCoroutine(_spinRoutine);
            _spinRoutine = StartCoroutine(SpinRoutine(availableCards, finalCard, onComplete));
        }

        private IEnumerator SpinRoutine(List<CardData> availableCards, CardData finalCard, Action onComplete) {
            if (_rootGroup != null) {
                _rootGroup.interactable = true;
                _rootGroup.blocksRaycasts = true;
            }

            // request UI to show spinner canvasgroup (listeners perform fade)
            CardUIEvents.OnSpinnerVisibilityRequested?.Invoke(true);

            float elapsed = 0f;
            float t = 0f;

            while (elapsed < _spinDuration) {
                // ease interval from fast to slow
                t = elapsed / _spinDuration;
                float interval = Mathf.Lerp(_minInterval, _maxInterval, t);
                // pick random card icon to show
                if (availableCards != null && availableCards.Count > 0) {
                    int idx = UnityEngine.Random.Range(0, availableCards.Count);
                    Sprite s = availableCards[idx].icon;
                    if (s != null && _displayImage != null) _displayImage.sprite = s;
                }

                elapsed += interval;
                yield return new WaitForSeconds(interval);
            }

            // show final card
            if (finalCard != null && _displayImage != null) _displayImage.sprite = finalCard.icon;

            // small pause so player sees result
            yield return new WaitForSeconds(_finalDisplayTime);

            // request UI to hide spinner canvasgroup
            CardUIEvents.OnSpinnerVisibilityRequested?.Invoke(false);

            if (_rootGroup != null) {
                _rootGroup.interactable = false;
                _rootGroup.blocksRaycasts = false;
            }

            // wait for UI hide completed event before activating the card
            bool completed = false;
            Action handler = () => { completed = true; };
            CardUIEvents.OnSpinnerHiddenCompleted += handler;

            // fallback timeout in case no UI listener or event
            float timeout = 1.0f;
            float waited = 0f;
            while (!completed && waited < timeout) {
                waited += Time.deltaTime;
                yield return null;
            }

            CardUIEvents.OnSpinnerHiddenCompleted -= handler;

            onComplete?.Invoke();
            Destroy(gameObject);
        }
    }
}
