using System.Collections;
using UnityEngine;

namespace Enara.Cutscene
{
    /// <summary>
    /// Applies a procedural shake to a Camera. Use for the car crash, dramatic moments, the
    /// noose wake-up. Call <see cref="Shake(float, float)"/> from any code, or wire it to a
    /// SignalReceiver's UnityEvent.
    /// </summary>
    public sealed class CameraShake : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;
        [SerializeField] private float defaultDuration = 0.4f;
        [SerializeField] private float defaultMagnitude = 0.3f;
        [SerializeField] private AnimationCurve falloff = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

        private Coroutine _routine;
        private Vector3 _originalLocalPos;

        private void Awake()
        {
            if (targetCamera == null) targetCamera = GetComponentInChildren<Camera>(true);
            if (targetCamera != null) _originalLocalPos = targetCamera.transform.localPosition;
        }

        /// <summary>Shake the camera with default settings. Safe to call from anywhere.</summary>
        public void Shake() => Shake(defaultMagnitude, defaultDuration);

        /// <summary>Shake the camera with explicit magnitude (in meters) and duration (seconds).</summary>
        public void Shake(float magnitude, float duration)
        {
            if (targetCamera == null) return;
            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(ShakeRoutine(magnitude, duration));
        }

        private IEnumerator ShakeRoutine(float magnitude, float duration)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float k = falloff.Evaluate(t / duration);
                var offset = Random.insideUnitSphere * magnitude * k;
                targetCamera.transform.localPosition = _originalLocalPos + offset;
                yield return null;
            }
            targetCamera.transform.localPosition = _originalLocalPos;
            _routine = null;
        }
    }
}
