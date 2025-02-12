using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KD_Steering
{
    public class SteeringPath : MonoBehaviour
    {
        #region Enums

        // Making path method
        private enum EPathMakingMethod
        {
            FromTransformList,
            FromBezierCurve
        }

        #endregion

        #region Attributes

        // Path radius
        [Tooltip("Radius in wich a character must stay")]
        [SerializeField]
        private float m_Radius = 3;

        // Point list
        private List<Vector3> m_PointList = new List<Vector3>();

        // How path will be generated
        [Tooltip("How path will be generated")]
        [SerializeField]
        private EPathMakingMethod m_PathMakingMethod = EPathMakingMethod.FromTransformList;

        // Transform list
        [Tooltip("Transforms used if making path from transform list")]
        [SerializeField]
        private List<Transform> m_TransformList = new List<Transform>();

        #endregion

        #region  Getters & Setters

        // Radius
        public float Radius
        {
            get { return m_Radius; }
        }

        // Point list
        public List<Vector3> PointList
        {
            get { return m_PointList; }
        }

        #endregion

        #region MonoBehaviour

        // Use this for initialization
        void Start()
        {
            // Generate path
            GeneratePath();
        }

        // Gizmos
        void OnDrawGizmos()
        {
            GeneratePath();

            for (int i = 0; i < m_PointList.Count - 1; i++)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(m_PointList[i], m_PointList[i + 1]);

                Gizmos.color = Color.red;
                Vector3 leftBorderStartPoint = m_PointList[i] + Quaternion.AngleAxis(-90, Vector3.up) * (m_PointList[i + 1] - m_PointList[i]).normalized * m_Radius;
                Vector3 rightBorderStartPoint = m_PointList[i] + Quaternion.AngleAxis(90, Vector3.up) * (m_PointList[i + 1] - m_PointList[i]).normalized * m_Radius;
                Gizmos.DrawLine(leftBorderStartPoint, leftBorderStartPoint + m_PointList[i + 1] - m_PointList[i]);
                Gizmos.DrawLine(rightBorderStartPoint, rightBorderStartPoint + m_PointList[i + 1] - m_PointList[i]);
            }
        }

        #endregion

        #region Private Manipulators

        /// <summary>
        /// Generate path using transform list as points of path
        /// </summary>
        private void GeneratePathFromTransformList()
        {
            for (int i = 0; i < m_TransformList.Count; i++)
            {
                if (m_PointList[i] != null)
                {
                    m_PointList.Add(m_TransformList[i].position);
                }
            }
        }

        /// <summary>
        /// Generate path using selected method
        /// </summary>
        private void GeneratePath()
        {
            m_PointList.Clear();

            if (m_PathMakingMethod == EPathMakingMethod.FromTransformList)
            {
                GeneratePathFromTransformList();
            }
        }

        #endregion
    }
}