using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform m_target;
    public float m_smooth = 5f;

    Vector3 m_offset;
    LayerMask maskCharacter;

    void Start()
    {
        m_offset = transform.position - m_target.position;
    }

    private void FixedUpdate()
    {
        Vector3 targetCamPos = m_target.position + m_offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, m_smooth * Time.deltaTime);
    }
}
