using UnityEngine;

    public class PathFinder : MonoBehaviour
    {
        #region Attributes

        // Path calculation frequency
        [Tooltip("Frequency in seconds for recalculating path")]
        [SerializeField]
        private float           m_PathCalculationFrequency          = 1;

        // Timer
        private float           m_Timer                             = 0;

        // Nav mesh path
        private UnityEngine.AI.NavMeshPath     m_NavMeshPath                       = null;

        // Destination
        private Vector3         m_Destination                       = Vector3.zero;

        // Destination set
        private bool            m_DestinationSet                    = false;

        // Destination sample position distance
        [Tooltip("Distance max to find closest point on nav mesh is wanted destination is not on nav mesh")]
        [SerializeField]
        private float           m_DestinationSamplePositionDistance = 32;

        // Enable gizmos
        [SerializeField]
        private bool            m_EnableGizmos                      = false;

        #endregion

        #region MonoBehaviour

        // Awake
        void Awake()
        {
            m_NavMeshPath = new UnityEngine.AI.NavMeshPath();
        }

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            // Update timer
            UpdateTimer(Time.deltaTime);
        }

        // On draw gizmos
        void OnDrawGizmos()
        {
            if (m_EnableGizmos && m_NavMeshPath != null)
            {
                Gizmos.color = Color.yellow;

                for (int i = 0; i < m_NavMeshPath.corners.Length; i++)
                {
                    Gizmos.DrawWireSphere(m_NavMeshPath.corners[i], 0.33f);

                    if (i < m_NavMeshPath.corners.Length - 1)
                    {
                        Gizmos.DrawLine(m_NavMeshPath.corners[i], m_NavMeshPath.corners[i + 1]);
                    }
                }
            }
        }

        #endregion

        #region Public Manipulators

        /// <summary>
        /// Reset destination and avoid any path calculation
        /// </summary>
        public void ResetDestination()
        {
            m_DestinationSet = false;
        }

        /// <summary>
        /// Set destination for path calculation and perform a first path calculation
        /// </summary>
        /// <param name="_Destination">Destination</param>
        public void SetDestination(Vector3 _Destination)
        {
            UnityEngine.AI.NavMeshHit navMeshHit;

            if (UnityEngine.AI.NavMesh.SamplePosition(_Destination, out navMeshHit, m_DestinationSamplePositionDistance, UnityEngine.AI.NavMesh.AllAreas))
            {
                m_Destination = _Destination;
                m_DestinationSet = true;
                CalculatePath();
            }
            else
            {
                ResetDestination();
            }
        }

        /// <summary>
        /// Get path
        /// </summary>
        /// <returns></returns>
        public Vector3[] GetPath()
        {
            return m_NavMeshPath.corners;
        }

        #endregion

        #region Private Manipulators

        /// <summary>
        /// Calculate a path
        /// </summary>
        private void CalculatePath()
        {
            if (m_DestinationSet)
            {
                UnityEngine.AI.NavMesh.CalculatePath(transform.position, m_Destination, UnityEngine.AI.NavMesh.AllAreas, m_NavMeshPath);
                
            }
        }

        /// <summary>
        /// Update timer adding specified delta time. Calculate path if waiting time reached
        /// </summary>
        /// <param name="_DeltaTime"></param>
        private void UpdateTimer(float _DeltaTime)
        {
            m_Timer += _DeltaTime;

            if (m_Timer > m_PathCalculationFrequency)
            {
                CalculatePath();
                m_Timer = 0;
            }
        }

        #endregion
    }
