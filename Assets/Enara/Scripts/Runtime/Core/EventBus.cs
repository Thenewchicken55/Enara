using System;
using System.Collections.Generic;

namespace Enara.Core
{
    /// <summary>
    /// A minimal in-process event bus. Anything can publish or subscribe to a strongly-typed
    /// event. The bus keeps no references to subscribers after they unsubscribe.
    ///
    /// Use this for one-shot cross-system signals like "PlayerDied", "ChapterCompleted",
    /// "QteSucceeded". For persistent game data, use a ScriptableObject or SaveSystem instead.
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, Delegate> s_handlers = new();

        /// <summary>Subscribe a callback for events of type <typeparamref name="T"/>.</summary>
        public static void Subscribe<T>(Action<T> handler) where T : struct, IGameEvent
        {
            if (handler == null) return;
            s_handlers.TryGetValue(typeof(T), out var existing);
            s_handlers[typeof(T)] = (existing as Action<T>) + handler;
        }

        /// <summary>Unsubscribe a previously-registered callback.</summary>
        public static void Unsubscribe<T>(Action<T> handler) where T : struct, IGameEvent
        {
            if (handler == null) return;
            if (!s_handlers.TryGetValue(typeof(T), out var existing)) return;
            var typed = existing as Action<T>;
            typed -= handler;
            s_handlers[typeof(T)] = typed;
        }

        /// <summary>Publish an event to all current subscribers.</summary>
        public static void Publish<T>(T evt) where T : struct, IGameEvent
        {
            if (s_handlers.TryGetValue(typeof(T), out var existing))
            {
                (existing as Action<T>)?.Invoke(evt);
            }
        }

        /// <summary>Remove every subscriber. Call this when leaving a scene to avoid leaks.</summary>
        public static void Clear()
        {
            s_handlers.Clear();
        }
    }

    /// <summary>Marker interface for structs that flow through the EventBus.</summary>
    public interface IGameEvent { }

    // ---- Built-in game events -----------------------------------------------------

    /// <summary>Published whenever the player succeeds at a QTE.</summary>
    public readonly struct QteSucceededEvent : IGameEvent
    {
        public readonly string QteId;
        public QteSucceededEvent(string qteId) { QteId = qteId; }
    }

    /// <summary>Published whenever the player fails a QTE.</summary>
    public readonly struct QteFailedEvent : IGameEvent
    {
        public readonly string QteId;
        public QteFailedEvent(string qteId) { QteId = qteId; }
    }

    /// <summary>Published when a chapter begins (use <see cref="ChapterDirector"/>).</summary>
    public readonly struct ChapterStartedEvent : IGameEvent
    {
        public readonly string ChapterId;
        public ChapterStartedEvent(string chapterId) { ChapterId = chapterId; }
    }

    /// <summary>Published when the player picks a dialogue choice.</summary>
    public readonly struct ChoiceMadeEvent : IGameEvent
    {
        public readonly string ChoiceId;
        public ChoiceMadeEvent(string choiceId) { ChoiceId = choiceId; }
    }

    /// <summary>Published when the player should be enabled/disabled.</summary>
    public readonly struct PlayerInputEnabledEvent : IGameEvent
    {
        public readonly bool Enabled;
        public PlayerInputEnabledEvent(bool enabled) { Enabled = enabled; }
    }
}
