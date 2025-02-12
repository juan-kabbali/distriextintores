using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KD_Steering
{
    public class SteeringFlowFieldZone : MonoBehaviour
    {
        #region Attributes

        // Direction
        private Vector3 m_Direction = Vector3.zero;

        // Bounding box
        [Tooltip("Bounding box in wich the zone is active if character enter")]
        [SerializeField]
        private Bounds m_Bounds = new Bounds(Vector3.zero, new Vector3(1, 1, 1));

        // Flow field zone list
        private static List<SteeringFlowFieldZone> s_FlowFieldZoneList = new List<SteeringFlowFieldZone>();

        #endregion

        #region Getters & Setters

        // Direction
        public Vector3 Direction
        {
            get { return m_Direction; }
            set { m_Direction = value.normalized; }
        }

        // Flow field zone list
        public static List<SteeringFlowFieldZone> FlowFieldZoneList
        {
            get { return s_FlowFieldZoneList; }
        }

        #endregion

        #region MonoBehaviour

        // Called at creation
        void Awake()
        {
            // Add this to flow field zone list
            s_FlowFieldZoneList.Add(this);
        }

        // Use this for initialization
        void Start()
        {

        }

        // On destroy
        void OnDestroy()
        {
            // Remove this of flow field zone
            if (s_FlowFieldZoneList.Contains(this))
            {
                s_FlowFieldZoneList.Remove(this);
            }
        }

        // Gizmos
        void OnDrawGizmos()
        {
            GizmosDrawBounds(transform.position, true, 1, m_Direction);
        }

        #endregion

        #region Public Manipulators

        /// <summary>
        /// Know if specified position is in bounds
        /// </summary>
        /// <param name="_Position">Position to test in bounds</param>
        /// <returns>True if specified position is in bounds, false otherwise</returns>
        public bool IsInBounds(Vector3 _Position)
        {
            _Position = transform.InverseTransformPoint(_Position);
            return m_Bounds.Contains(_Position);
        }

        /// <summary>
        /// Draw bounds in gizmos
        /// </summary>
        /// <param name="_Position">Position to draw</param>
        public void GizmosDrawBounds(Vector3 _Position, bool _IsOnScene, float _ColorAlpa, Vector3 _Direction)
        {
            if (_IsOnScene)
            {
                _Position = Vector3.zero;
            }

            Vector3 topFrontRight = _Position + transform.TransformPoint(m_Bounds.center + m_Bounds.extents);
            Vector3 topFrontLeft = _Position + transform.TransformPoint(m_Bounds.center + Vector3.Scale(m_Bounds.extents, new Vector3(-1, 1, 1)));
            Vector3 topBackRight = _Position + transform.TransformPoint(m_Bounds.center + Vector3.Scale(m_Bounds.extents, new Vector3(1, 1, -1)));
            Vector3 topBackLeft = _Position + transform.TransformPoint(m_Bounds.center + Vector3.Scale(m_Bounds.extents, new Vector3(-1, 1, -1)));
            Vector3 bottomFrontRight = _Position + transform.TransformPoint(m_Bounds.center + Vector3.Scale(m_Bounds.extents, new Vector3(1, -1, 1)));
            Vector3 bottomFrontLeft = _Position + transform.TransformPoint(m_Bounds.center + Vector3.Scale(m_Bounds.extents, new Vector3(-1, -1, 1)));
            Vector3 bottomBackRight = _Position + transform.TransformPoint(m_Bounds.center + Vector3.Scale(m_Bounds.extents, new Vector3(1, -1, -1)));
            Vector3 bottomBackLeft = _Position + transform.TransformPoint(m_Bounds.center + Vector3.Scale(m_Bounds.extents, new Vector3(-1, -1, -1)));

            Gizmos.color = Color.magenta * new Color(1, 1, 1, _ColorAlpa);
            Gizmos.DrawLine(topFrontLeft, topFrontRight);
            Gizmos.DrawLine(bottomFrontLeft, bottomFrontRight);
            Gizmos.DrawLine(topBackLeft, topBackRight);
            Gizmos.DrawLine(bottomBackLeft, bottomBackRight);
            Gizmos.DrawLine(topFrontLeft, topBackLeft);
            Gizmos.DrawLine(topFrontRight, topBackRight);
            Gizmos.DrawLine(bottomFrontLeft, bottomBackLeft);
            Gizmos.DrawLine(bottomFrontRight, bottomBackRight);
            Gizmos.DrawLine(topFrontLeft, bottomFrontLeft);
            Gizmos.DrawLine(topFrontRight, bottomFrontRight);
            Gizmos.DrawLine(topBackLeft, bottomBackLeft);
            Gizmos.DrawLine(topBackRight, bottomBackRight);

            if (_IsOnScene)
            {
                _Position = transform.position;
            }

            Gizmos.color = Color.green * new Color(1, 1, 1, _ColorAlpa);
            Gizmos.DrawLine(_Position + m_Bounds.center + transform.up * m_Bounds.extents.y, _Position + m_Bounds.center + transform.up * m_Bounds.extents.y + _Direction);
            Gizmos.DrawLine(_Position + m_Bounds.center + transform.up * m_Bounds.extents.y + _Direction, _Position + m_Bounds.center + transform.up * m_Bounds.extents.y + _Direction + Quaternion.AngleAxis(45, transform.up) * -_Direction.normalized * 0.5f);
            Gizmos.DrawLine(_Position + m_Bounds.center + transform.up * m_Bounds.extents.y + _Direction, _Position + m_Bounds.center + transform.up * m_Bounds.extents.y + _Direction + Quaternion.AngleAxis(-45, transform.up) * -_Direction.normalized * 0.5f);
        }

        #endregion
    }
}