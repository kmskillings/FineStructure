using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyConstraints : MonoBehaviour
{
    Rigidbody body;

    public bool constrainLocalX;
    public bool constrainLocalY;
    public bool constrainLocalZ;

    public bool limitLocalX;
    public bool limitLocalY;
    public bool limitLocalZ;

    public Vector3 limitsMin;
    public Vector3 limitsMax;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 localPos = transform.localPosition;
        Vector3 localVelocity = transform.InverseTransformDirection(body.velocity);

        float newX = localPos.x;
        float newY = localPos.y;
        float newZ = localPos.z;

        float newVelX = localVelocity.x;
        float newVelY = localVelocity.y;
        float newVelZ = localVelocity.z;

        if (limitLocalX && !constrainLocalX)
        {
            newX = Mathf.Clamp(newX, limitsMin.x, limitsMax.x);
        }
        if (limitLocalY && !constrainLocalY)
        {
            newY = Mathf.Clamp(newY, limitsMin.y, limitsMax.y);
        }
        if (limitLocalZ && !constrainLocalZ)
        {
            newZ = Mathf.Clamp(newZ, limitsMin.z, limitsMax.z);
        }

        if (constrainLocalX)
        {
            newX = 0;
            newVelX = 0;
        }
        if (constrainLocalY)
        {
            newY = 0;
            newVelY = 0;
        }
        if (constrainLocalZ)
        {
            newZ = 0;
            newVelZ = 0;
        }

        transform.localPosition = new Vector3(newX, newY, newZ);
        body.velocity = transform.TransformDirection(new Vector3(newVelX, newVelY, newVelZ));
    }
}
