using UnityEngine;
using System.Collections.Generic;

namespace KD_Steering
{
    public class SteeringCrowdUnit : MonoBehaviour
    {
        #region Attributes

        // Delta time before last velocity update
        private float m_DeltaTimeBeforeLastVelocityUpdate = 0;

        // Velocity
        private Vector3 m_Velocity = Vector3.zero;

        // Old position
        private Vector3 m_OldPosition = Vector3.zero;

        // Rigidbody
        private Rigidbody m_Rigidbody = null;

        // Crowd unit list
        private static List<SteeringCrowdUnit> s_CrowUnitList = new List<SteeringCrowdUnit>();

        #endregion

        #region Getters & Setters

        // Orientation
        public Vector3 Orientation
        {
            get { return transform.forward; }
        }

        // Velocity
        public Vector3 Velocity
        {
            get
            {
                if (m_Rigidbody != null)
                {
                    if (m_Rigidbody.velocity != Vector3.zero)
                    {
                        return m_Rigidbody.velocity;
                    }
                }

                return m_Velocity;
            }
        }

        // Crow unit list
        public static List<SteeringCrowdUnit> CrowdUnitList
        {
            get { return s_CrowUnitList; }
        }

        #endregion

        #region MonoBehaviour

        // Called at creation
        void Awake()
        {
            s_CrowUnitList.Add(this);
        }

        // Use this for initialization
        void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            m_DeltaTimeBeforeLastVelocityUpdate += Time.deltaTime;

            if ((transform.position - m_OldPosition).sqrMagnitude > 0.01f)
            {
                m_Velocity = (transform.position - m_OldPosition) / m_DeltaTimeBeforeLastVelocityUpdate;
                m_OldPosition = transform.position;
                m_DeltaTimeBeforeLastVelocityUpdate = 0;
            }
        }

        // On destroy
        void OnDestroy()
        {
            if (s_CrowUnitList.Contains(this))
            {
                s_CrowUnitList.Remove(this);
            }
        }

        #endregion
    }
}