using System.Collections.Generic;
using UnityEngine;

namespace KD_Steering
{
    public class SteeringBehavior_LeaderFollowing : SteeringBehavior
    {
        #region Attributes

        // Separation force scale
        [Tooltip("Scale separation force")]
        [SerializeField]
        private float m_SeparationForceScale = 1;

        // Leader ahead radius
        [Tooltip("Distance ahead leader where ahead point will be placed")]
        [SerializeField]
        private float m_LeaderAheadPointRadius = 3;

        // Leader ahead point distance
        [Tooltip("Distance ahead leader where ahead point will be placed")]
        [SerializeField]
        private float m_LeaderAheadPointDistance = 3;

        // Leader behind distance
        [Tooltip("Distance behind leader the character follow")]
        [SerializeField]
        private float m_LeaderBehindPointDistance = 3;

        // Slowing distance
        [SerializeField]
        private float m_SlowingDistance = 10;

        // Minimum neighbor hood unit count
        [Tooltip("Minimum number of units in neighbor hood to consider them as a crowd")]
        [SerializeField]
        private uint m_MinNeighborHoodUnitCount = 4;

        // Neighbor hood radius
        [Tooltip("Radius of zone detecting any crowd unit")]
        [SerializeField]
        private float m_NeighborHoodRadius = 6;

        // Neighboor hood angle
        [Range(180, 300)]
        [Tooltip("Vision cone angle, neihboor hood should not take account of units behind character")]
        [SerializeField]
        private float m_NeighborHoodAngle = 225;

        // Draw neigbor hood wire sphere
        [SerializeField]
        private bool m_DrawNeighborHoodWireSphere = true;

        // Old target position
        private Vector3 m_TargetOldPosition = Vector3.zero;

        // Desired velocity
        private Vector3 m_DesiredVelocity = Vector3.zero;

        #endregion

        #region SteeringBehavior Override

        /// <summary>
        /// Arrival behavior
        /// </summary>
        public override void PerformSteeringBehavior()
        {
            if (SteeringCore == null)
            {
                return;
            }

            // Calculate desired velocity
            Vector3 separationVelocity = Vector3.zero;
            Vector3 evasionVelocity = Vector3.zero;

            m_DesiredVelocity = GetArrivalVelocity();

            if (GetEvasionVelocity(out evasionVelocity))
            {
                m_DesiredVelocity += evasionVelocity;
            }

            /*
            if (GetSeparationForce(out separationVelocity))
            {
                m_DesiredVelocity += separationVelocity;
            }
            */

            m_DesiredVelocity = Vector3.ClampMagnitude(m_DesiredVelocity, SteeringCore.MaxSpeed);

            // Calculate steering force
            SteeringForce = m_DesiredVelocity - SteeringCore.Velocity;

            // Keep old target position
            m_TargetOldPosition = SteeringCore.TargetTransform.position;
        }

        #endregion

        #region MonoBehaviour

