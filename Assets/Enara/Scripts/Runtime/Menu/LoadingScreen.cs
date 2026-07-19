using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Enara.Menu
{
    /// <summary>
    /// Loading screen with a progress bar. Use instead of (or alongside) the Fader when a
    /// chapter scene is large and you want to show load progress to the player.
    ///
    /// Wire the <see cref="SceneLoader"/> to call <see cref="Show"/> before load and
    /// <see cref="Hide"/> after.
    /// </summary>
    public sealed class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Slider progressBar;
        [SerializeField] private float fakeMinDuration = 1.5f;

        public static LoadingScreen Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (root == null) root = gameObject;
            Hide();
        }

        private void OnDestroy() { if (Instance == this) Instance = null; }

        public void Show()
        {
            if (root != null) root.SetActive(true);
            if (progressBar != null) progressBar.value = 0f;
        }

        public void SetProgress(float normalized)
        {
            if (progressBar != null) progressBar.value = Mathf.Clamp01(normalized);
        }

        public void Hide()
        {
            if (root != null) root.SetActive(false);
        }

        /// <summary>Convenience: show, wait for a SceneLoader's load to complete, then hide.</summary>
        public IEnumerator ShowUntilLoaded(System.Func<bool> isDone)
        {
            Show();
            float start = Time.realtimeSinceStartup;
            while (!isDone() || (Time.realtimeSinceStartup - start) < fakeMinDuration)
            {
                SetProgress((Time.realtimeSinceStartup - start) / fakeMinDuration);
                yield return null;
            }
            SetProgress(1f);
            Hide();
        }
    }
}
