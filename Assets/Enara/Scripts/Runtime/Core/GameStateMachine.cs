using System;

namespace Enara.Core
{
    /// <summary>
    /// Coarse-grained state of the whole game. Used by <see cref="GameManager"/> to decide
    /// which systems are active (e.g. input is locked during cutscenes).
    /// </summary>
    public enum GameState
    {
        /// <summary>Project just opened, Boot scene running. No gameplay yet.</summary>
        Boot,
        /// <summary>A pre-rendered or timeline-driven cutscene is playing. Player input off.</summary>
        Cutscene,
        /// <summary>Player can walk/look around normally.</summary>
        Exploration,
        /// <summary>A Quick Time Event is in progress.</summary>
        Qte,
        /// <summary>A dialogue tree is on screen. Movement is locked, choices are live.</summary>
        Dialogue,
        /// <summary>A UI menu is open (pause, settings, etc).</summary>
        Menu,
        /// <summary>Paused.</summary>
        Paused,
        /// <summary>Game has ended, end-card or credits rolling.</summary>
        Ending
    }

    /// <summary>
    /// Lightweight state machine. Fires <see cref="OnStateChanged"/> whenever the state changes.
    /// Other systems subscribe to enable/disable themselves based on the new state.
    /// </summary>
    public sealed class GameStateMachine
    {
        /// <summary>Raised with (previousState, newState) on every transition.</summary>
        public event Action<GameState, GameState> OnStateChanged;

        public GameState Current { get; private set; } = GameState.Boot;

        /// <summary>Transition to <paramref name="next"/>. Returns true if it was a real change.</summary>
        public bool TransitionTo(GameState next)
        {
            if (next == Current) return false;
            var prev = Current;
            Current = next;
            OnStateChanged?.Invoke(prev, next);
            return true;
        }

        /// <summary>True iff the current state should accept player gameplay input.</summary>
        public bool IsInputAllowed =>
            Current == GameState.Exploration ||
            Current == GameState.Qte ||
            Current == GameState.Dialogue;
    }
}
