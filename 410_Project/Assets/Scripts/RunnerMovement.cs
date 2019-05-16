using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerMovement : MonoBehaviour
{
    private float m_moveSpeed = 26f;
    private float m_turnSpeed = 200f;

    private string m_TurnAxisName;
    private Rigidbody m_Rigidbody;

    private Animator m_animator;
    //private float m_MovementValue;
    private float m_TurnValue;

    //public Rigidbody follower;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>(); //using getcomponent_rigid body to store a reference
    }

    private void OnEnable()
    {
        //m_Rigidbody.isKinematic = false;
        //m_MovementValue = 0f;
        m_TurnValue = 0f;
    }

    //private void OnDisable()
    //{
    //    m_Rigidbody.isKinematic = true; //stops forces from moving it while it's invisible
    //}

    private void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    private void Update()
    {
        //m_MovementValue = 0f;
        m_TurnValue = 0f;
    }

    private void FixedUpdate()
    {
        Move();
        Turn();
    }

    private void Move()
    {

        Vector3 movement = transform.forward * 1f * m_moveSpeed * Time.deltaTime;

        //m_animator.SetFloat("MoveSpeed", 1f);

        m_Rigidbody.AddForce(movement * m_moveSpeed);
        m_Rigidbody.AddForce(Physics.gravity);
 
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);

    }

    private void Turn()
    {
        float turn = m_TurnValue * m_turnSpeed * Time.deltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Endpoint"))
        {
            //col.gameObject.SetActive(false);
            m_Rigidbody.gameObject.SetActive(false);
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.gameObject.tag.CompareTo("Player") == 0)
    //    {
    //        m_moveSpeed += 5;
    //    }
    //}

}