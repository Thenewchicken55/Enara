using UnityEngine;

namespace Enara.NPC
{
    /// <summary>
    /// Smoothly rotates a GameObject (or one of its bones) to face the player. Use for the
    /// Idol statue ("It can turn its head and talk to the player") and for ambient NPCs that
    /// should react when the player approaches.
    ///
    /// If <see cref="bone"/> is left null, the whole GameObject rotates; otherwise only that
    /// bone (e.g. the idol's head bone) rotates.
    /// </summary>
    public sealed class LookAtPlayer : MonoBehaviour
    {
        [SerializeField] private Transform bone;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float rotationSpeed = 4f;
        [SerializeField, Range(0f, 180f)] private float maxYawDegrees = 70f;
        [SerializeField] private bool onlyWhenInRange = true;
        [SerializeField] private float range = 6f;

        private Quaternion _initialLocalRot;

        private void Reset()
        {
            if (bone == null) bone = transform;
        }

        private void Awake()
        {
            if (bone == null) bone = transform;
            _initialLocalRot = bone.localRotation;
            if (playerTransform == null)
            {
                var player = FindObjectOfType<Player.PlayerController>();
                if (player != null) playerTransform = player.transform;
            }
        }

        private void Update()
        {
            if (playerTransform == null || bone == null) return;

            float distance = Vector3.Distance(bone.position, playerTransform.position);
            if (onlyWhenInRange && distance > range)
            {
                bone.localRotation = Quaternion.Slerp(bone.localRotation, _initialLocalRot, Time.deltaTime * rotationSpeed);
                return;
            }

            var direction = playerTransform.position - bone.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.001f) return;
            var targetRot = Quaternion.LookRotation(direction);
            var parentRot = bone.parent != null ? bone.parent.rotation : Quaternion.identity;
            var localTarget = Quaternion.Inverse(parentRot) * targetRot;
            float yaw;
            Vector3 euler = localTarget.eulerAngles;
            yaw = euler.y > 180f ? euler.y - 360f : euler.y;
            yaw = Mathf.Clamp(yaw, -maxYawDegrees, maxYawDegrees);
            var clamped = Quaternion.Euler(0f, yaw, 0f);
            bone.localRotation = Quaternion.Slerp(bone.localRotation, clamped, Time.deltaTime * rotationSpeed);
        }
    }
}
