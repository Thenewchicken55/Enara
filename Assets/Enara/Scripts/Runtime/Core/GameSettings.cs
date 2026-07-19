using UnityEngine;

namespace Enara.Core
{
    /// <summary>
    /// Global tunable settings for the whole game. One asset is created under
    /// Assets/Enara/Settings/GameSettings.asset and referenced via <c>Resources.Load</c>
    /// or via the Inspector on <see cref="GameManager"/>.
    ///
    /// Add fields here for any cross-system tunable - mixing volumes, QTE timings, etc.
    /// Keep it small; this is not a dumping ground.
    /// </summary>
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Enara/GameSettings", order = -100)]
    public sealed class GameSettings : ScriptableObject
    {
        [Header("QTE (Jesus Prayer)")]
        [SerializeField, Min(0.1f)] private float qteWindowSeconds = 1.2f;
        [SerializeField, Min(0.1f)] private float qteMinInterval = 4f;
        [SerializeField, Min(0.1f)] private float qteMaxInterval = 8f;
        [SerializeField, Tooltip("How many QTEs must succeed before the player is 'healed' and QTEs stop.")]
        private int qtesUntilHealed = 8;

        [Header("Player")]
        [SerializeField, Min(0.1f)] private float walkSpeed = 1.6f;
        [SerializeField, Min(0.1f)] private float limpSpeed = 0.9f;
        [SerializeField, Min(0.1f)] private float lookSensitivity = 0.05f;

        [Header("Audio")]
        [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float musicVolume = 0.7f;
        [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

        public float QteWindowSeconds => qteWindowSeconds;
        public float QteMinInterval => qteMinInterval;
        public float QteMaxInterval => qteMaxInterval;
        public int QtesUntilHealed => qtesUntilHealed;
        public float WalkSpeed => walkSpeed;
        public float LimpSpeed => limpSpeed;
        public float LookSensitivity => lookSensitivity;
        public float MasterVolume => masterVolume;
        public float MusicVolume => musicVolume;
        public float SfxVolume => sfxVolume;

        /// <summary>Default asset path used by <see cref="Resources.Load"/>.</summary>
        public const string ResourcesPath = "GameSettings";

        /// <summary>Load the shared settings asset from Resources. Returns null if missing.</summary>
        public static GameSettings LoadOrDefault()
        {
            var s = Resources.Load<GameSettings>(ResourcesPath);
            if (s == null) Debug.LogWarning("[GameSettings] No GameSettings.asset in Resources - using null. Create one via Assets > Create > Enara > GameSettings.");
            return s;
        }
    }
}
