using UnityEngine;
using UnityEngine.UI;
using Enara.Core;

namespace Enara.Menu
{
    /// <summary>
    /// Pause overlay shown when the player presses Escape. Freezes time, shows buttons:
    /// Resume, Settings, Quit to Menu.
    ///
    /// Place this on a UI Canvas child of the persistent Boot scene root. The component
    /// disables itself on Awake; the InputReader's PausePressedThisFrame enables it.
    /// </summary>
    public sealed class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitToMenuButton;
        [SerializeField] private Input.InputReader input;
        [SerializeField] private SceneFlow.SceneLoader sceneLoader;
        [SerializeField] private string mainMenuScenePath = "Assets/Enara/Scenes/MainMenu.unity";

        private GameState _stateBeforePause;

        private void Awake()
        {
            if (root == null) root = gameObject;
            Hide();
            if (input == null) input = FindObjectOfType<Input.InputReader>();
            if (resumeButton != null) resumeButton.onClick.AddListener(Resume);
            if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);
            if (quitToMenuButton != null) quitToMenuButton.onClick.AddListener(QuitToMenu);
        }

        private void Update()
        {
            if (input != null && input.PausePressedThisFrame)
            {
                if (root.activeSelf) Resume();
                else Pause();
            }
        }

        /// <summary>Pause the game and show this menu.</summary>
        public void Pause()
        {
            if (GameManager.Instance == null) return;
            _stateBeforePause = GameManager.Instance.State.Current;
            GameManager.Instance.SetState(GameState.Paused);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (root != null) root.SetActive(true);
        }

        /// <summary>Resume the game from where it was paused.</summary>
        public void Resume()
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (root != null) root.SetActive(false);
            if (GameManager.Instance != null) GameManager.Instance.SetState(_stateBeforePause);
        }

        /// <summary>Open the settings panel (which should auto-hide this if needed).</summary>
        public void OpenSettings()
        {
            var s = FindObjectOfType<SettingsMenu>(true);
            if (s != null) s.Show();
        }

        /// <summary>Quit to the main menu. Restores time scale first.</summary>
        public void QuitToMenu()
        {
            Time.timeScale = 1f;
            if (sceneLoader != null) sceneLoader.LoadScene(mainMenuScenePath);
        }

        private void Hide() { if (root != null) root.SetActive(false); }
    }
}
