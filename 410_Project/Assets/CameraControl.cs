using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;         //the approximate time you want the camera take for it to get into position
    public float m_ScreenEdgeBuffer = 4f;   //number that we add to make sure tanks are fully on screen
    public float m_MinSize = 6.5f;          //a minimum size so isn't too zoomed in
    /*[HideInInspector]*/ public Transform[] m_Targets;

    private Camera m_Camera;                //instance of camera
    private float m_ZoomSpeed;              //reference to how fast camera is being damped
    private Vector3 m_MoveVelocity;
    private Vector3 m_DesiredPosition;      //position it is trying to reach


    private void Awake()                       //finds first camera and child object
    {
        m_Camera = GetComponentInChildren<Camera>();
    }


    private void FixedUpdate()      //moves camera
    {
        Move();
        Zoom();
    }


    private void Move() 
    {
        FindAveragePosition(); //find average position

        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime); //moves camera between desired position and current position
    }


    private void FindAveragePosition()
    {
        //Rewrite because we only need the camera to follow one character

        Vector3 averagePos = new Vector3(); //creates blank vector3
        int numTargets = 0;                 //number of targets we are averaging over

        if (!m_Targets[0].gameObject.activeSelf)    //for each active gameobject...
            continue;

            averagePos += m_Targets[0].position;
            numTargets++;
        }

        if (numTargets > 0)
            averagePos /= numTargets;   //for every target, find average position

        averagePos.y = transform.position.y;    //set average position

        m_DesiredPosition = averagePos; //returns the average position

    }


    private void Zoom() //find size and damp to that size
    {
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    private float FindRequiredSize()
    {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition); //find desired position in the camera rigs local space

        float size = 0f;

        for (int i = 0; i < m_Targets.Length; i++) //loop through and...
        {
            if (!m_Targets[i].gameObject.activeSelf)//for all the active gameobjects
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position); //find position in camera rigs local space

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos; //find desired position

            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y)); //calculate size

            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / m_Camera.aspect); //calculate size
        }

        size += m_ScreenEdgeBuffer; //increment screen edge buffer

        size = Mathf.Max(size, m_MinSize);

        return size; //return size
    }


    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}