using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

namespace Enara.Menu
{
    /// <summary>
    /// Settings panel: master/music/sfx volume sliders + look sensitivity slider + (optional)
    /// subtitles toggle. Wired to the AudioManager and GameSettings at runtime.
    /// </summary>
    public sealed class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider sensitivitySlider;
        [SerializeField] private Toggle subtitlesToggle;
        [SerializeField] private Button closeButton;

        private Core.GameSettings _settings;
        private Audio.AudioManager _audio;

        private void Awake()
        {
            if (root == null) root = gameObject;
            _settings = Core.GameSettings.LoadOrDefault();
            _audio = Audio.AudioManager.Instance;
            Hide();
            WireSliders();
            WireButton(closeButton, Hide);
        }

        private void WireSliders()
        {
            if (_settings == null) return;
            if (masterSlider != null) { masterSlider.value = _settings.MasterVolume; masterSlider.onValueChanged.AddListener(v => ApplyMaster(v)); }
            if (musicSlider != null) { musicSlider.value = _settings.MusicVolume; musicSlider.onValueChanged.AddListener(v => ApplyMusic(v)); }
            if (sfxSlider != null) { sfxSlider.value = _settings.SfxVolume; sfxSlider.onValueChanged.AddListener(v => ApplySfx(v)); }
            if (sensitivitySlider != null) { sensitivitySlider.value = _settings.LookSensitivity; sensitivitySlider.onValueChanged.AddListener(v => ApplySensitivity(v)); }
            if (subtitlesToggle != null) { subtitlesToggle.isOn = true; subtitlesToggle.onValueChanged.AddListener(on => SetSubtitles(on)); }
        }

        private void WireButton(Button b, UnityEngine.Events.UnityAction a)
        {
            if (b != null) b.onClick.AddListener(a);
        }

        public void Show() { if (root != null) root.SetActive(true); }
        public void Hide() { if (root != null) root.SetActive(false); }

        // ---- Settings changers ------------------------------------------------------

        public void ApplyMaster(float v) { if (_audio != null && _settings != null) _audio.ApplyVolumes(_settings); }
        public void ApplyMusic(float v) { if (_audio != null && _settings != null) _audio.ApplyVolumes(_settings); }
        public void ApplySfx(float v) { if (_audio != null && _settings != null) _audio.ApplyVolumes(_settings); }
        public void ApplySensitivity(float v) { /* Read from GameSettings each frame - nothing to do. */ }
        public void SetSubtitles(bool on)
        {
            var subs = FindObjectOfType<UI.Subtitles>(true);
            if (subs != null) subs.gameObject.SetActive(on);
        }
    }
}
