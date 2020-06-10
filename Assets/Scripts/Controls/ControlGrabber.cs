using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlGrabber : MonoBehaviour
{
    public OVRInput.Controller controller;
    public OVRInput.Axis1D grabAxis;
    public float grabAxisThreshold;

    public FSHand hand;

    public Collider[] grabColliders;

    public Transform anchorTransform;

    List<GrabbableControl> grabCandidates = new List<GrabbableControl>();

    bool isGrabbing;
    GrabbableControl grabbed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Checks the grabAxis
        float grabFloat = OVRInput.Get(grabAxis, controller);
        if(grabFloat > grabAxisThreshold && !isGrabbing)
        {
            Grab();
        }
        if(grabFloat < grabAxisThreshold && isGrabbing)
        {
            UnGrab();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Checks other for a physicalControl layer
        if (other.gameObject.layer != LayerMask.NameToLayer("PhysicalControl")) return;
        
        //Checks for a GrabbableControl on the other.
        GrabbableControl control = other.attachedRigidbody.GetComponent<GrabbableControl>();
        if (control == null) return;

        //Adds the grabbable to the list
        grabCandidates.Insert(0, control);
    }

    private void OnTriggerExit(Collider other)
    {
        //Checks other for a physicalControl layer
        if (other.gameObject.layer != LayerMask.NameToLayer("PhysicalControl")) return;

        //Checks for a GrabbableControl on the other.
        GrabbableControl control = other.attachedRigidbody.GetComponent<GrabbableControl>();
        if (control == null) return;

        //Removes the GrabbableControl from the list
        grabCandidates.Remove(control);
    }

    private void Grab()
    {
        if (grabCandidates.Count == 0) return;

        isGrabbing = true;
        grabbed = grabCandidates[0];
        hand.SetGrabbedControl(grabbed);
        grabbed.Grab(this);
        grabCandidates.Clear();
        grabbed.RemoveHandHover(gameObject);

        foreach(Collider collider in grabColliders)
        {
            collider.enabled = false;
        }
    }

    private void UnGrab()
    {
        isGrabbing = false;

        hand.UnGrabControl();
        grabbed.UnGrab();
        grabCandidates.Clear();

        foreach(Collider collider in grabColliders)
        {
            collider.enabled = true;
        }
    }
}
