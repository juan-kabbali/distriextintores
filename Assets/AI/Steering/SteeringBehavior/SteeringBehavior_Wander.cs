using UnityEngine;

namespace KD_Steering
{
    public class SteeringBehavior_Wander : SteeringBehavior
    {
        #region Attributes

        // Wander circle radius
        [Tooltip("Radius of wander circle in front of character")]
        [SerializeField]
        private float   m_WanderRadius          = 5;

        // Wander distance
        [Tooltip("Distane between character and wander circle")]
        [SerializeField]
        private float   m_WanderDistance        = 5;

        // Wander angle
        //private float   m_WanderAngle           = 0;

        // Wander angle change
        //[Tooltip("Angle variation for wander")]
        //[SerializeField]
        //private float   m_WanderAngleChange     = 1;

        // Random point frequency
        [Tooltip("Frequency in seconds to generate a new random point")]
        [SerializeField]
        private float   m_RandomPointFrequency  = 1;

        // Timer
        private float   m_Timer                 = 0;

        // Random point
        private Vector3 m_RandomPoint           = Vector3.zero;

        // Wander circle position
        private Vector3 m_WanderCirclePosition  = Vector3.zero;

        // Wander offset
        //private Vector3 m_WanderOffset          = Vector3.zero;

        // Desired velocity
        private Vector3 m_DesiredVelocity       = Vector3.zero;

        #endregion

        #region SteeringBehavior Override

        /// <summary>
        /// Wander behavior
        /// </summary>
        public override void PerformSteeringBehavior()
        {
            if (SteeringCore == null)
            {
                return;
            }

            // Calculate desired velocity
            m_WanderCirclePosition = transform.position + transform.forward * m_WanderDistance;
            /*
            m_WanderOffset = transform.forward * m_WanderRadius;
            m_WanderOffset = Quaternion.AngleAxis(m_WanderAngle, Vector3.up) * m_WanderOffset;
            m_WanderAngle += Random.Range(-m_WanderAngleChange, m_WanderAngleChange);
            */
            m_DesiredVelocity = (m_RandomPoint - transform.position).normalized * SteeringCore.MaxSpeed;
            //m_DesiredVelocity = (m_WanderCirclePosition + m_WanderOffset - transform.position).normalized * SteeringCore.MaxSpeed;

            // Calculate steering force
            SteeringForce = m_DesiredVelocity - SteeringCore.Velocity;

            // Update timer
            UpdateTimer();
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

            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, transform.position + SteeringCore.Velocity.normalized * m_WanderDistance);
            Gizmos.DrawWireSphere(transform.position + SteeringCore.Velocity.normalized * m_WanderDistance, m_WanderRadius);
            Gizmos.DrawWireSphere(m_RandomPoint, 0.33f);
            Gizmos.DrawLine(transform.position + SteeringCore.Velocity.normalized * m_WanderDistance, m_RandomPoint);
        }

        #endregion

        #region Private Manipulators

        /// <summary>
        /// Update timer. Generate new random point if time reached
        /// </summary>
        private void UpdateTimer()
        {
            m_Timer += Time.deltaTime;

            if (m_Timer > m_RandomPointFrequency)
            {
                m_RandomPoint = Random.insideUnitCircle * m_WanderRadius;
                m_RandomPoint.z = m_RandomPoint.y;
                m_RandomPoint.y = 0;
                m_RandomPoint += m_WanderCirclePosition;
                m_Timer = 0;
            }
        }

        #endregion
    }
}