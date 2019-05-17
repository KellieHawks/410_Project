using System;
using UnityEngine;

[Serializable]
public class CharacterManager
{
    public Transform m_SpawnPoint;
    [HideInInspector] public GameObject m_Instance;
    [HideInInspector] public int m_Wins;

    private FollowerMovement m_Movement;

    public void Setup()
    {
        m_Movement = m_Instance.GetComponent<FollowerMovement>();

    }

    public void DisableControl()
    {
        m_Movement.enabled = false;
    }


    public void EnableControl()
    {
        m_Movement.enabled = true;
    }


    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
