using UnityEngine;

namespace Enara.NPC
{
    /// <summary>
    /// Base class for any non-player character. Holds identity data (display name, optional
    /// Animator) and exposes a <see cref="Pause"/>/<see cref="Resume"/> pair so cutscenes can
    /// freeze NPCs. Subclass or compose this for specific NPC behaviors (Babel slave, Old Man,
    /// leader of Babel).
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class NPCController : MonoBehaviour
    {
        [SerializeField] private string npcId = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private Animator animator;
        [SerializeField] private bool isInteractable = false;

        /// <summary>Stable designer-facing ID. Use this in scripts/data, not the GameObject name.</summary>
        public string NpcId => npcId;
        public string DisplayName => displayName;
        public Animator Animator => animator;
        public bool IsInteractable => isInteractable;

        private bool _paused;

        protected virtual void Reset()
        {
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }

        /// <summary>Freeze this NPC's behavior (animations keep playing, scripts stop).</summary>
        public virtual void Pause() { _paused = true; }

        /// <summary>Resume normal NPC behavior.</summary>
        public virtual void Resume() { _paused = false; }

        /// <summary>True while <see cref="Pause"/> is in effect.</summary>
        public bool IsPaused => _paused;
    }
}
