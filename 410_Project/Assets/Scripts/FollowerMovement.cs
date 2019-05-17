using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowerMovement : MonoBehaviour
{
    private float m_moveSpeed = 80f;
    private float m_turnSpeed = 180f;
    private float m_jumpForce = 300f;
    public AudioSource m_MovementAudio;
    //public AudioClip m_running;
    public AudioClip m_walking;
    public AudioClip m_none;
    public float m_PitchRange = 0.2f;

    private string m_MovementAxisName;
    private string m_TurnAxisName;
    private Rigidbody m_Rigidbody;
    private float m_MovementInputValue;
    private float m_TurnInputValue;
    private float m_OriginalPitch;

    private int numjumps;
    private bool m_isGrounded;
    private bool m_wasGrounded;
    private bool speed_powerup = false;
    private int counter = 0;

    //private bool game_over = false;

    private Animator m_animator;

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

        m_wasGrounded = false;

        m_MovementAxisName = "Vertical"; //gets correct axes
        m_TurnAxisName = "Horizontal";

        m_OriginalPitch = m_MovementAudio.pitch; //tanks remains the same pitch
    }

    private void Update() {

        m_animator.SetBool("Grounded", m_isGrounded);
        // Store the player's input and make sure the audio for the engine is playing.
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);
        WalkingAudio(); //calls function where engine sounds are dealt with


        if (speed_powerup == true)
        {
            counter++;
            if (counter > 20)
            {
                m_moveSpeed = 50f;
                counter = 0;
                speed_powerup = false;
            }
        }

    }

    private void WalkingAudio(){
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
        { //if movement in vertical plane or horizontal axis, you are moving 
            if (m_MovementAudio.clip == m_walking)
            {
                m_MovementAudio.clip = m_none;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            if (m_MovementAudio.clip == m_none)
            {
                m_MovementAudio.clip = m_walking;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }

    private void FixedUpdate()
    {
        // Move and turn the collower.
        Move();
        Turn();
    }

    private void Move()
    {

        if (!m_wasGrounded && m_isGrounded)
        {
            m_animator.SetTrigger("Land");
        }

        if (!m_isGrounded && m_wasGrounded)
        {
            m_animator.SetTrigger("Jump");
        }

        Vector3 movement = transform.forward * m_MovementInputValue * m_moveSpeed * Time.deltaTime;

        m_animator.SetFloat("MoveSpeed", m_MovementInputValue);

        //m_Rigidbody.AddForce(movement * m_moveSpeed);
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);

        if (Input.GetKeyDown("space") && numjumps < 1)
        {
            m_isGrounded = false;
            m_wasGrounded = true;

            numjumps++;
            Vector3 jump = new Vector3(0.0f, m_jumpForce, 0.0f);
            m_Rigidbody.AddForce(jump);
        }

        if (m_isGrounded == true)
        {
            numjumps = 0;
            m_wasGrounded = false;
        }

    }

    private void Turn()
    {
        // Adjust the rotation of the character based on the player's input.
        float turn = m_TurnInputValue * m_turnSpeed * Time.deltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            m_isGrounded = true;
        }
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Endpoint"))
        {
            //col.gameObject.SetActive(false);
            m_Rigidbody.gameObject.SetActive(false);
        }

        if (col.gameObject.CompareTag("pumpkin"))
        {
            col.gameObject.SetActive(false);
            m_moveSpeed = 150;
            speed_powerup = true;

            //setCountText(m_moveSpeed);
        }
        if (col.gameObject.CompareTag("apple"))
        {
            col.gameObject.SetActive(false);
            m_moveSpeed = 20;
            speed_powerup = true;

            //setCountText(m_moveSpeed);
        }
    }

    //void setCountText(float numb)
    //{
    //    if (numb > 50)
    //        m_MessageText.text = "You have a speed power up!";
    //    else
    //    {
    //        m_MessageText.text = "You have a speed power down!";
    //    }

    //}


    /*private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        { //when triggered, object is deactivated
            other.gameObject.SetActive(false);
            count++;
            setCountText();
        }
    }*/
}
