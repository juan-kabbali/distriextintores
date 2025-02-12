using UnityEngine;

namespace KD_Steering
{
    public class SteeringBehavior_Arrival : SteeringBehavior
    {
        #region Attributes

        // Slowing distance
        [SerializeField]
        private float m_SlowingDistance = 10;

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

            if (SteeringCore.Target == null)
            {
                return;
            }

            // Calculate stopping factor
            float TargetDistance = (SteeringCore.Target - transform.position).magnitude;
            float stoppingFactor;

            if (m_SlowingDistance > 0)
            {
                stoppingFactor = Mathf.Clamp(TargetDistance / m_SlowingDistance, 0.0f, 1.0f);
            }
            else
            {
                stoppingFactor = Mathf.Clamp(TargetDistance, 0.0f, 1.0f);
            }

            m_DesiredVelocity = (SteeringCore.Target - transform.position).normalized * SteeringCore.MaxSpeed * stoppingFactor;

            // Calculate steering force
            SteeringForce = m_DesiredVelocity - SteeringCore.Velocity;
        }

        #endregion

        #region MonoBehaviour

        // Gizmos
        void OnDrawGizmos()
        {
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