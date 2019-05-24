using UnityEngine;
using UnityEngine.Events;


public class ArrowController : MonoBehaviour
{

    public Transform target;
    public float speed = 5f;

    private void Update()
    {
        Vector3 direction = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, speed * Time.deltaTime);
    }
}