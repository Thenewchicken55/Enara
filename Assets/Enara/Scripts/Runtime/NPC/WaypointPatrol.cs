using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enara.NPC
{
    /// <summary>
    /// Walks a list of waypoints in order, looping. Used for the Babel slaves hauling dirt up
    /// the mountain and for any ambient NPC that needs to look alive.
    ///
    /// Uses <see cref="NavMeshAgent"/> for pathfinding - make sure your scene has a baked
    /// NavMesh (Window > AI > Navigation > Bake).
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class WaypointPatrol : MonoBehaviour
    {
        [SerializeField] private List<Transform> waypoints = new();
        [SerializeField] private float arrivalThreshold = 0.5f;
        [SerializeField] private float waitAtWaypoint = 1.5f;
        [SerializeField] private bool loop = true;
        [SerializeField] private float speed = 1.2f;

        private NavMeshAgent _agent;
        private int _currentIndex;
        private float _waitTimer;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = speed;
        }

        private void OnEnable()
        {
            if (waypoints.Count > 0) _agent.destination = waypoints[0].position;
        }

        private void Update()
        {
            if (waypoints.Count == 0 || !_agent.isOnNavMesh) return;

            if (_waitTimer > 0f)
            {
                _waitTimer -= Time.deltaTime;
                return;
            }

            if (_agent.remainingDistance <= arrivalThreshold && !_agent.pathPending)
            {
                _currentIndex = (_currentIndex + 1) % waypoints.Count;
                if (_currentIndex == 0 && !loop) { _agent.isStopped = true; return; }
                _agent.destination = waypoints[_currentIndex].position;
                _waitTimer = waitAtWaypoint;
            }
        }

        /// <summary>Stop patrolling. Call this when an NPC should freeze for a cutscene.</summary>
        public void Stop() { if (_agent.isOnNavMesh) _agent.isStopped = true; }

        /// <summary>Resume patrolling from the current waypoint.</summary>
        public void ResumePatrol() { if (_agent.isOnNavMesh) _agent.isStopped = false; }

        private void OnDrawGizmosSelected()
        {
            if (waypoints == null) return;
            Gizmos.color = Color.cyan;
            for (int i = 0; i < waypoints.Count; i++)
            {
                if (waypoints[i] == null) continue;
                Gizmos.DrawSphere(waypoints[i].position, 0.2f);
                if (i + 1 < waypoints.Count && waypoints[i + 1] != null)
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
    }
}
