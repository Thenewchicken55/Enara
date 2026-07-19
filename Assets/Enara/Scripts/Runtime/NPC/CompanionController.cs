using UnityEngine;
using UnityEngine.AI;

namespace Enara.NPC
{
    /// <summary>
    /// Companion AI for the Old Man in Path 1. Follows the player at a distance, optionally
    /// stops when the player stops, and can be told to leave (when the Virgin Mary says
    /// "Leave him alone. Go, now.").
    ///
    /// Uses NavMeshAgent for pathfinding - bake a NavMesh in the scene.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class CompanionController : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float followDistance = 2.5f;
        [SerializeField] private float stopDistance = 1.8f;
        [SerializeField] private float updateInterval = 0.25f;
        [SerializeField] private float moveSpeed = 1.4f;

        private NavMeshAgent _agent;
        private float _nextUpdate;
        private bool _isLeaving;
        private Vector3 _leaveTarget;

        /// <summary>True while the companion is walking away (e.g. dismissed by Mary).</summary>
        public bool IsLeaving => _isLeaving;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = moveSpeed;
            if (playerTransform == null)
            {
                var p = FindObjectOfType<Player.PlayerController>();
                if (p != null) playerTransform = p.transform;
            }
        }

        private void Update()
        {
            if (_isLeaving)
            {
                if (_agent.isOnNavMesh && !_agent.pathPending && _agent.remainingDistance < 0.5f)
                    enabled = false; // reached leave target
                return;
            }

            if (playerTransform == null || Time.time < _nextUpdate) return;
            _nextUpdate = Time.time + updateInterval;

            float dist = Vector3.Distance(transform.position, playerTransform.position);
            if (dist > followDistance && _agent.isOnNavMesh)
            {
                _agent.isStopped = false;
                _agent.destination = playerTransform.position;
            }
            else if (dist < stopDistance && _agent.isOnNavMesh)
            {
                _agent.isStopped = true;
            }
        }

        /// <summary>Tell the companion to walk away to <paramref name="destination"/> and disable itself.</summary>
        public void Dismiss(Vector3 destination)
        {
            _isLeaving = true;
            _leaveTarget = destination;
            if (_agent.isOnNavMesh)
            {
                _agent.isStopped = false;
                _agent.destination = destination;
            }
        }

        /// <summary>Resume following the player after a Dismiss call.</summary>
        public void Rejoin()
        {
            _isLeaving = false;
            enabled = true;
        }
    }
}
