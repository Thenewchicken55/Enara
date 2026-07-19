using System;
using UnityEngine;

namespace Enara.QTE
{
    /// <summary>
    /// Definition of a single Quick Time Event. Designed to be authored as a ScriptableObject
    /// so designers can tweak timing without touching code. For the Jesus Prayer loop, one
    /// asset is reused repeatedly; the prompt text alternates between words of the prayer.
    /// </summary>
    [Serializable]
    public struct QteDefinition
    {
        [Tooltip("Stable identifier for analytics / event payloads.")]
        public string id;
        [Tooltip("The key the player must press. Match a control in InputReader.")]
        public QteInput expectedInput;
        [Tooltip("How long (seconds) the player has to press the right key.")]
        [Min(0.1f)] public float windowSeconds;
        [Tooltip("Text shown on the prompt (a word of the Jesus Prayer, a button glyph, etc).")]
        [TextArea(1, 3)] public string promptText;
    }

    /// <summary>Input options for a QTE. Extend as needed (gamepad buttons, mouse buttons...).</summary>
    public enum QteInput
    {
        AnyKey,
        Space,
        E,
        Q,
        F,
        LeftClick,
    }

    /// <summary>Read-only view of a QTE in progress. Used by the UI to draw the prompt and timer.</summary>
    public readonly struct QteState
    {
        public readonly string Id;
        public readonly QteInput Expected;
        public readonly string PromptText;
        public readonly float RemainingSeconds;
        public readonly float TotalSeconds;

        public QteState(QteDefinition def, float remaining)
        {
            Id = def.id;
            Expected = def.expectedInput;
            PromptText = def.promptText;
            RemainingSeconds = remaining;
            TotalSeconds = def.windowSeconds;
        }

        public float NormalizedProgress => TotalSeconds > 0f ? RemainingSeconds / TotalSeconds : 0f;
    }
}
