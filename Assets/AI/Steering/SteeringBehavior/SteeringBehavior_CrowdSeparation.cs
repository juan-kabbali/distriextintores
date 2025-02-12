using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KD_Steering
{
    public class SteeringBehavior_CrowdSeparation : SteeringBehavior
    {
        #region Attributes

        // Minimum neighbor hood unit count
        [Tooltip("Minimum number of units in neighbor hood to consider them as a crowd")]
        [SerializeField]
        private uint m_MinNeighborHoodUnitCount = 4;

        // Neighbor hood radius
        [Tooltip("Radius of zone detecting any crowd unit")]
        [SerializeField]
        private float m_NeighborHoodRadius = 6;

        // Draw neigbor hood wire sphere
        [SerializeField]
        private bool m_DrawNeighborHoodWireSphere = true;

        // Desired velocity
        private Vector3 m_DesiredVelocity = Vector3.zero;

        #endregion

        #region SteeringBehavior Override

        /// <summary>
        /// Crowd cohesion behavior
        /// </summary>
        public override void PerformSteeringBehavior()
        {
            if (SteeringCore == null)
            {
                return;
            }

            // Get cohesion point
            Vector3 separationForce;

            if (GetSeparationVelocity(out separationForce))
            {
                // Calculate desired velocity
                m_DesiredVelocity = separationForce.normalized * SteeringCore.MaxSpeed;

                // Calculate steering force
                SteeringForce = m_DesiredVelocity - SteeringCore.Velocity;
            }
            else
            {
                SteeringForce = Vector3.zero;
            }
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
        }

        #endregion

        #region Private Manipulators

        /// <summary>
        /// Get separation velocity from crowd
        /// </summary>
        /// <returns>Separation velocity from crowd</returns>
        private bool GetSeparationVelocity(out Vector3 _SeparationForce)
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

            Vector3 force;
            // Calculate separation from neighbor hood
            for (int i = 0; i < crowdNeighbors.Count; i++)
            {
                force = transform.position - crowdNeighbors[i].transform.position;
                force *= 1 - Mathf.Min(force.sqrMagnitude / (m_NeighborHoodRadius * m_NeighborHoodRadius), 1);
                _SeparationForce += force;
            }

            _SeparationForce /= crowdNeighbors.Count;

            return true;
        }

        #endregion
    }
}