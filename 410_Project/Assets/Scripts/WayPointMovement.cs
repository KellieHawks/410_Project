using UnityEngine;

public class WayPointMovement : MonoBehaviour
{
    public Transform[] wayPointList;

    public int currentWayPoint = 0;
    Transform targetWayPoint;

    public float speed = 4f;
    

    void Start()
    {
    }

    void Update()
    {
        // check if the cat has somewhere to move
        if (currentWayPoint < this.wayPointList.Length)
        {
            if (targetWayPoint == null)
                targetWayPoint = wayPointList[currentWayPoint];
            Move();
        }
    }

    void Move()
    {
        transform.forward = Vector3.RotateTowards(transform.forward, targetWayPoint.position - transform.position, speed * Time.deltaTime, 0.0f);
        transform.position = Vector3.MoveTowards(transform.position, targetWayPoint.position, speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if ("WayPoint".CompareTo(collision.gameObject.tag) == 0)
        {
            collision.gameObject.SetActive(false);
            currentWayPoint++;
            targetWayPoint = wayPointList[currentWayPoint];
        }
    }
}