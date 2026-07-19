using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enara.Core;

namespace Enara.SceneFlow
{
    /// <summary>
    /// Drives the player through the game. Owns an ordered list of <see cref="ChapterDefinition"/>
    /// and exposes <see cref="Advance"/> to move to the next chapter.
    ///
    /// The list of chapters is wired in the Inspector (a ChapterDirector MonoBehaviour
    /// lives in the Boot scene). The actual scene load is delegated to <see cref="SceneLoader"/>.
    /// </summary>
    public sealed class ChapterDirector : MonoBehaviour
    {
        [SerializeField] private List<ChapterDefinition> chapters = new();
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField, Tooltip("If true, call StartGame() automatically in Start(). Useful for the Boot scene; turn off if you have a Main Menu.")]
        private bool startOnAwake = false;
        [SerializeField, Tooltip("Delay (seconds) before auto-start so the Fader can fade in. Ignored if startOnAwake is false.")]
        private float startDelay = 0.5f;

        /// <summary>Index of the currently-active chapter, or -1 before <see cref="StartGame"/> is called.</summary>
        public int CurrentIndex { get; private set; } = -1;

        /// <summary>The chapter that is currently active, or null.</summary>
        public ChapterDefinition CurrentChapter =>
            CurrentIndex >= 0 && CurrentIndex < chapters.Count ? chapters[CurrentIndex] : null;

        private void Reset()
        {
            // Auto-find a SceneLoader on the same GameObject or in the scene when this component is added.
            if (sceneLoader == null) sceneLoader = FindObjectOfType<SceneLoader>();
        }

        private void Start()
        {
            if (startOnAwake) StartCoroutine(StartRoutine());
        }

        private IEnumerator StartRoutine()
        {
            if (startDelay > 0f) yield return new WaitForSeconds(startDelay);
            StartGame();
        }

        /// <summary>Begin the game by loading the first chapter.</summary>
        public void StartGame()
        {
            if (chapters.Count == 0)
            {
                Debug.LogError("[ChapterDirector] No chapters assigned - cannot start game.");
                return;
            }
            LoadChapter(0);
        }

        /// <summary>Move to the next chapter in the list. No-op if at the end.</summary>
        public void Advance()
        {
            if (CurrentIndex + 1 >= chapters.Count)
            {
                Debug.Log("[ChapterDirector] Reached end of chapters. Trigger ending here.");
                GameManager.Instance?.SetState(GameState.Ending);
                return;
            }
            LoadChapter(CurrentIndex + 1);
        }

        /// <summary>Jump to a chapter by its <see cref="ChapterId"/>. Returns false if not found.</summary>
        public bool GoToChapter(ChapterId id)
        {
            var idx = chapters.FindIndex(c => c.Id == id);
            if (idx < 0) return false;
            LoadChapter(idx);
            return true;
        }

        private void LoadChapter(int index)
        {
            CurrentIndex = index;
            var chapter = chapters[index];
            EventBus.Publish(new ChapterStartedEvent(chapter.Id.Value));
            if (sceneLoader != null && !string.IsNullOrEmpty(chapter.ScenePath))
                sceneLoader.LoadScene(chapter.ScenePath);
            else
                Debug.LogWarning($"[ChapterDirector] Chapter '{chapter.Id}' has no SceneLoader / scene path - skipping load.");
        }
    }
}
