using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerMovement : MonoBehaviour{
    private float m_moveSpeed = 15f;
    private float m_turnSpeed = 180f;
    private float m_jumpForce = 300f;
    public AudioSource m_MovementAudio;
    public AudioClip m_running;
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

    private Animator m_animator;
    private readonly float m_interpolation = 10;
    private readonly float m_walkScale = 0.33f;
    private readonly float m_backwardsWalkScale = 0.16f;
    private readonly float m_backwardRunScale = 0.66f;
    private float m_currentV = 0;

    private void Awake(){
        m_Rigidbody = GetComponent<Rigidbody>(); //using getcomponent_rigid body to store a reference
    }

    private void OnEnable(){
        m_Rigidbody.isKinematic = false; //kinematic means no forces are applied
        m_MovementInputValue = 0f; //reset values to set up commencing driving
        m_TurnInputValue = 0f;
    }

    private void OnDisable(){
        m_Rigidbody.isKinematic = true; //stops forces from moving it while it's invisible
    }

    private void Start(){

        m_wasGrounded = false;

        m_MovementAxisName = "Vertical"; //gets correct axes
        m_TurnAxisName = "Horizontal";

        m_OriginalPitch = m_MovementAudio.pitch; //tanks remains the same pitch
    }

    private void Update(){

        m_animator = GetComponent<Animator>();

        m_animator.SetBool("Grounded", m_isGrounded);

        // Store the player's input and make sure the audio for the engine is playing.
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        WalkingAudio(); //calls function where engine sounds are dealt with
    }

    private void WalkingAudio(){
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.

        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f) { //if movement in vertical plane or horizontal axis, you are moving 
            if (m_MovementAudio.clip == m_walking){
                m_MovementAudio.clip = m_none;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else{
            if (m_MovementAudio.clip == m_none){
                m_MovementAudio.clip = m_walking;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }

    private void FixedUpdate(){
        // Move and turn the tank.
        Move();
        Turn();
    }

    private void Move(){
        // Adjust the position of the tank based on the player's input.


        float v = Input.GetAxis("Vertical");
        bool walk = Input.GetKey(KeyCode.LeftShift);
        if (v < 0){
            if (walk) { v *= m_backwardsWalkScale; }
            else { v *= m_backwardRunScale; }
        }
        else if (walk){
            v *= m_walkScale;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);


        if (!m_wasGrounded && m_isGrounded){
            m_animator.SetTrigger("Land");
        }

        if (!m_isGrounded && m_wasGrounded){
            m_animator.SetTrigger("Jump");
        }

        Vector3 movement = transform.forward * m_MovementInputValue * m_moveSpeed * Time.deltaTime;

        m_animator.SetFloat("MoveSpeed", m_currentV);

        m_Rigidbody.AddForce(movement * m_moveSpeed);
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);

        if (Input.GetKeyDown("space") && numjumps < 2){

            m_isGrounded = false;
            m_wasGrounded = true;

            numjumps++;
            Vector3 jump = new Vector3(0.0f, m_jumpForce, 0.0f);
            m_Rigidbody.AddForce(jump);

        }

        if (m_isGrounded == true){
            numjumps = 0;
            m_wasGrounded = false;
        }

    }

    private void Turn(){
        // Adjust the rotation of the tank based on the player's input.
        float turn = m_TurnInputValue * m_turnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }

    void OnCollisionEnter(Collision col){
        if (col.gameObject.CompareTag("Ground")){
            m_isGrounded = true;
        }
    }
}
