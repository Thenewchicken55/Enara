using System.Collections.Generic;
using UnityEngine;

namespace Enara.Audio
{
    /// <summary>
    /// Central audio hub. Plays music tracks and one-shot SFX / VO. Routes everything through
    /// an AudioMixer (assigned in the Inspector) so the master/music/sfx volumes from
    /// <see cref="Core.GameSettings"/> can be applied.
    ///
    /// One AudioManager lives in the Boot scene and persists across scene loads
    /// (DontDestroyOnLoad is applied by <see cref="Core.GameManager"/>'s parent GameObject).
    /// </summary>
    public sealed class AudioManager : MonoBehaviour
    {
        /// <summary>Per-process singleton. Set in Awake.</summary>
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource voSource;

        [Header("Defaults")]
        [SerializeField] private AudioClip defaultMusic;

        private readonly HashSet<int> _playingSfxIds = new();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy() { if (Instance == this) Instance = null; }

        private void Reset()
        {
            // Auto-grab the AudioMixer under Assets/Enara/Settings/AudioMixer.mixer if it's there.
            var sources = GetComponents<AudioSource>();
            if (musicSource == null && sources.Length > 0) musicSource = sources[0];
            if (sfxSource == null && sources.Length > 1) sfxSource = sources[1];
            if (voSource == null && sources.Length > 2) voSource = sources[2];
        }

        private void Start()
        {
            var settings = Core.GameSettings.LoadOrDefault();
            if (settings != null) ApplyVolumes(settings);
            if (defaultMusic != null) PlayMusic(defaultMusic);
        }

        /// <summary>Crossfade to a new music track. Pass null to fade to silence.</summary>
        public void PlayMusic(AudioClip clip, float fadeDuration = 1.5f)
        {
            if (musicSource == null) return;
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }

        /// <summary>Play a one-shot SFX clip.</summary>
        public void PlaySfx(AudioClip clip, float volumeScale = 1f)
        {
            if (sfxSource != null && clip != null) sfxSource.PlayOneShot(clip, volumeScale);
        }

        /// <summary>Play a voice-over line. Stops any currently-playing VO.</summary>
        public void PlayVoiceOver(AudioClip clip)
        {
            if (voSource == null || clip == null) return;
            voSource.Stop();
            voSource.clip = clip;
            voSource.Play();
        }

        /// <summary>Apply the master/music/sfx volumes from a settings asset.</summary>
        public void ApplyVolumes(Core.GameSettings settings)
        {
            if (mixer == null || settings == null) return;
            mixer.SetFloat("MasterVolume", LinearToDb(settings.MasterVolume));
            mixer.SetFloat("MusicVolume", LinearToDb(settings.MusicVolume));
            mixer.SetFloat("SfxVolume", LinearToDb(settings.SfxVolume));
        }

        private static float LinearToDb(float linear)
        {
            if (linear <= 0.0001f) return -80f;
            return 20f * Mathf.Log10(linear);
        }
    }
}
