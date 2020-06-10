using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationConstraints : MonoBehaviour
{
    public bool constrainLocalX;
    public bool constrainLocalY;
    public bool constrainLocalZ;

    public bool limitLocalX;
    public bool limitLocalY;
    public bool limitLocalZ;

    public float xMin;
    public float xMax;

    public float yMin;
    public float yMax;

    public float zMin;
    public float zMax;

    // Update is called once per frame
    void Update()
    {
        Vector3 localEuler = transform.localEulerAngles;

        float newX = Wrap(localEuler.x);
        float newY = Wrap(localEuler.y);
        float newZ = Wrap(localEuler.z);

        if (limitLocalX) newX = Mathf.Clamp(newX, xMin, xMax);
        if (limitLocalY) newY = Mathf.Clamp(newY, yMin, yMax);
        if (limitLocalZ) newZ = Mathf.Clamp(newZ, zMin, zMax);

        if (constrainLocalX) newX = 0;
        if (constrainLocalY) newY = 0;
        if (constrainLocalZ) newZ = 0;

        transform.localEulerAngles = new Vector3(newX, newY, newZ);

    }

    private float Wrap(float f)
    {
        if (f > 180) return f - 360;
        return f;
    }
}
