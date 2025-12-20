using System.Collections;
using UnityEngine;

namespace PROJECT_CUBE.CARDS {
    public class CardUICanvasController : MonoBehaviour {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeTime = 0.15f;
        [Header("Events")]
        [SerializeField] private bool _listenSpinner = true;
        [SerializeField] private bool _listenSkill = false;

        private Coroutine _fadeRoutine;

        private void OnEnable() {
            if (_listenSpinner) CardUIEvents.OnSpinnerVisibilityRequested += OnSpinnerRequested;
            if (_listenSkill) CardUIEvents.OnSkillVisibilityRequested += OnSkillRequested;
        }

        private void OnDisable() {
            if (_listenSpinner) CardUIEvents.OnSpinnerVisibilityRequested -= OnSpinnerRequested;
            if (_listenSkill) CardUIEvents.OnSkillVisibilityRequested -= OnSkillRequested;
        }

        private void OnSpinnerRequested(bool show) {
            StartFade(show);
        }

        private void OnSkillRequested(bool show) {
            StartFade(show);
        }

        private void StartFade(bool show) {
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(FadeRoutine(show));
        }

        private IEnumerator FadeRoutine(bool show) {
            if (_canvasGroup == null) yield break;
            float start = _canvasGroup.alpha;
            float end = show ? 1f : 0f;
            float t = 0f;
            while (t < _fadeTime) {
                t += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(start, end, t / _fadeTime);
                yield return null;
            }
            _canvasGroup.alpha = end;
            _canvasGroup.interactable = show;
            _canvasGroup.blocksRaycasts = show;
            // if we just hid the spinner and this controller listens spinner, notify completion
            if (!show && _listenSpinner) {
                CardUIEvents.OnSpinnerHiddenCompleted?.Invoke();
            }
        }
    }
}
