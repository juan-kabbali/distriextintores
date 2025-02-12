using System.Collections.Generic;
using UnityEngine;

namespace KD_Steering
{
    public class SteeringBehavior_UnalignedObstacleAvoidance : SteeringBehavior
    {
        #region Attributes

        // Bounding sphere radius
        [Tooltip("A sphere of this radius will be used to anticipate a collision")]
        [SerializeField]
        private float m_BoundingSphereRadius = 1;

        // Obstacle max distance
        [Tooltip("Max distance to anticipate an obstacle and begin avoidance")]
        [SerializeField]
        private float m_ObstacleMaxDistance = 10;

        // Steering force conservation after avoiding
        [Tooltip("Scale down steering force to become null in a close futur, not instantly. More the factor is high, more the steering force will take time to reduce")]
        [SerializeField]
        [Range(0.1f, 0.9f)]
        private float m_SteeringForceConservation = 0.9f;

        // Desired velocity
        private Vector3 m_DesiredVelocity = Vector3.zero;

        // Dynamic obstacle
        private SteeringExternalElement_Dynamic m_DynamicObstacle = null;

        #endregion

        #region SteeringBehavior Override

        /// <summary>
        /// Unaligned obstacle avoidance behavior
        /// </summary>
        public override void PerformSteeringBehavior()
        {
            if (SteeringCore == null)
            {
                return;
            }

            Vector3 avoidanceForce = Vector3.zero;

            // Get most threatening obstacle
            m_DynamicObstacle = GetMostThreateningDynamicObstacle();

            // Calculate avoidance force
            if (m_DynamicObstacle != null)
            {
                avoidanceForce = transform.position + SteeringCore.Velocity - m_DynamicObstacle.transform.position;

                if (Vector3.Dot(avoidanceForce, SteeringCore.Velocity) < -0.9f)
                {
                    avoidanceForce = transform.right;
                }
            }

            if (avoidanceForce != Vector3.zero)
            {
                // Calculate desired velocity
                m_DesiredVelocity = (avoidanceForce).normalized * SteeringCore.MaxSpeed;

                // Calculate steering force
                SteeringForce = m_DesiredVelocity - SteeringCore.Velocity;
            }
            else
            {
                SteeringForce *= m_SteeringForceConservation;
            }
        }

        #endregion

        #region MonoBehaviour

        // Gizmos
        void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, m_BoundingSphereRadius);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * m_ObstacleMaxDistance);

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

            if (m_DynamicObstacle != null)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.white;
            }

            Gizmos.DrawWireSphere(transform.position + SteeringCore.Velocity, m_BoundingSphereRadius);

            List<SteeringExternalElement_Dynamic> dynamicObstacleList = SteeringExternalElement_Dynamic.ElementList;

            for (int i = 0; i < dynamicObstacleList.Count; i++)
            {
                if (dynamicObstacleList[i] != null)
                {
                    if (dynamicObstacleList[i].gameObject != gameObject)
                    {
                        if (m_DynamicObstacle == dynamicObstacleList[i])
                        {
                            Gizmos.color = Color.red;
                        }
                        else
                        {
                            Gizmos.color = Color.white;
                        }

                        Gizmos.DrawWireSphere(dynamicObstacleList[i].transform.position + dynamicObstacleList[i].Velocity, dynamicObstacleList[i].BoundingSphereRadius);
                    }
                }
            }
        }

        #endregion

        #region Private Manipulators

        /// <summary>
        /// Get the most threatening dynamic obstacle in range
        /// </summary>
        /// <returns>The most threatening dynamic obstacle in range</returns>
        private SteeringExternalElement_Dynamic GetMostThreateningDynamicObstacle()
        {
            List<SteeringExternalElement_Dynamic> dynamicObstacleList = SteeringExternalElement_Dynamic.ElementList;
            List<SteeringExternalElement_Dynamic> threateningObstacleList = new List<SteeringExternalElement_Dynamic>();
            Vector3 obstacleFuturePosition = Vector3.zero;
            Vector3 futurePosition = Vector3.zero;
            SteeringExternalElement_Dynamic dynamicObstacle = null;
            float sumRadius = 0;

            // Get all threatening obstacle
            for (int i = 0; i < dynamicObstacleList.Count; i++)
            {
                dynamicObstacle = dynamicObstacleList[i];

                if (dynamicObstacle != null)
                {
                    if (dynamicObstacle.gameObject != gameObject)
                    {
                        if ((dynamicObstacle.transform.position - transform.position).sqrMagnitude < m_ObstacleMaxDistance * m_ObstacleMaxDistance)
                        {
                            obstacleFuturePosition = dynamicObstacle.transform.position + dynamicObstacle.Velocity;
                            futurePosition = transform.position + SteeringCore.Velocity;

                            sumRadius = m_BoundingSphereRadius + dynamicObstacle.BoundingSphereRadius;

                            if ((obstacleFuturePosition - futurePosition).sqrMagnitude < sumRadius * sumRadius)
                            {
                                threateningObstacleList.Add(dynamicObstacle);
                            }
                        }
                    }
                }
            }

            if (threateningObstacleList.Count <= 0)
            {
                return null;
            }

            float nearestSqrDist = 0;
            float sqrDist = 0;
            SteeringExternalElement_Dynamic nearestDynamicObstacle = threateningObstacleList[0];

            // Get nearest threatening obstacle
            for (int i = 0; i < threateningObstacleList.Count; i++)
            {
                dynamicObstacle = threateningObstacleList[i];
                nearestSqrDist = (transform.position - nearestDynamicObstacle.transform.position).sqrMagnitude;
                sqrDist = (transform.position - dynamicObstacle.transform.position).sqrMagnitude;

                if (sqrDist < nearestSqrDist)
                {
                    nearestDynamicObstacle = dynamicObstacle;
                }
            }

            return nearestDynamicObstacle;
        }

        #endregion
    }
}