using UnityEngine;

namespace Enara.World
{
    /// <summary>
    /// Gradually darkens the environment over a duration. Used in Path 1: "They keep walking.
    /// Screen gets darker and darker" as the Old Man leads the player toward the noose.
    ///
    /// Two layers:
    /// - Fades the global ambient light (RenderSettings.ambientLight) from <see cref="startColor"/>
    ///   to <see cref="endColor"/> over <see cref="durationSeconds"/>.
    /// - Optionally drives a URP Volume weight (post-processing) if you wire one in.
    /// </summary>
    public sealed class MoodLightingController : MonoBehaviour
    {
        [SerializeField] private Color startAmbient = new Color(0.21f, 0.22f, 0.25f, 1f);
        [SerializeField] private Color endAmbient = new Color(0.03f, 0.03f, 0.05f, 1f);
        [SerializeField] private float durationSeconds = 120f;
        [SerializeField] private UnityEngine.Rendering.Volume postProcessVolume;
        [SerializeField] private float startVolumeWeight = 0f;
        [SerializeField] private float endVolumeWeight = 1f;

        private float _elapsed;
        private bool _active;

        /// <summary>Begin the mood transition from start to end.</summary>
        public void BeginTransition() { _elapsed = 0f; _active = true; }

        /// <summary>Snap instantly back to the start (initial) mood.</summary>
        public void ResetToStart()
        {
            _active = false;
            RenderSettings.ambientLight = startAmbient;
            if (postProcessVolume != null) postProcessVolume.weight = startVolumeWeight;
        }

        private void Start()
        {
            RenderSettings.ambientLight = startAmbient;
            if (postProcessVolume != null) postProcessVolume.weight = startVolumeWeight;
        }

        private void Update()
        {
            if (!_active) return;
            _elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsed / durationSeconds);
            RenderSettings.ambientLight = Color.Lerp(startAmbient, endAmbient, t);
            if (postProcessVolume != null)
                postProcessVolume.weight = Mathf.Lerp(startVolumeWeight, endVolumeWeight, t);
            if (t >= 1f) _active = false;
        }
    }
}
