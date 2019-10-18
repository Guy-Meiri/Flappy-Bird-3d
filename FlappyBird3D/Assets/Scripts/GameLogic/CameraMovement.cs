using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Player;

    [SerializeField]
    private Vector3 m_CameraLocationOffset; //we initialize this within Unity to be new Vector3(0,1,4)
    
    void Update()
    {
        transform.position = m_Player.transform.position + m_CameraLocationOffset;
    }
}
