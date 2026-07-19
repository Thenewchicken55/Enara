using System.Collections;
using UnityEngine;
using TMPro;

namespace Enara.UI
{
    /// <summary>
    /// Single-line subtitle display (used for dialogue text and the Jesus Prayer words).
    /// Call <see cref="Show(string, float)"/> to display text, optionally auto-hiding after a delay.
    /// </summary>
    public sealed class Subtitles : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TMP_Text label;

        private Coroutine _hideRoutine;

        private void Reset() { if (root == null) root = gameObject; }
        private void Awake() { if (root == null) root = gameObject; Hide(); }

        /// <summary>Show text. If <paramref name="duration"/> > 0, hide after that many seconds.</summary>
        public void Show(string text, float duration = 0f)
        {
            if (label != null) label.text = text;
            if (root != null) root.SetActive(true);
            if (_hideRoutine != null) StopCoroutine(_hideRoutine);
            if (duration > 0f) _hideRoutine = StartCoroutine(HideAfter(duration));
        }

        public void Hide()
        {
            if (root != null) root.SetActive(false);
            if (_hideRoutine != null) { StopCoroutine(_hideRoutine); _hideRoutine = null; }
        }

        private IEnumerator HideAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Hide();
        }
    }
}