        // Gizmos
        void OnDrawGizmos()
        {
            if (m_DrawNeighborHoodWireSphere)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(transform.position, m_NeighborHoodRadius);
                Gizmos.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(-m_NeighborHoodAngle * 0.5f, transform.up) * transform.forward * m_NeighborHoodRadius);
                Gizmos.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(m_NeighborHoodAngle * 0.5f, transform.up) * transform.forward * m_NeighborHoodRadius);
            }

            if (SteeringCore == null)
            {
                return;
            }

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + m_DesiredVelocity);

            //if (SteeringCore.Rigidbody != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + SteeringCore.Velocity);
            }

            Gizmos.color = Color.white;
            Vector3 behindPointPosition = SteeringCore.TargetTransform.position - SteeringCore.TargetTransform.forward * m_LeaderBehindPointDistance;
            Vector3 aheadPointPosition = SteeringCore.TargetTransform.position + SteeringCore.TargetTransform.transform.forward * m_LeaderAheadPointDistance;

            Gizmos.DrawLine(SteeringCore.TargetTransform.position, behindPointPosition);
            Gizmos.DrawWireSphere(behindPointPosition, 1);

            Gizmos.DrawLine(SteeringCore.TargetTransform.position, aheadPointPosition);
            Gizmos.DrawWireSphere(aheadPointPosition, m_LeaderAheadPointRadius);
        }

        #endregion

        #region Private Manipulators

        /// <summary>
        /// Get velocity for arrival velocity
        /// </summary>
        /// <returns>Needed velocity to arrive to leader</returns>
        private Vector3 GetArrivalVelocity()
        {
            // Calculate stopping factor
            Vector3 behindPointPosition = SteeringCore.TargetTransform.position - SteeringCore.TargetTransform.forward * m_LeaderBehindPointDistance;
            float TargetDistance = (behindPointPosition - transform.position).magnitude;
            float stoppingFactor;

            if (m_SlowingDistance > 0)
            {
                stoppingFactor = Mathf.Clamp(TargetDistance / m_SlowingDistance, 0.0f, 1.0f);
            }
            else
            {
                stoppingFactor = Mathf.Clamp(TargetDistance, 0.0f, 1.0f);
            }

            Vector3 arrivalVelocity = (behindPointPosition - transform.position).normalized * SteeringCore.MaxSpeed * stoppingFactor;

            return arrivalVelocity;
        }

        /// <summary>
        /// Get evasion velocity
        /// </summary>
        /// <returns></returns>
        private bool GetEvasionVelocity(out Vector3 _EvasionVelocity)
        {
            _EvasionVelocity = Vector3.zero;

            // Check character is in ahead zone
            Vector3 aheadPointPosition = SteeringCore.TargetTransform.position + SteeringCore.TargetTransform.transform.forward * m_LeaderAheadPointDistance;
            float sqrDist = (transform.position - aheadPointPosition).sqrMagnitude;

            if (sqrDist > m_LeaderAheadPointRadius * m_LeaderAheadPointRadius)
            {
                return false;
            }

            // Predict target future position depending on distance
            Vector3 targetVelocity = SteeringCore.TargetTransform.position - m_TargetOldPosition;
            float targetDistance = (SteeringCore.TargetTransform.position - transform.position).magnitude;
            float targetVelocityMagnitude = targetVelocity.magnitude;

            if (targetVelocityMagnitude == 0)
            {
                targetVelocityMagnitude = 0.0001f;
            }

            float targetVelocityScale = targetDistance / targetVelocityMagnitude;

            // Calculate desired velocity
            _EvasionVelocity = -(SteeringCore.TargetTransform.position + (targetVelocity * targetVelocityScale) - transform.position);

            return true;
        }

        /// <summary>
        /// Get separtion velocity from crowd
        /// </summary>
        /// <returns></returns>
        private bool GetSeparationForce(out Vector3 _SeparationForce)
        {
            List<SteeringCrowdUnit> crowd = SteeringCrowdUnit.CrowdUnitList;
            List<SteeringCrowdUnit> crowdNeighbors = new List<SteeringCrowdUnit>();
            float sqrDist = 0;
            _SeparationForce = Vector3.zero;

            // Get all crow units in neighbor hood
            for (int i = 0; i < crowd.Count; i++)
            {
                SteeringCrowdUnit crowdUnit = crowd[i];

                if (crowdUnit != null)
                {
                    if (crowdUnit.gameObject != gameObject)
                    {
                        sqrDist = (crowdUnit.transform.position - transform.position).sqrMagnitude;

                        if (sqrDist < m_NeighborHoodRadius * m_NeighborHoodRadius)
                        {
                            crowdNeighbors.Add(crowdUnit);
                        }
                    }
                }
            }

            if (crowdNeighbors.Count < m_MinNeighborHoodUnitCount)
            {
                return false;
            }

            // Calculate separation from neighbor hood
            for (int i = 0; i < crowdNeighbors.Count; i++)
            {
                _SeparationForce += transform.position - crowdNeighbors[i].transform.position;
            }

            _SeparationForce /= crowdNeighbors.Count;
            _SeparationForce *= m_SeparationForceScale;

            return true;
        }

        #endregion
    }
}