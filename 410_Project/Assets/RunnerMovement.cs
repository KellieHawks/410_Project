using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerMovement : MonoBehaviour
{
    private float m_moveSpeed = 25f;
    private float m_turnSpeed = 180f;
    private float m_jumpForce = 300f;
    public float m_PitchRange = 0.2f;

    private string m_MovementAxisName;
    private string m_TurnAxisName;
    private Rigidbody m_Rigidbody;

    private Animator m_animator;
    private float m_MovementInputValue;
    private float m_TurnInputValue;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>(); //using getcomponent_rigid body to store a reference
    }

    private void OnEnable()
    {
        m_Rigidbody.isKinematic = false; //kinematic means no forces are applied
        m_MovementInputValue = 0f; //reset values to set up commencing driving
        m_TurnInputValue = 0f;
    }

    private void OnDisable()
    {
        m_Rigidbody.isKinematic = true; //stops forces from moving it while it's invisible
    }

    private void Start()
    {
        m_animator = GetComponent<Animator>();

        m_MovementAxisName = "Vertical"; //gets correct axes
        m_TurnAxisName = "Horizontal";
    }

    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.
        m_MovementInputValue = 0f; //Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = 0f; //Input.GetAxis(m_TurnAxisName);
    }

    private void FixedUpdate()
    {
        // Move and turn the tank.
        Move();
        Turn();
    }

    private void Move()
    {

        Vector3 movement = transform.forward * 1f * m_moveSpeed * Time.deltaTime;

        m_animator.SetFloat("MoveSpeed", 1f);

        m_Rigidbody.AddForce(movement * m_moveSpeed);
        m_Rigidbody.AddForce(Physics.gravity);
 
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);

    }

    private void Turn()
    {
        // Adjust the rotation of the character based on the player's input.
        float turn = m_TurnInputValue * m_turnSpeed * Time.deltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }

}