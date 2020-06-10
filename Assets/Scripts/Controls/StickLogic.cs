using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickLogic : MonoBehaviour
{
    public Transform pivot;

    private GrabbableControl control;
    private bool isGrabbed;
    private Transform handAnchorTransform;
    private Vector3 initialHandAnchorPosition;  //As measured in the StickLogic's local space

    public float maxRollDeflection;
    public float maxPitchDeflection;
    public float sensitivity;

    public float hapticFrequency;
    public float hapticMaxAmplitude;

    public float CurrentRoll
    {
        get
        {
            //Debug.Log(WrapAngle(pivot.localEulerAngles.z) / maxRollDeflection);
            return WrapAngle(pivot.localEulerAngles.z) / maxRollDeflection; 
        }
    }

    public float CurrentPitch
    {
        get
        {
            //return pivot.localEulerAngles.x / maxPitchDeflection;
            return WrapAngle(pivot.localEulerAngles.x) / maxPitchDeflection;
        }
    }

    public float CurrentTrigger
    {
        get
        {
            if (!isGrabbed) return 0;

            OVRInput.Controller grabbingController = OVRInput.Controller.Touch;
            if (control.grabber.gameObject.layer == LayerMask.NameToLayer("HandRight"))
            {
                grabbingController = OVRInput.Controller.RTouch;
            }
            else if (control.grabber.gameObject.layer == LayerMask.NameToLayer("HandLeft"))
            {
                grabbingController = OVRInput.Controller.LTouch;
            }
            return OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, grabbingController);
        }
    }

    private void Awake()
    {
        control = GetComponent<GrabbableControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrabbed)
        {
            //Gets the position of the handAnchorTransform in local space (to the base Stick's transform, not the pivot)
            Vector3 handAnchorLocalPos = transform.InverseTransformPoint(handAnchorTransform.position) - initialHandAnchorPosition;
            handAnchorLocalPos.y = 1 / sensitivity;

            float pitchAngle = Mathf.Atan2(handAnchorLocalPos.z, handAnchorLocalPos.y) * Mathf.Rad2Deg;
            float rollAngle = -Mathf.Atan2(handAnchorLocalPos.x, handAnchorLocalPos.y) * Mathf.Rad2Deg;

            pitchAngle = WrapAngle(pitchAngle);
            rollAngle = WrapAngle(rollAngle);

            pitchAngle = Mathf.Clamp(pitchAngle, -maxPitchDeflection, maxPitchDeflection);
            rollAngle = Mathf.Clamp(rollAngle, -maxRollDeflection, maxRollDeflection);

            Vector3 newEuler = new Vector3(pitchAngle, 0, rollAngle);

            pivot.localEulerAngles = newEuler;

            OVRInput.Controller grabbingController = OVRInput.Controller.Touch;
            if (control.grabber.gameObject.layer == LayerMask.NameToLayer("HandRight"))
            {
                grabbingController = OVRInput.Controller.RTouch;
            }
            else if (control.grabber.gameObject.layer == LayerMask.NameToLayer("HandLeft"))
            {
                grabbingController = OVRInput.Controller.LTouch;
            }

            Vector2 d = new Vector2(CurrentPitch, CurrentRoll);
            OVRInput.SetControllerVibration(hapticFrequency, d.magnitude * hapticMaxAmplitude, grabbingController);
        }
    }

    public void Grab()
    {
        isGrabbed = true;
        handAnchorTransform = control.grabber.anchorTransform;
        initialHandAnchorPosition = transform.InverseTransformPoint(handAnchorTransform.position);
    }

    public void UnGrab()
    {
        isGrabbed = false;
        pivot.localEulerAngles = Vector3.zero;
    }

    private float WrapAngle(float a)
    {
        while(a > 180)
        {
            a -= 360;
        }
        while(a < -180)
        {
            a += 360;
        }
        return a;
    }
}
