using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Enara.Story
{
    /// <summary>
    /// README Path 3: "Even among the miracles they see even the pious get tribulations. Even
    /// harder tribulations."
    ///
    /// A miracle is a one-shot dramatic visual event triggered by a Timeline signal, a
    /// TriggerZone, or by code. Each <see cref="MiracleEvent"/> asset defines a name + list of
    /// effects to spawn. Fire it via <see cref="Trigger"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "Miracle", menuName = "Enara/Miracle Event", order = 30)]
    public sealed class MiracleEvent : ScriptableObject
    {
        [SerializeField] private string miracleId;
        [SerializeField, TextArea(2, 6)] private string description;
        [SerializeField] private List<GameObject> effectsToSpawn = new();
        [SerializeField] private Vector3 spawnOffset = Vector3.zero;
        [SerializeField] private AudioClip miracleSfx;
        [SerializeField] private bool saveFlagWhenTriggered = true;

        public string MiracleId => miracleId;
        public string Description => description;

        /// <summary>Trigger this miracle at <paramref name="origin"/>. Saves a flag so it can only fire once.</summary>
        public void Trigger(Transform origin)
        {
            if (saveFlagWhenTriggered && Save.SaveSystem.Instance != null && Save.SaveSystem.Instance.HasFlag($"miracle_{miracleId}"))
                return;

            if (origin != null)
            {
                foreach (var prefab in effectsToSpawn)
                {
                    if (prefab == null) continue;
                    Object.Instantiate(prefab, origin.position + spawnOffset, Quaternion.identity);
                }
            }
            if (miracleSfx != null && Audio.AudioManager.Instance != null)
                Audio.AudioManager.Instance.PlaySfx(miracleSfx);

            if (saveFlagWhenTriggered && Save.SaveSystem.Instance != null)
                Save.SaveSystem.Instance.SetFlag($"miracle_{miracleId}");
        }
    }
}
