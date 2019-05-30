using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//move faster you increase the timescale 
//radius determines how far apart it moves 
//HAVE TO MOVE IT IN ACTUAL UNITY NOT HERE 
//for a new blob just have to attatch it to the Blobscript again. Don't add component
//new script just there in search bar type in Blobscript and it will pop up.

class Blobscript : MonoBehaviour
{
    Vector3 startingPosition;

    public Vector3 Direction = new Vector3(1, 0, 0);
    public float radius = 10f;
    public float timescale = 1f;

    void Start()
    {
        startingPosition = transform.position;
    }

    void FixedUpdate()
    {
        transform.position = startingPosition + radius * Mathf.Sin(timescale * Time.time) * Direction;
    }
}