using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandContactTracker : MonoBehaviour
{
    private int leftTouchingCount = 0;
    private int rightTouchingCount = 0;

    public bool IsLeftTouching
    {
        get
        {
            return leftTouchingCount > 0;
        }
    }

    public bool IsRightTouching
    {
        get
        {
            return rightTouchingCount > 0;
        }
    }

    public VibrationClip contactClip;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.layer == LayerMask.NameToLayer("HandLeft"))
        {
            leftTouchingCount++;
            if(leftTouchingCount == 1)
            {
                contactClip.BeginClip(OVRInput.Controller.LTouch);
            }
        }

        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("HandRight"))
        {
            rightTouchingCount++;
            if (rightTouchingCount == 1)
            {
                contactClip.BeginClip(OVRInput.Controller.RTouch);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("HandLeft"))
        {
            leftTouchingCount--;
            if(leftTouchingCount < 0)
            {
                leftTouchingCount = 0;
                Debug.LogWarning("leftTouchingCount of HandContactTracker attached to object " + gameObject.name + " is less than 0.");
            }
        }

        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("HandRight"))
        {
            rightTouchingCount--;
            if (rightTouchingCount < 0)
            {
                rightTouchingCount = 0;
                Debug.LogWarning("rightTouchingCount of HandContactTracker attached to object " + gameObject.name + " is less than 0.");
            }
        }
    }

    public void PlayOnTouchingHands(VibrationClip clip)
    {
        if (IsLeftTouching) clip.BeginClip(OVRInput.Controller.LTouch);
        if (IsRightTouching) clip.BeginClip(OVRInput.Controller.RTouch);
    }
}
