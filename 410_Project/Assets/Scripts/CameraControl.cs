//using UnityEngine;

//public class CameraControl : MonoBehaviour
//{
//    public float m_DampTime = 0.2f;                 // Approximate time for the camera to refocus.
//    public float m_ScreenEdgeBuffer = 4f;           // Space between the top/bottom most target and the screen edge.
using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{

    public Transform[] m_Targets;
    private Vector3 offset;         //Private variable to store the offset distance between the player and camera
    private Camera m_Camera;
    // Use this for initialization

    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }


    void Start()
    {
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - m_Targets[0].transform.position;
    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        transform.position = m_Targets[0].transform.position + offset;
    }
}