using UnityEngine;

namespace Enara.Core
{
    /// <summary>
    /// Global game orchestrator. Owns the <see cref="GameStateMachine"/>, exposes it to other
    /// systems, and survives scene loads (it lives in the Boot scene via DontDestroyOnLoad).
    ///
    /// Access the singleton via <see cref="Instance"/>. In the rare case you cannot reach it
    /// (e.g. inside an EditMode test), the property returns null - always null-check.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public sealed class GameManager : MonoBehaviour
    {
        /// <summary>Per-process singleton. Set in Awake.</summary>
        public static GameManager Instance { get; private set; }

        /// <summary>The active state machine. Never null while <see cref="Instance"/> is set.</summary>
        public GameStateMachine State { get; } = new GameStateMachine();

        [SerializeField, Tooltip("If true, the EventBus is cleared whenever a scene loads.")]
        private bool clearEventBusOnSceneLoad = true;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            Application.quitting += OnApplicationQuitting;
        }

        private void OnDestroy()
        {
            if (Instance != this) return;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
            Application.quitting -= OnApplicationQuitting;
            Instance = null;
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            if (clearEventBusOnSceneLoad) EventBus.Clear();
        }

        private void OnApplicationQuitting()
        {
            EventBus.Clear();
            ServiceLocator.Clear();
        }

        /// <summary>Convenience: transition the global state.</summary>
        public void SetState(GameState next) => State.TransitionTo(next);
    }
}
