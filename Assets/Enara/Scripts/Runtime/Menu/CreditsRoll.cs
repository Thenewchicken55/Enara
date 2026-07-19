using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Enara.Menu
{
    /// <summary>
    /// Scrolls a credits text from bottom to top, then hides itself. Used for the ending.
    /// Drop this on a Canvas child with a TMP_Text inside a ScrollRect (or a moving TMP).
    /// </summary>
    public sealed class CreditsRoll : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private RectTransform creditsTextTransform;
        [SerializeField] private float scrollSpeed = 60f; // pixels per second
        [SerializeField] private float endYPosition = 1200f;
        [SerializeField] private float startDelay = 1.5f;

        public static CreditsRoll Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (root == null) root = gameObject;
            Hide();
        }

        private void OnDestroy() { if (Instance == this) Instance = null; }

        public void Play()
        {
            if (root != null) root.SetActive(true);
            StartCoroutine(RollRoutine());
        }

        public void Hide()
        {
            if (root != null) root.SetActive(false);
        }

        private IEnumerator RollRoutine()
        {
            if (creditsTextTransform == null) yield break;
            var pos = creditsTextTransform.anchoredPosition;
            pos.y = -endYPosition;
            creditsTextTransform.anchoredPosition = pos;
            yield return new WaitForSeconds(startDelay);
            while (creditsTextTransform.anchoredPosition.y < endYPosition)
            {
                var p = creditsTextTransform.anchoredPosition;
                p.y += scrollSpeed * Time.unscaledDeltaTime;
                creditsTextTransform.anchoredPosition = p;
                yield return null;
            }
        }
    }
}
