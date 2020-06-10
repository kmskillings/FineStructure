using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GrabbableControl : MonoBehaviour
{
    public Transform handFollowTransform;
    public ControlGrabber grabber;
    public int grabPose;

    public VibrationClip hoverClip;

    public UnityEvent OnGrab;
    public UnityEvent OnUnGrab;

    int leftHoveringCounter = 0;
    int rightHoveringCounter = 0;

    private void OnTriggerEnter(Collider other)
    {
        AddHandHover(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        RemoveHandHover(other.gameObject);
    }

    public void Grab(ControlGrabber grabber)
    {
        this.grabber = grabber;
        OnGrab?.Invoke();
    }

    public void UnGrab()
    {
        grabber = null;
        OnUnGrab?.Invoke();
    }

    public void AddHandHover(GameObject go)
    {
        if (go.layer == LayerMask.NameToLayer("HandLeft"))
        {
            leftHoveringCounter++;
            if (leftHoveringCounter == 1)
            {
                hoverClip.BeginClip(OVRInput.Controller.LTouch);
            }
        }
        if (go.layer == LayerMask.NameToLayer("HandRight"))
        {
            rightHoveringCounter++;
            Debug.Log("Collider entered. Counter is now " + rightHoveringCounter);
            if (rightHoveringCounter == 1)
            {
                hoverClip.BeginClip(OVRInput.Controller.RTouch);
            }
        }
    }

    public void RemoveHandHover(GameObject go)
    {
        if (go.layer == LayerMask.NameToLayer("HandLeft"))
        {
            leftHoveringCounter--;
        }

        if (go.layer == LayerMask.NameToLayer("HandRight"))
        {
            rightHoveringCounter--;
            Debug.Log("Collider exited. Counter is now " + rightHoveringCounter);
        }
    }
}