using UnityEngine;

namespace Enara.Story
{
    /// <summary>
    /// Drives a "holy light" / angel / divine appearance. Used for the angel that warns the
    /// player in Path 1 ("don't talk to anyone or anything") and for the Virgin Mary who
    /// banishes the Old Man and says "Wake up!".
    ///
    /// The component just toggles a root GameObject on/off and optionally plays a particle
    /// effect + an AudioSource. Trigger via <see cref="Appear"/> / <see cref="Disappear"/> from
    /// a TriggerZone's onEnter event or from a Timeline signal.
    /// </summary>
    public sealed class HolyLightController : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private ParticleSystem particles;
        [SerializeField] private AudioSource ambientSource;
        [SerializeField] private AudioClip appearSfx;
        [SerializeField] private AudioClip disappearSfx;

        private void Awake() { if (root == null) root = gameObject; Hide(); }

        /// <summary>Make the light / angel appear. Plays particle burst + appear SFX.</summary>
        public void Appear()
        {
            if (root != null) root.SetActive(true);
            if (particles != null) particles.Play();
            if (ambientSource != null) ambientSource.Play();
            if (appearSfx != null && Audio.AudioManager.Instance != null) Audio.AudioManager.Instance.PlaySfx(appearSfx);
        }

        /// <summary>Make the light / angel disappear (e.g. Mary "disappears" after wake-up).</summary>
        public void Disappear()
        {
            if (disappearSfx != null && Audio.AudioManager.Instance != null) Audio.AudioManager.Instance.PlaySfx(disappearSfx);
            if (particles != null) particles.Stop();
            if (ambientSource != null) ambientSource.Stop();
            if (root != null) root.SetActive(false);
        }

        public void Hide()
        {
            if (root != null) root.SetActive(false);
        }
    }
}
