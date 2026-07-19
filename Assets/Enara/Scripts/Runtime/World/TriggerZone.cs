using System;
using UnityEngine;
using UnityEngine.Events;

namespace Enara.World
{
    /// <summary>
    /// Generic trigger zone. Calls <see cref="onEnter"/> when a tagged collider enters the
    /// attached trigger collider, and <see cref="onExit"/> when it leaves. Use this everywhere
    /// you need a "step here to do X" - path entrance triggers, area events, Mary appearance,
    /// the noose trigger, etc.
    ///
    /// The filter is by tag - default "Player". The trigger collider can be on this GameObject
    /// or a child; <see cref="Reset"/> grabs the first one it finds.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class TriggerZone : MonoBehaviour
    {
        [SerializeField] private string triggerTag = "Player";
        [SerializeField] private bool triggerOnce = true;
        [SerializeField] private UnityEvent onEnter = new();
        [SerializeField] private UnityEvent onExit = new();

        public UnityEvent OnEnter => onEnter;
        public UnityEvent OnExit => onExit;

        private bool _fired;
        private Collider _collider;

        private void Reset()
        {
            _collider = GetComponent<Collider>();
            if (_collider != null) _collider.isTrigger = true;
        }

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            if (_collider != null) _collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggerOnce && _fired) return;
            if (!string.IsNullOrEmpty(triggerTag) && !other.CompareTag(triggerTag)) return;
            _fired = true;
            onEnter?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!string.IsNullOrEmpty(triggerTag) && !other.CompareTag(triggerTag)) return;
            onExit?.Invoke();
        }

        /// <summary>Reset the trigger so it can fire again.</summary>
        public void ResetTrigger() { _fired = false; }
    }
}
