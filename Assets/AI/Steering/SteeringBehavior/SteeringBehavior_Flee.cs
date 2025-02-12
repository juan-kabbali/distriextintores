using UnityEngine;

namespace KD_Steering
{
    public class SteeringBehavior_Flee : SteeringBehavior
    {
        #region Attributes

        // Desired velocity
        private Vector3 m_DesiredVelocity = Vector3.zero;

        #endregion

        #region SteeringBehavior Override

        /// <summary>
        /// Flee behavior
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

            // Calculate desired velocity
            m_DesiredVelocity = -(SteeringCore.Target - transform.position).normalized * SteeringCore.MaxSpeed;

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
            Gizmos.DrawLine(transform.position, transform.position - m_DesiredVelocity);

            //if (SteeringCore.Rigidbody != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + SteeringCore.Velocity);
            }
        }

        #endregion
    }
}