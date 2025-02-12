using System.Collections.Generic;
using UnityEngine;

namespace KD_Steering
{
    public class SteeringBehavior_FlowFieldFollowing : SteeringBehavior
    {
        #region Attributes

        // Desired velocity
        private Vector3 m_DesiredVelocity = Vector3.zero;

        #endregion

        #region SteeringBehavior Override

        /// <summary>
        /// Flow field following behavior
        /// </summary>
        public override void PerformSteeringBehavior()
        {
            if (SteeringCore == null)
            {
                return;
            }

            // Get flow field zone containing character
            SteeringFlowFieldZone flowFieldZone = GetFlowFieldZoneEntered();

            if (flowFieldZone != null)
            {
                // Calculate desired velocity
                m_DesiredVelocity = (flowFieldZone.Direction).normalized * SteeringCore.MaxSpeed;

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
        /// Get flow field zone in wich character will be present
        /// </summary>
        /// <returns>Flow field zone in wich character will be present, null if in no flow field zone</returns>
        private SteeringFlowFieldZone GetFlowFieldZoneEntered()
        {
            List<SteeringFlowFieldZone> flowFieldZoneList = SteeringFlowFieldZone.FlowFieldZoneList;

            // Find flow field where character will be in future position
            for (int i = 0; i < flowFieldZoneList.Count; i++)
            {
                if (flowFieldZoneList[i].IsInBounds(transform.position))
                {
                    return flowFieldZoneList[i];
                }
            }

            return null;
        }

        #endregion
    }
}