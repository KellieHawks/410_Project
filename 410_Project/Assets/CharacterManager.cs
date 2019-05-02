using System;
using UnityEngine;

[Serializable]
public class CharacterManager
{
    public Transform m_SpawnPoint;
    [HideInInspector] public int m_PlayerNumber;
    [HideInInspector] public GameObject m_Instance;
    [HideInInspector] public int m_Wins;


    private FollowerMovement m_Movement;
    private GameObject m_CanvasGameObject;


    public void Setup()
    {
        m_Movement = m_Instance.GetComponent<FollowerMovement>();

        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

    }

    public void DisableControl()
    {
        m_Movement.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }


    public void EnableControl()
    {
        m_Movement.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }


    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
