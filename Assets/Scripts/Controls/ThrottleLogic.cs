using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrottleLogic : MonoBehaviour
{
    private GrabbableControl control;
    private bool isGrabbed;
    public Transform movingPartTransform;
    private Transform handAnchorTransform;
    private Vector3 initialHandAnchorPosition;  //Where the hand was when it started grabbing the throttle, as measured in the throttle's local space.
    private Vector3 initialThrottlePosition;    //The position of the throttle was when the hand started grabbing the throttle

    public float minTravel;
    public float maxTravel;
    public float sensitivity;

    public float hapticFrequency;
    public float hapticMaxAmplitude;

    public float CurrentSetting
    {
        get
        {
            return (movingPartTransform.localPosition.z - minTravel) / (maxTravel - minTravel);
        }
    }

    private void Awake()
    {
        control = GetComponent<GrabbableControl>();
    }

    private void Update()
    {
        if (isGrabbed)
        {
            //The amount forward or backward the hand anchor has moved from where it was when in started grabbing
            float dZ = transform.InverseTransformPoint(handAnchorTransform.position).z - initialHandAnchorPosition.z;
            //This is the amount forward or back the moving part of the throttle should also move
            float newZ = initialThrottlePosition.z + dZ * sensitivity;
            newZ = Mathf.Clamp(newZ, minTravel, maxTravel);
            movingPartTransform.localPosition = new Vector3(initialThrottlePosition.x, initialThrottlePosition.y, newZ);

            OVRInput.Controller controller = OVRInput.Controller.LTouch;
            if (control.grabber.gameObject.layer == LayerMask.NameToLayer("HandRight"))
            {
                controller = OVRInput.Controller.RTouch;
            }
            OVRInput.SetControllerVibration(hapticFrequency, hapticMaxAmplitude * CurrentSetting, controller);
        }
    }

    public void Grab()
    {
        isGrabbed = true;
        handAnchorTransform = control.grabber.anchorTransform;
        initialHandAnchorPosition = transform.InverseTransformPoint(handAnchorTransform.position);
        initialThrottlePosition = movingPartTransform.localPosition;
    }

    public void UnGrab()
    {
        isGrabbed = false;
    }
}
