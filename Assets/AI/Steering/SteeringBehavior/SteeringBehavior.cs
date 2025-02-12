using UnityEngine;

namespace KD_Steering
{
    [RequireComponent(typeof(SteeringCore))]
    public abstract class SteeringBehavior : MonoBehaviour
    {
        #region Attributes

        // Blend scale
        [Tooltip("Behavior priority when calculating steering with other behaviors")]
        [Range(1, 100)]
        [SerializeField]
        private uint m_BlendScale = 1;

        // Steering core
        private SteeringCore m_SteeringCore = null;

        // Steering force
        private Vector3 m_SteeringForce = Vector3.zero;

        #endregion

        #region Getters & Setters

        // Priority
        public float BlendScale
        {
            get { return m_BlendScale; }
        }

        // Steering force
        public Vector3 SteeringForce
        {
            get { return m_SteeringForce; }
            set { m_SteeringForce = value; }
        }

        // Steering core
        protected SteeringCore SteeringCore
        {
            get { return m_SteeringCore; }
        }

        #endregion

        #region MonoBehaviour

        // Use this for initialization
        void Start()
        {
            m_SteeringCore = GetComponent<SteeringCore>();
        }

        #endregion

        #region Public Manipulators

        /// <summary>
        /// Perform steering behavior
        /// </summary>
        public abstract void PerformSteeringBehavior();

        #endregion
    }
}