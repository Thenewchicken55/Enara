using System.Collections.Generic;
using UnityEngine;
using Enara.Core;

namespace Enara.QTE
{
    /// <summary>
    /// Drives the README's core QTE loop while the player is limping through the forest:
    ///
    /// - Every <see cref="Core.GameSettings.QteMinInterval"/>..<see cref="Core.GameSettings.QteMaxInterval"/>
    ///   seconds, a QTE fires.
    /// - The player has <see cref="Core.GameSettings.QteWindowSeconds"/> to press the right key.
    /// - On success, the next word of the Jesus Prayer is shown via subtitles, and a counter goes up.
    /// - After <see cref="Core.GameSettings.QtesUntilHealed"/> successes, the player is "healed":
    ///   QTEs stop, the player's limp goes away, and the README beat "they forsake Him" begins.
    /// </summary>
    public sealed class QuickTimeEventSystem : MonoBehaviour
    {
        [SerializeField] private QteUI ui;
        [SerializeField] private UI.Subtitles subtitles;
        [SerializeField] private Player.PlayerController player;

        /// <summary>The Jesus Prayer, split word-by-word so each QTE success reveals the next.</summary>
        private static readonly string[] kPrayerWords =
        {
            "Lord", "Jesus", "Christ,", "Son", "of", "God,",
            "have", "mercy", "on", "me,", "a", "sinner."
        };

        private readonly List<QteDefinition> _pending = new();
        private GameSettings _settings;
        private GameStateMachine _state;
        private float _nextFireTime;
        private QteDefinition? _active;
        private float _activeEndTime;
        private int _successCount;
        private int _prayerWordIndex;
        private bool _healed;

        /// <summary>True once the player has completed enough QTEs and is healed.</summary>
        public bool IsHealed => _healed;

        /// <summary>Number of successful QTEs so far.</summary>
        public int SuccessCount => _successCount;

        private void Awake()
        {
            _settings = GameSettings.LoadOrDefault();
            _state = GameManager.Instance != null ? GameManager.Instance.State : null;
            BuildPendingList();
        }

        private void OnEnable()
        {
            if (_state != null) _state.OnStateChanged += HandleStateChanged;
            ScheduleNext();
        }

        private void OnDisable()
        {
            if (_state != null) _state.OnStateChanged -= HandleStateChanged;
            CancelActive();
        }

        private void HandleStateChanged(GameState prev, GameState next)
        {
            // Only fire QTEs during Exploration (limping through the forest).
            if (next == GameState.Exploration) ScheduleNext();
            else CancelActive();
        }

        private void Update()
        {
            if (_healed) return;

            if (!_active.HasValue)
            {
                if (Time.time >= _nextFireTime) FireNext();
                return;
            }

            // Active QTE - check timeout and any input.
            if (Time.time >= _activeEndTime) { FailActive(); return; }

            var input = FindObjectOfType<Input.InputReader>();
            if (input == null) return;

            var def = _active.Value;
            bool pressed = def.expectedInput switch
            {
                QteInput.Space => input.InteractPressedThisFrame || Input.GetKey(KeyCode.Space),
                QteInput.E => input.InteractPressedThisFrame || Input.GetKeyDown(KeyCode.E),
                QteInput.Q => Input.GetKeyDown(KeyCode.Q),
                QteInput.F => Input.GetKeyDown(KeyCode.F),
                QteInput.LeftClick => Input.GetMouseButtonDown(0),
                QteInput.AnyKey => AnyKeyPressed(),
                _ => false
            };
            if (pressed) SucceedActive();
            else UpdateUI();
        }

        private void BuildPendingList()
        {
            _pending.Clear();
            var window = _settings != null ? _settings.QteWindowSeconds : 1.2f;
            // Alternate inputs so the player can't just spam one key.
            var cycle = new[] { QteInput.Space, QteInput.E, QteInput.F, QteInput.Q };
            for (int i = 0; i < kPrayerWords.Length; i++)
            {
                _pending.Add(new QteDefinition
                {
                    id = $"prayer_{i}",
                    expectedInput = cycle[i % cycle.Length],
                    windowSeconds = window,
                    promptText = kPrayerWords[i]
                });
            }
        }

        private void ScheduleNext()
        {
            if (_healed || _settings == null) { _nextFireTime = float.MaxValue; return; }
            float interval = Random.Range(_settings.QteMinInterval, _settings.QteMaxInterval);
            _nextFireTime = Time.time + interval;
        }

        private void FireNext()
        {
            if (_pending.Count == 0) { ScheduleNext(); return; }
            _active = _pending[0];
            _pending.RemoveAt(0);
            _activeEndTime = Time.time + _active.Value.windowSeconds;
            if (_state != null) _state.TransitionTo(GameState.Qte);
            UpdateUI();
        }

        private void SucceedActive()
        {
            EventBus.Publish(new QteSucceededEvent(_active.Value.id));

            // Show the next word of the Jesus Prayer as a subtitle.
            if (subtitles != null && _prayerWordIndex < kPrayerWords.Length)
            {
                subtitles.Show(kPrayerWords[_prayerWordIndex], 2.5f);
                _prayerWordIndex++;
            }
            _successCount++;
            _active = null;
            if (ui != null) ui.Hide();

            if (_settings != null && _successCount >= _settings.QtesUntilHealed)
                Heal();
            else
            {
                if (_state != null) _state.TransitionTo(GameState.Exploration);
                ScheduleNext();
            }
        }

        private void FailActive()
        {
            EventBus.Publish(new QteFailedEvent(_active.Value.id));
            _active = null;
            if (ui != null) ui.Hide();
            if (_state != null) _state.TransitionTo(GameState.Exploration);
            // On failure: re-queue the same word so the player gets another shot.
            if (_prayerWordIndex > 0) _prayerWordIndex--;
            BuildPendingList();
            ScheduleNext();
        }

        private void CancelActive()
        {
            _active = null;
            if (ui != null) ui.Hide();
        }

        private void Heal()
        {
            _healed = true;
            if (player != null) player.Limping = false;
            if (_state != null) _state.TransitionTo(GameState.Exploration);
            Debug.Log("[QTE] Player healed. Limp removed. QTEs will stop. (README beat: player forsakes the Lord.)");
        }

        private void UpdateUI()
        {
            if (ui == null || !_active.HasValue) return;
            ui.Show(new QteState(_active.Value, _activeEndTime - Time.time));
        }

        private static bool AnyKeyPressed()
        {
            if (Input.anyKeyDown) return true;
            for (int k = (int)KeyCode.A; k <= (int)KeyCode.Z; k++)
                if (Input.GetKeyDown((KeyCode)k)) return true;
            return false;
        }
    }
}
