using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowVelocityVector : MonoBehaviour
{
    public Rigidbody rigidbody;
    public float distance;
    public Vector3 defaultDirection;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = transform.InverseTransformDirection(rigidbody.velocity);
        if(velocity.magnitude > 0)
        {
            transform.localPosition = velocity.normalized * distance;
        }
        else
        {
            transform.localPosition = defaultDirection.normalized * distance;
        }
    }
}
