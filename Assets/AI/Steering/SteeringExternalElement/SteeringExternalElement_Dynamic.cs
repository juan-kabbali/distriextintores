using System.Collections.Generic;
using UnityEngine;

namespace KD_Steering
{
    public class SteeringExternalElement_Dynamic : MonoBehaviour
    {
        #region Attributes

        // Bounding sphere radius
        [SerializeField]
        private float m_BoundingSphereRadius = 1;

        // Old position
        private Vector3 m_OldPosition = Vector3.zero;

        // Rigidbody
        private Rigidbody m_Rigidbody = null;

        // Steering dynamic external element list
        private static List<SteeringExternalElement_Dynamic> s_ElementList = new List<SteeringExternalElement_Dynamic>();

        #endregion

        #region Getters & Setters

        // Bounding sphere radius
        public float BoundingSphereRadius
        {
            get { return m_BoundingSphereRadius; }
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

                return (transform.position - m_OldPosition) / Time.deltaTime;
            }
        }

        // Steering dynamic external element list
        public static List<SteeringExternalElement_Dynamic> ElementList
        {
            get { return s_ElementList; }
        }

        #endregion

        #region MonoBehaviour

        // Called at creation
        void Awake()
        {
            s_ElementList.Add(this);
        }

        // Use this for initialization
        void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            m_OldPosition = transform.position;
        }

        // On destroy
        void OnDestroy()
        {
            if (s_ElementList.Contains(this))
            {
                s_ElementList.Remove(this);
            }
        }

        // Gizmos
        void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, m_BoundingSphereRadius);
        }

        #endregion
    }
}