using UnityEngine;

namespace Enara.Player
{
    /// <summary>
    /// First-person mouse look. Applies yaw to the player body and pitch to a child camera
    /// transform (so the body always faces forward while the head looks up/down).
    ///
    /// Looks at <see cref="Core.GameSettings"/> for sensitivity, and disables itself when
    /// <see cref="Core.GameState"/> is anything other than Exploration.
    /// </summary>
    public sealed class FirstPersonLook : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform bodyTransform;
        [SerializeField, Range(-89f, 0f)] private float minPitch = -80f;
        [SerializeField, Range(0f, 89f)] private float maxPitch = 80f;

        private float _yaw;
        private float _pitch;

        private void Reset()
        {
            bodyTransform = transform;
            if (cameraTransform == null && TryGetComponentInChildren<Camera>(out var cam))
                cameraTransform = cam.transform;
        }

        private void Start()
        {
            if (cameraTransform == null && TryGetComponentInChildren<Camera>(out var cam))
                cameraTransform = cam.transform;
            _yaw = bodyTransform != null ? bodyTransform.localEulerAngles.y : 0f;
        }

        /// <summary>Apply a mouse delta (already scaled by sensitivity). Call from PlayerController.</summary>
        public void ApplyLook(Vector2 delta)
        {
            if (!enabled) return;
            _yaw += delta.x;
            _pitch = Mathf.Clamp(_pitch - delta.y, minPitch, maxPitch);
            if (bodyTransform != null)
                bodyTransform.localRotation = Quaternion.Euler(0f, _yaw, 0f);
            if (cameraTransform != null)
                cameraTransform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }
    }
}
