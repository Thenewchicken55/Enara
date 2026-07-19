using UnityEngine;

namespace Enara.Story
{
    /// <summary>
    /// Visual markers on the player. Currently used for the README beat "the person gets tatted
    /// with some sigil" when they bow to the idol. Toggle <see cref="ApplySigil"/> from a
    /// dialogue choice's UnityEvent.
    ///
    /// Each marker is a child GameObject (e.g. a skinned decal, a UI overlay, a particle effect)
    /// that gets activated when its corresponding flag is set.
    /// </summary>
    public sealed class PlayerAppearance : MonoBehaviour
    {
        [SerializeField] private GameObject sigilMarker;
        [SerializeField] private GameObject[] additionalMarkers;

        /// <summary>Apply the sigil marker (and any others). Idempotent.</summary>
        public void ApplySigil()
        {
            if (sigilMarker != null) sigilMarker.SetActive(true);
            foreach (var m in additionalMarkers) if (m != null) m.SetActive(true);
            if (Save.SaveSystem.Instance != null) Save.SaveSystem.Instance.SetFlag("has_sigil");
        }

        /// <summary>Remove all markers (e.g. for testing or a special story beat).</summary>
        public void ClearAll()
        {
            if (sigilMarker != null) sigilMarker.SetActive(false);
            foreach (var m in additionalMarkers) if (m != null) m.SetActive(false);
        }

        /// <summary>Restore markers based on saved flags. Call on player spawn.</summary>
        public void RestoreFromSave()
        {
            bool hasSigil = Save.SaveSystem.Instance != null && Save.SaveSystem.Instance.HasFlag("has_sigil");
            if (sigilMarker != null) sigilMarker.SetActive(hasSigil);
        }
    }
}
