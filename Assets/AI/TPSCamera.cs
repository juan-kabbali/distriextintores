using UnityEngine;
using System.Collections;

public class TPSCamera : MonoBehaviour
{
    #region Attributes

        // Target
        [SerializeField]
        private Transform   m_Target                    = null;

        // Offset
        [SerializeField]
        private Vector3     m_TargetOffset              = new Vector3(0, 1, -6);

        // Lerp factor
        [SerializeField]
        private float       m_LerpFactor                = 6;

    #endregion

    #region MonoBehaviour

        // Called once per frame
        void FixedUpdate()
        {
            //  Camera follow character. Written in fixed update to avoid camera lerp break
            if (m_Target != null)
            {
                // Calculate local offset depending on dodge action
                Vector3 localOffset = m_Target.transform.right * m_TargetOffset.x + m_Target.transform.up * m_TargetOffset.y + m_Target.transform.forward * m_TargetOffset.z;

                // Update position based on offset
                Vector3 desiredPosition = m_Target.transform.position + localOffset;
                transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.fixedDeltaTime * m_LerpFactor);

                // Follow character rotation depending on dodge action
                transform.rotation = Quaternion.Lerp(transform.rotation, m_Target.transform.rotation, Time.fixedDeltaTime * m_LerpFactor);
            }
        }

    #endregion
}
