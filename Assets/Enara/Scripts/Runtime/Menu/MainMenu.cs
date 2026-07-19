using UnityEngine;
using UnityEngine.UI;
using Enara.Core;
using Enara.SceneFlow;

namespace Enara.Menu
{
    /// <summary>
    /// Title screen. Buttons: New Game, Continue (if save exists), Settings, Quit.
    /// Wire the buttons' OnClick events to the matching public methods.
    /// </summary>
    public sealed class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private ChapterDirector chapterDirector;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private string bootScenePath = "Assets/Enara/Scenes/Boot.unity";

        private void Awake()
        {
            if (root == null) root = gameObject;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (GameManager.Instance != null) GameManager.Instance.SetState(GameState.Menu);

            if (continueButton != null)
                continueButton.interactable = Save.SaveSystem.Instance != null && Save.SaveSystem.Instance.HasSaveFile();

            WireButton(newGameButton, OnNewGame);
            WireButton(continueButton, OnContinue);
            WireButton(settingsButton, OnSettings);
            WireButton(quitButton, OnQuit);
        }

        private void WireButton(Button b, UnityEngine.Events.UnityAction action)
        {
            if (b != null) b.onClick.AddListener(action);
        }

        /// <summary>Start a new game. Wipes the save, loads the first chapter.</summary>
        public void OnNewGame()
        {
            if (Save.SaveSystem.Instance != null) Save.SaveSystem.Instance.Reset();
            if (chapterDirector != null) chapterDirector.StartGame();
            else if (sceneLoader != null) sceneLoader.LoadScene(bootScenePath);
        }

        /// <summary>Continue from the saved chapter.</summary>
        public void OnContinue()
        {
            if (Save.SaveSystem.Instance == null || !Save.SaveSystem.Instance.HasSaveFile())
            {
                OnNewGame();
                return;
            }
            var chapterId = new ChapterId { Value = Save.SaveSystem.Instance.CurrentChapter };
            if (chapterDirector != null && !chapterDirector.GoToChapter(chapterId))
                chapterDirector.StartGame();
        }

        /// <summary>Open the settings panel.</summary>
        public void OnSettings()
        {
            var settings = FindObjectOfType<SettingsMenu>(true);
            if (settings != null) settings.Show();
        }

        /// <summary>Quit to desktop (or to Boot scene in editor).</summary>
        public void OnQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
