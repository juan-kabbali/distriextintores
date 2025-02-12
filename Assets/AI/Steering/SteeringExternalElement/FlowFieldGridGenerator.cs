using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KD_Steering
{
    public class FlowFieldGridGenerator : MonoBehaviour
    {
        #region Enums

        private enum EGenerationMethod
        {
            DirectionList,
            Random,
            PerlinNoise,
            CircularGravity
        }

        #endregion

        #region Attributes

        // Grid size
        [Tooltip("Number of object to generate per dimension")]
        [SerializeField]
        private Vector3 m_GridSize = Vector3.zero;

        // Speration space
        [Tooltip("Space between 2 generated object")]
        [SerializeField]
        private float m_SeparationSpace = 1;

        // Flow field zone prefab
        [Tooltip("Flow field zone prefab used for generation")]
        [SerializeField]
        private SteeringFlowFieldZone m_FlowFieldZonePrefab = null;

        // Generation method
        [Tooltip("Method that will be used to generate flow field zones direction")]
        [SerializeField]
        private EGenerationMethod m_GenerationMethod = EGenerationMethod.DirectionList;

        [Header("Direction list generation settings")]

        // Flow field zone direction list
        [Tooltip("Set manually direction for each generated flow field zone")]
        [SerializeField]
        private List<Vector3> m_DirectionList = new List<Vector3>();

        [Header("Random generation settings")]

        // Random direction axis constraints
        [Tooltip("Is random direction used, the random will be calculated only on axis if 1 is set, 0 will not use the axis")]
        [SerializeField]
        private Vector3 m_RandomDirectionAxisConstraints = new Vector3(1, 0, 1);

        [Header("Perlin noise generation settings")]

        // Perlin noise angle
        [SerializeField]
        private float m_PerlinNoiseAngle = 45;

        // Perlin noise parameters
        private float m_PerlinX;
        private float m_PerlinY;

        #endregion

        #region MonoBehaviour

        // Use this for initialization
        void Start()
        {
            // Generate grid
            GenerateGrid();
        }


        // Gizmos
        void OnDrawGizmos()
        {
            if (m_FlowFieldZonePrefab != null)
            {
                int sumIndex = 0;
                for (int i = 0; i < m_GridSize.x; i++)
                {
                    for (int j = 0; j < m_GridSize.y; j++)
                    {
                        for (int k = 0; k < m_GridSize.z; k++)
                        {
                            Vector3 direction = GetDirection(sumIndex, i, j, k);
                            m_FlowFieldZonePrefab.GizmosDrawBounds(transform.position + new Vector3(i * m_SeparationSpace, j * m_SeparationSpace, k * m_SeparationSpace), false, 0.33f, direction.normalized);
                            sumIndex++;
                            m_PerlinY = k * 0.1f;
                        }
                    }

                    m_PerlinX = i * 0.1f;
                }
            }
        }

        #endregion

        #region Private Manipulators

        /// <summary>
        /// Generate grid using grid size and separation space settings
        /// </summary>
        private void GenerateGrid()
        {
            if (m_FlowFieldZonePrefab != null)
            {
                int sumIndex = 0;
                for (int i = 0; i < m_GridSize.x; i++)
                {
                    for (int j = 0; j < m_GridSize.y; j++)
                    {
                        for (int k = 0; k < m_GridSize.z; k++)
                        {
                            // Instantiate zone
                            SteeringFlowFieldZone zone = Instantiate(m_FlowFieldZonePrefab);
                            zone.transform.position = transform.position + new Vector3(i * m_SeparationSpace, j * m_SeparationSpace, k * m_SeparationSpace);
                            zone.transform.parent = transform;

                            // Set zone direction
                            Vector3 direction = GetDirection(sumIndex, i, j, k);
                            zone.Direction = direction;

                            sumIndex++;
                            m_PerlinY = k * 0.1f;
                        }
                    }
                    m_PerlinX = i * 0.1f;
                }
            }
        }

        /// <summary>
        /// Get direction for floaw field zone at specified index
        /// </summary>
        /// <param name="_SumIndex"></param>
        /// <returns></returns>
        private Vector3 GetDirection(int _SumIndex, int _IndexX, int _IndexY, int _IndexZ)
        {
            Vector3 direction = Vector3.zero;

            switch (m_GenerationMethod)
            {
                case EGenerationMethod.Random:

                    float x = Random.Range(-1.0f, 1.0f) * Mathf.Clamp(m_RandomDirectionAxisConstraints.x, 0, 1);
                    float y = Random.Range(-1.0f, 1.0f) * Mathf.Clamp(m_RandomDirectionAxisConstraints.y, 0, 1);
                    float z = Random.Range(-1.0f, 1.0f) * Mathf.Clamp(m_RandomDirectionAxisConstraints.z, 0, 1);
                    direction = new Vector3(x, y, z);

                    break;

                case EGenerationMethod.DirectionList:

                    if (_SumIndex < m_DirectionList.Count)
                    {
                        direction = m_DirectionList[_SumIndex];
                    }

                    break;

                case EGenerationMethod.PerlinNoise:

                    float noiseX = _SumIndex / (m_GridSize.x + m_GridSize.y + m_GridSize.z) / m_GridSize.x;
                    float noiseY = _SumIndex / (m_GridSize.x + m_GridSize.y + m_GridSize.z) / m_GridSize.z;
                    float noise = Mathf.PerlinNoise(m_PerlinX, m_PerlinY) * m_PerlinNoiseAngle * Mathf.Deg2Rad;
                    direction = new Vector3(Mathf.Cos(noise), 0, Mathf.Sin(noise));

                    break;

                case EGenerationMethod.CircularGravity:

                    int midIndexX = (int)(m_GridSize.x * 0.5f);
                    int midIndexY = (int)(m_GridSize.y * 0.5f);
                    int midIndexZ = (int)(m_GridSize.z * 0.5f);

                    Vector3 currentZonePosition = new Vector3(m_SeparationSpace * _IndexX, m_SeparationSpace * _IndexY, m_SeparationSpace * _IndexZ);
                    Vector3 midZonePosition = new Vector3(m_SeparationSpace * midIndexX, m_SeparationSpace * midIndexY, m_SeparationSpace * midIndexZ);
                    direction = midZonePosition - currentZonePosition;

                    break;
            }

            return direction;
        }

        #endregion
    }
}