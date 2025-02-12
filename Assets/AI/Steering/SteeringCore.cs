using UnityEngine;
using System.Collections.Generic;

namespace KD_Steering
{
    [RequireComponent(typeof(Rigidbody))]
    public class SteeringCore : MonoBehaviour
    {
        #region Attributes

        // Steering behavior list
        private List<SteeringBehavior> m_SteeringBehaviorList = new List<SteeringBehavior>();

        // Rigidbody
        private Rigidbody m_Rigidbody = null;

        // Current velocity
        //private Vector3 m_Velocity = Vector3.zero;

        [Header("Steering Settings")]

        // Max steering force
        [SerializeField]
        private float m_MaxSteeringForce = 1;

        // Max speed
        [SerializeField]
        private float m_MaxSpeed = 4;

        // Target
        private Vector3 m_Target = Vector3.zero;

        // Target transform
        private Transform m_TargetTransform = null;

        [Header("Tweak Settings")]

        // Rotation sync scale
        [Tooltip("Scale the rotation speed to look at velocity direction")]
        [SerializeField]
        private float m_RotationSyncScale = 5;

        [Header("Debug")]

        // Use debug
        [SerializeField]
        private bool m_UseDebug = false;

        // Debug target transform
        [SerializeField]
        private Transform m_DebugTargetTransform = null;

        // Debug start velocity
        [SerializeField]
        private Vector3 m_DebugStartVelocity = Vector3.zero;

        #endregion

        #region Getters & Setters

        // Rigidbody
        /*public Rigidbody Rigidbody
        {
            get { return m_Rigidbody; }
        }*/

        // Max steering force
        public float MaxSteeringForce
        {
            get { return m_MaxSteeringForce; }
        }

        // Current velocity
        public Vector3 Velocity
        {
            //get { return m_Rigidbody.velocity; }
            get { return m_Rigidbody.velocity;/* m_Velocity;*/ }
        }

        // Target transform
        public Transform TargetTransform
        {
            get { return m_TargetTransform; }
            set { m_TargetTransform = value; }
        }

        // Target position
        public Vector3 Target
        {
            get { return m_Target; }
            set { m_Target = value; }
        }

        // Max speed
        public float MaxSpeed
        {
            get { return m_MaxSpeed; }
        }

        #endregion

        #region MonoBehaviour

        // Use this for initialization
        void Start()
        {
            // Get rigidbody component
            m_Rigidbody = GetComponent<Rigidbody>();

            // Get steering behavior components
            GetSteeringBehaviors();

            // Debug
            if (m_UseDebug)
            {
                PerformDebugStart();
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Debug
            if (m_UseDebug)
            {
                PerformDebugUpdate();
            }

            // Update steering behaviors
            UpdateSteering();

            // Apply steering
            ApplySteering();
        }

        // Gizmos
        void OnDrawGizmos()
        {
            Vector3 steeringForceAverage = Vector3.zero;
            float priorityScale = 1;

            for (int i = 0; i < m_SteeringBehaviorList.Count; i++)
            {
                if (m_SteeringBehaviorList.Count > 1)
                {
                    priorityScale = m_SteeringBehaviorList[i].BlendScale;
                }

                steeringForceAverage += m_SteeringBehaviorList[i].SteeringForce * priorityScale;
            }

            steeringForceAverage.y = 0;
            //steeringForceAverage = Vector3.ClampMagnitude(steeringForceAverage, m_MaxSteeringForce);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + steeringForceAverage);
        }

        #endregion

        #region Private Manipulators

        /// <summary>
        /// Apply steering
        /// </summary>
        private void ApplySteering()
        {
            // Get steering force average
            Vector3 steeringForceAverage = Vector3.zero;
            float priorityScale = 1;

            for (int i = 0; i < m_SteeringBehaviorList.Count; i++)
            {
                if (m_SteeringBehaviorList.Count > 1)
                {
                    priorityScale = m_SteeringBehaviorList[i].BlendScale;
                }

                steeringForceAverage += m_SteeringBehaviorList[i].SteeringForce * priorityScale;
            }

            steeringForceAverage.y = 0;
            steeringForceAverage = Vector3.ClampMagnitude(steeringForceAverage, m_MaxSteeringForce);

            // Add steering force to velocity
            m_Rigidbody.velocity += steeringForceAverage;
            //m_Velocity += steeringForceAverage;
            //transform.position += m_Velocity * Time.deltaTime;

            // Update rotation
            if (m_Rigidbody.velocity.sqrMagnitude > 1)
            //if (m_Velocity != Vector3.zero)
            {
                //Vector3 velocityNoY = m_Rigidbody.velocity;
                //Vector3 velocityNoY = m_Velocity;
                //velocityNoY.y = 0;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(m_Rigidbody.velocity), Time.deltaTime * m_RotationSyncScale);
            }
        }

        /// <summary>
        /// Get steering behavior components
        /// </summary>
        private void GetSteeringBehaviors()
        {
            SteeringBehavior[] steeringBehaviors = GetComponents<SteeringBehavior>();

            for (int i = 0; i < steeringBehaviors.Length; i++)
            {
                m_SteeringBehaviorList.Add(steeringBehaviors[i]);
            }
        }

        /// <summary>
        /// Update all steering behaviors
        /// </summary>
        private void UpdateSteering()
        {
            for (int i = 0; i < m_SteeringBehaviorList.Count; i++)
            {
                if (m_SteeringBehaviorList[i].enabled)
                {
                    m_SteeringBehaviorList[i].PerformSteeringBehavior();
                }
            }
        }

        #endregion

        #region Debug

        /// <summary>
        /// Call debug methods in start
        /// </summary>
        private void PerformDebugStart()
        {
            // Debug set velocity
            DebugSetVelocity();
        }

        /// <summary>
        /// Call debug methods in update
        /// </summary>
        private void PerformDebugUpdate()
        {
            // Debug target
            DebugSetTarget();
        }

        /// <summary>
        /// Set debug start velocity
        /// </summary>
        private void DebugSetVelocity()
        {
            m_Rigidbody.velocity = m_DebugStartVelocity;
            //m_Velocity = m_DebugStartVelocity;
        }

        /// <summary>
        /// Set debug target
        /// </summary>
        private void DebugSetTarget()
        {
            if (m_DebugTargetTransform != null)
            {
                m_Target = m_DebugTargetTransform.position;
                m_TargetTransform = m_DebugTargetTransform;
            }
        }

        #endregion
    }
}