using UnityEngine;
using UnityEngine.Events;

namespace Enara.World
{
    /// <summary>
    /// Fires its event when the player's camera has been looking at this GameObject for
    /// <see cref="requiredSeconds"/> total. Used for "witness" beats - the player must look at
    /// the icon of Christ, the idol, etc. for a moment before something happens.
    ///
    /// Place this on the looked-at object and ensure it has a collider (the Interactor's raycast
    /// hits the collider, not the transform).
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class WitnessTrigger : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float requiredSeconds = 2f;
        [SerializeField] private bool fireOnce = true;
        [SerializeField] private UnityEvent onWitnessed = new();

        private float _accumulated;
        private bool _fired;

        public UnityEvent OnWitnessed => onWitnessed;

        private void Awake()
        {
            if (playerCamera == null) playerCamera = FindObjectOfType<Camera>(true);
        }

        private void Update()
        {
            if (_fired || playerCamera == null) return;

            var dir = (transform.position - playerCamera.transform.position).normalized;
            float dot = Vector3.Dot(playerCamera.transform.forward, dir);
            if (dot < 0.985f) { _accumulated = Mathf.Max(0f, _accumulated - Time.deltaTime); return; }

            // Verify the camera actually has line of sight (not looking through a wall).
            if (Physics.Raycast(playerCamera.transform.position, dir, out var hit, 50f))
            {
                if (hit.collider.GetComponentInParent<WitnessTrigger>() != this) return;
            }
            else return;

            _accumulated += Time.deltaTime;
            if (_accumulated >= requiredSeconds)
            {
                onWitnessed?.Invoke();
                if (fireOnce) _fired = true;
            }
        }

        /// <summary>Manually mark this as witnessed (e.g. from a different trigger system).</summary>
        public void MarkWitnessed()
        {
            if (_fired) return;
            _fired = true;
            onWitnessed?.Invoke();
        }

        /// <summary>Allow this to fire again.</summary>
        public void ResetWitness() { _fired = false; _accumulated = 0f; }
    }
}
