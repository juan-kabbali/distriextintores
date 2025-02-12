using UnityEngine;

namespace KD_Steering
{
    public class SteeringBehavior_ObstacleAvoidance : SteeringBehavior
    {
        #region Attributes

        // Layer mask for collision detection
        [Tooltip("Layers that will be used or ignored in obstacle avoidance")]
        [SerializeField]
        private LayerMask m_LayerMask;

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

        // Steering force conservation duration
        [Tooltip("Time (in sec) needed to loose all old steering force if no more to apply. Avoid null force in 1 frame only")]
        [SerializeField]
        private float m_SteeringForceConservationDuration = 1;

        // Max floor angle
        [Tooltip("Max floor angle walkable, collision with higher angle will not be avoided")]
        [SerializeField]
        [Range(0, 90)]
        private float m_MaxFloorAngle = 45;

        // Steering force conservation timer
        private float m_SteeringForceConservationTimer = 0;

        // Old valid steering force
        private Vector3 m_OldValidSteeringForce = Vector3.zero;

        // Desired velocity
        private Vector3 m_DesiredVelocity = Vector3.zero;

        #endregion

        #region SteeringBehavior Override

        /// <summary>
        /// Obstacle avoidance behavior
        /// </summary>
        public override void PerformSteeringBehavior()
        {
            if (SteeringCore == null)
            {
                return;
            }

            // Get most threatening obstacle
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hitInfo;
            Vector3 avoidanceForce = Vector3.zero;

            // Calculate avoidance force
            if (Physics.SphereCast(ray, m_BoundingSphereRadius, out hitInfo, m_ObstacleMaxDistance, m_LayerMask))
            {
                if (Vector3.Angle(hitInfo.normal, transform.up) > m_MaxFloorAngle)
                {
                    avoidanceForce = Vector3.Reflect(SteeringCore.Velocity, hitInfo.normal);

                    if (Vector3.Dot(avoidanceForce, SteeringCore.Velocity) < -0.9f)
                    {
                        avoidanceForce = transform.right;
                    }
                }
            }

            if (avoidanceForce != Vector3.zero)
            {
                // Calculate desired velocity
                m_DesiredVelocity = (avoidanceForce).normalized * SteeringCore.MaxSpeed;

                // Calculate steering force
                SteeringForce = m_DesiredVelocity - SteeringCore.Velocity;
                m_OldValidSteeringForce = SteeringForce;
                m_SteeringForceConservationTimer = 0;
            }
            else
            {
                //SteeringForce *= m_SteeringForceConservation;
                /*
                m_SteeringForceConservationTimer += Time.deltaTime;

                if (m_SteeringForceConservationTimer > m_SteeringForceConservationDuration)
                {
                    m_SteeringForceConservationTimer = m_SteeringForceConservationDuration;
                }

                float ratio = 1 - (m_SteeringForceConservationTimer / m_SteeringForceConservationDuration);
                SteeringForce = m_OldValidSteeringForce * ratio;*/

                SteeringForce = Vector3.zero;
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
        }

        #endregion
    }
}