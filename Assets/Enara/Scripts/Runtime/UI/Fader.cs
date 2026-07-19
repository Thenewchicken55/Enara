using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Enara.UI
{
    /// <summary>
    /// Full-screen black fade used by <see cref="SceneFlow.SceneLoader"/> to mask scene transitions
    /// and by cutscenes for cinematic fades. Lives in the Boot scene as a singleton.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public sealed class Fader : MonoBehaviour
    {
        public static Fader Instance { get; private set; }

        [SerializeField] private Color fadeColor = Color.black;
        [SerializeField] private float startAlpha = 0f;

        private Image _image;
        private Coroutine _routine;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (transform.parent != null && transform.root != null)
                DontDestroyOnLoad(transform.root.gameObject);
            _image = GetComponent<Image>();
            _image.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, startAlpha);
            _image.raycastTarget = false;
        }

        private void OnDestroy() { if (Instance == this) Instance = null; }

        /// <summary>Fade the screen to opaque (black). Yields until finished.</summary>
        public IEnumerator FadeOut(float duration)
        {
            yield return FadeTo(1f, duration);
        }

        /// <summary>Fade the screen from opaque to transparent. Yields until finished.</summary>
        public IEnumerator FadeIn(float duration)
        {
            yield return FadeTo(0f, duration);
        }

        private IEnumerator FadeTo(float targetAlpha, float duration)
        {
            if (duration <= 0f)
            {
                SetAlpha(targetAlpha);
                yield break;
            }
            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(RunFade(targetAlpha, duration));
            yield return _routine;
        }

        private IEnumerator RunFade(float targetAlpha, float duration)
        {
            _image.raycastTarget = true; // block input during the fade
            float startAlpha = _image.color.a;
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, t / duration));
                yield return null;
            }
            SetAlpha(targetAlpha);
            _image.raycastTarget = false;
        }

        private void SetAlpha(float a)
        {
            var c = _image.color;
            c.r = fadeColor.r; c.g = fadeColor.g; c.b = fadeColor.b; c.a = a;
            _image.color = c;
        }
    }
}
