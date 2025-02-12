using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AutoMovable : MonoBehaviour
{
    #region Attributes

    // Rigidbody
    private Rigidbody m_Rigidbody = null;

    // Max speed
    [SerializeField]
    private float m_MaxSpeed = 4;

    #endregion

    #region MonoBehaviour

    // Use this for initialization
    void Start()
    {
        // Get rigidbody component
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Set velocity
        SetVelocity();
    }

    #endregion

    #region Private Manipulators

    /// <summary>
    /// Set velocity
    /// </summary>
    private void SetVelocity()
    {
        Vector3 velocity = transform.forward * m_MaxSpeed;
        velocity.y = m_Rigidbody.velocity.y;
        m_Rigidbody.velocity = velocity;
    }

    #endregion
}
