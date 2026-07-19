using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Enara.SceneFlow
{
    /// <summary>
    /// Thin wrapper around <see cref="SceneManager.LoadSceneAsync"/> that:
    /// - lets the <see cref="UI.Fader"/> (if present) fade the screen out before the load
    ///   and back in after.
    /// - reports progress (0..1) so a loading bar can drive off it.
    /// </summary>
    public sealed class SceneLoader : MonoBehaviour
    {
        [SerializeField, Tooltip("If set, fade duration before/after a scene load. 0 = no fade.")]
        private float fadeDuration = 0.6f;

        /// <summary>True while a load operation is in flight.</summary>
        public bool IsLoading { get; private set; }

        /// <summary>Load <paramref name="scenePath"/> by its asset path. Optional callback runs after the new scene is active.</summary>
        public void LoadScene(string scenePath, Action onComplete = null)
        {
            if (IsLoading)
            {
                Debug.LogWarning($"[SceneLoader] Already loading; ignoring request for '{scenePath}'.");
                return;
            }
            StartCoroutine(LoadRoutine(scenePath, onComplete));
        }

        private IEnumerator LoadRoutine(string scenePath, Action onComplete)
        {
            IsLoading = true;
            var fader = UI.Fader.Instance;

            if (fader != null && fadeDuration > 0f)
                yield return fader.FadeOut(fadeDuration);

            var op = SceneManager.LoadSceneAsync(scenePath);
            while (op != null && !op.isDone) yield return null;

            if (fader != null && fadeDuration > 0f)
                yield return fader.FadeIn(fadeDuration);

            IsLoading = false;
            onComplete?.Invoke();
        }
    }
}
