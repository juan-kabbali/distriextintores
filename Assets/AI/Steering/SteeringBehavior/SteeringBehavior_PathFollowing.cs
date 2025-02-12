using System.Collections.Generic;
using UnityEngine;

namespace KD_Steering
{
    public class SteeringBehavior_PathFollowing : SteeringBehavior
    {
        #region Attributes

        // Path to follow
        [SerializeField]
        private SteeringPath m_PathToFollow = null;

        // Normal scale on anticipation
        [Tooltip("Anticpation distance scale importance to smooth replacement")]
        [SerializeField]
        private float m_NormalScaleOnAnticipation = 1.5f;

        // Steering force scale on anticipation
        [Tooltip("Anticpation result force importance to smooth replacement")]
        [SerializeField]
        private float m_SteeringForceScaleOnAnticipation = 0.01f;

        // Normal point
        private Vector3 m_NormalPoint = Vector3.zero;

        // Desired velocity
        private Vector3 m_DesiredVelocity = Vector3.zero;

        #endregion

        #region SteeringBehavior Override

        /// <summary>
        /// Path following behavior
        /// </summary>
        public override void PerformSteeringBehavior()
        {
            if (SteeringCore == null || m_PathToFollow == null)
            {
                return;
            }

            if (m_PathToFollow.PointList.Count <= 1)
            {
                return;
            }

            // Get nearest point on path
            Vector3 nearestPoint;
            Vector3 previousPoint;
            Vector3 nextPoint;
            Vector3 pathLine;
            Vector3 normalPointA;
            Vector3 normalPointB;

            GetNearestPointOnPath(out nearestPoint, out previousPoint, out nextPoint);
            normalPointA = GetNormalPoint(previousPoint, nearestPoint);
            normalPointB = GetNormalPoint(nearestPoint, nextPoint);

            if (previousPoint == Vector3.zero)
            {
                normalPointA = normalPointB;
            }

            Vector3 predictedPosition = transform.position + SteeringCore.Velocity;
            m_NormalPoint = GetNearestPoint(normalPointA, normalPointB);
            Vector3 normal = m_NormalPoint - predictedPosition;

            if (m_NormalPoint == normalPointA)
            {
                pathLine = nearestPoint - previousPoint;
            }
            else
            {
                pathLine = nextPoint - nearestPoint;
            }

            if (normal.sqrMagnitude > m_PathToFollow.Radius * m_PathToFollow.Radius)
            {
                // Calculate desired velocity
                m_DesiredVelocity = (m_NormalPoint - transform.position).normalized * SteeringCore.MaxSpeed;

                // Calculate steering force
                SteeringForce = m_DesiredVelocity - SteeringCore.Velocity;
            }
            else if ((normal * m_NormalScaleOnAnticipation).sqrMagnitude > m_PathToFollow.Radius * m_PathToFollow.Radius)
            {
                // Calculate desired velocity
                m_DesiredVelocity = (m_NormalPoint - transform.position).normalized * SteeringCore.MaxSpeed;

                // Calculate steering force
                SteeringForce = m_DesiredVelocity - SteeringCore.Velocity;
                SteeringForce *= m_SteeringForceScaleOnAnticipation;
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

            // Future position
            Vector3 predictedPosition = transform.position + SteeringCore.Velocity;

            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, predictedPosition);
            Gizmos.DrawLine(predictedPosition, m_NormalPoint);

            Vector3 nearestPoint;
            Vector3 previousPoint;
            Vector3 nextPoint;

            GetNearestPointOnPath(out nearestPoint, out previousPoint, out nextPoint);

            Gizmos.DrawWireSphere(nearestPoint, 0.33f);

            if (previousPoint != Vector3.zero)
            {
                Gizmos.DrawWireSphere(previousPoint, 0.33f);
            }

            // Draw next point
            Gizmos.DrawWireSphere(nextPoint, 0.33f);

            Vector3 normalPointA = GetNormalPoint(previousPoint, nearestPoint);
            Vector3 normalPointB = GetNormalPoint(nearestPoint, nextPoint);

            Gizmos.color = new Color(1, 1, 1, 0.33f);
            // Draw normal point A
            Gizmos.DrawWireSphere(normalPointA, 0.33f);

            // Draw normal point B
            Gizmos.DrawWireSphere(normalPointB, 0.33f);

            // Draw used normal point
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(m_NormalPoint, 0.33f);
        }

        #endregion

        #region Private Manipulators

        /// <summary>
        /// Get nearest point on path, its previous and next point on path
        /// </summary>
        /// <param name="_NearestPoint">Nearest point on path</param>
        /// <param name="_PreviousPoint">Previous point on path</param>
        /// <param name="_NextPoint">Next point on path</param>
        private void GetNearestPointOnPath(out Vector3 _NearestPoint, out Vector3 _PreviousPoint, out Vector3 _NextPoint)
        {
            _NearestPoint = Vector3.zero;
            _PreviousPoint = Vector3.zero;
            _NextPoint = Vector3.zero;

            if (m_PathToFollow.PointList.Count <= 1)
            {
                return ;
            }

            _NearestPoint = m_PathToFollow.PointList[0];
            _NextPoint = m_PathToFollow.PointList[1];
            float sqrDist;
            float nearestSqrDist;

            for (int i = 0; i < m_PathToFollow.PointList.Count; i++)
            {
                sqrDist = (m_PathToFollow.PointList[i] - transform.position).sqrMagnitude;
                nearestSqrDist = (_NearestPoint - transform.position).sqrMagnitude;

                if (sqrDist < nearestSqrDist)
                {
                    _NearestPoint = m_PathToFollow.PointList[i];

                    if (i < m_PathToFollow.PointList.Count - 1)
                    {
                        _NextPoint = m_PathToFollow.PointList[i + 1];
                    }
                    
                    if (i > 0)
                    {
                        _PreviousPoint = m_PathToFollow.PointList[i - 1];
                    }
                }
            }
        }

        /// <summary>
        /// Get normal point from projection
        /// </summary>
        /// <param name="_StartPoint"></param>
        /// <param name="_EndPoint"></param>
        /// <returns></returns>
        private Vector3 GetNormalPoint(Vector3 _StartPoint, Vector3 _EndPoint)
        {
            Vector3 predictedPosition = transform.position + SteeringCore.Velocity;
            Vector3 nearestPointToPredictedPosition = predictedPosition - _StartPoint;
            Vector3 pathLine = _EndPoint - _StartPoint;
            Vector3 projection = Vector3.Project(nearestPointToPredictedPosition, pathLine);
            Vector3 normalPoint = _StartPoint + projection;

            if (Vector3.Dot(projection.normalized, pathLine.normalized) < 0)
            {
                normalPoint = _StartPoint;
            }
            else if (projection.magnitude > pathLine.magnitude)
            {
                normalPoint = _EndPoint;
            }

            return normalPoint;
        }

        /// <summary>
        /// Get nearest point from character predicted position
        /// </summary>
        /// <param name="_PointA">Point A in comparison</param>
        /// <param name="_PointB">Point B in comparison</param>
        /// <returns></returns>
        private Vector3 GetNearestPoint(Vector3 _PointA, Vector3 _PointB)
        {
            Vector3 predictedPosition = transform.position + SteeringCore.Velocity;
            float sqrDistA = (_PointA - predictedPosition).sqrMagnitude;
            float sqrDistB = (_PointB - predictedPosition).sqrMagnitude;

            return (sqrDistA < sqrDistB) ? _PointA : _PointB;
        }

        #endregion
    }
}