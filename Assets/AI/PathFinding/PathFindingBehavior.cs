using System.Collections;
using UnityEngine;
using KD_Steering;

namespace KD_PathFinding
{
    [RequireComponent(typeof(SteeringCore))]
    [RequireComponent(typeof(PathFinder))]
    public class PathFindingBehavior : MonoBehaviour
    {
        #region Attributes

        // Path finder component
        private PathFinder      m_PathFinder                        = null;

        // Steering core component
        private SteeringCore    m_SteeringCore                      = null;

        // Movement coroutine
        private Coroutine       m_MovementCoroutine                 = null;

        // Debug destination
        [SerializeField]
        private Transform       m_DebugTransform                    = null;

        // Disatnce needed to switch to next point on path
        [SerializeField]
        private float           m_DistanceToSwitchToNextPathPoint   = 1;

        // Index of current point on path
        private int             m_IndexCurrentPathPoint             = 0;

        // Old path length
        private int             m_OldPathLength                     = 0;

        #endregion

        #region MonoBehaviour

        // Use this for initialization
        void Start()
        {
            // Get path finder component
            m_PathFinder = GetComponent<PathFinder>();

            // Get steering core component
            m_SteeringCore = GetComponent<SteeringCore>();

            SetDestination(m_DebugTransform.position);
        }

        // Gizmos
        void OnDrawGizmos()
        {
            if (m_SteeringCore != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(m_SteeringCore.Target, 0.5f);
            }
        }

        #endregion

        #region Public Manipulators

        /// <summary>
        /// Stop movement on path over time
        /// </summary>
        public void ResetDestination()
        {
            if (m_PathFinder != null)
            {
                m_PathFinder.ResetDestination();

                if (m_MovementCoroutine != null)
                {
                    StopCoroutine(m_MovementCoroutine);
                }
            }
        }

        /// <summary>
        /// Set destination. Move agent on path over time to reach destination from steering.
        /// Set steering core target with destination, require a steering seek / arrival behavior to move
        /// Call reset destination to stop movement on path
        /// </summary>
        /// <param name="_Destination"></param>
        public void SetDestination(Vector3 _Destination)
        {
            if (m_PathFinder != null)
            {
                m_PathFinder.SetDestination(_Destination);

                if (m_MovementCoroutine == null)
                {
                    m_MovementCoroutine = StartCoroutine(CR_MoveOnPath());
                }
            }
        }

        #endregion

        #region Coroutines

        private IEnumerator CR_MoveOnPath()
        {
            while (m_PathFinder != null && m_SteeringCore != null)
            {
                // Update steering core target
                Vector3[] path = m_PathFinder.GetPath();

                if (path.Length > 0)
                {
                    if (path.Length != m_OldPathLength)
                    {
                        m_IndexCurrentPathPoint = 0;
                    }

                    if (m_IndexCurrentPathPoint < path.Length)
                    {
                        m_SteeringCore.Target = path[m_IndexCurrentPathPoint];
                    }
                    else
                    {
                        ResetDestination();
                        yield break;
                    }

                    if ((path[m_IndexCurrentPathPoint] - transform.position).sqrMagnitude < m_DistanceToSwitchToNextPathPoint * m_DistanceToSwitchToNextPathPoint)
                    {
                        m_IndexCurrentPathPoint++;
                    }
                }
                else
                {
                    ResetDestination();
                }

                m_OldPathLength = path.Length;

                yield return null;
            }
        }

        #endregion

        #region DEBUG

        [ContextMenu("Set destination")]
        private void SetDestination()
        {
            SetDestination(m_DebugTransform.position);
        }

        #endregion
    }
}