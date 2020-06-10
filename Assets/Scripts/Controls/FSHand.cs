using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSHand : MonoBehaviour
{
    public OVRInput.Controller controller;

    public Transform handAnchor;

    public Animator animator;
    private int layerPoint;
    private int layerThumb;

    public Collider pointerCollider;
    public Collider thumbCollider;

    private List<Renderer> hiddenWhenFocusLost = new List<Renderer>();  //All the renderers that were hidden when focus was lost. Should be unhidden when focus is regained.

    private bool isGrabbing = false;
    private GrabbableControl heldControl;
    private Transform followControlTransform;

    private bool isPointing = false;
    private bool isThumbUp = false;

    // Start is called before the first frame update
    void Start()
    {
        OVRManager.InputFocusLost += OnInputFocusLost;
        OVRManager.InputFocusAcquired += OnInputFocusAcquired;

        layerPoint = animator.GetLayerIndex("Point Layer");
        layerThumb = animator.GetLayerIndex("Thumb Layer");
    }

    // Update is called once per frame
    void Update()
    {   
        //Follows the anchor, if it's not already the parent and not already holding a control
        if(!isGrabbing && transform.parent != handAnchor)
        {
            transform.position = handAnchor.position;
            transform.rotation = handAnchor.rotation;
        }

        //Updates animations
        //if holding a control, don't do any of this.
        if (isGrabbing)
        {
            //Follow the control's transform
            transform.position = followControlTransform.position;
            transform.rotation = followControlTransform.rotation;
            
            return;
        }

        //Hand flex
        float flex = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controller);
        animator.SetFloat("Flex", flex);

        //Thumb/Index pinching
        float pinch = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller);
        animator.SetFloat("Pinch", pinch);

        //Pointing
        isPointing = !OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, controller);
        float point = isPointing ? 1f : 0f;
        animator.SetLayerWeight(layerPoint, point);

        //Thumb
        isThumbUp = !OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, controller);
        float thumb = isThumbUp ? 1f : 0f;
        animator.SetLayerWeight(layerThumb, thumb);

        //Sorts out colliders for the finger and thumb
        pointerCollider.enabled = isPointing;
        thumbCollider.enabled = isThumbUp;
    }

    private void OnInputFocusLost()
    {
        //Makes sure all the renderers are hidden. Keeps track of which ones were disabled, to be reenabled later.
        hiddenWhenFocusLost.Clear();
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            if (r.enabled)
            {
                hiddenWhenFocusLost.Add(r);
                r.enabled = false;
            }
        }
    }

    private void OnInputFocusAcquired()
    {
        //Reenables all the ones that were hidden when focus was lost.
        foreach(Renderer r in hiddenWhenFocusLost)
        {
            r.enabled = true;
        }
    }

    public void SetGrabbedControl(GrabbableControl control)
    {
        isGrabbing = true;
        heldControl = control;
        followControlTransform = control.handFollowTransform;

        //Takes care of animations
        animator.SetInteger("Pose", heldControl.grabPose);
        animator.SetFloat("Pinch", 0);
        animator.SetFloat("Flex", 0);
        animator.SetLayerWeight(layerThumb, 0);
        animator.SetLayerWeight(layerPoint, 0);
    }

    public void UnGrabControl()
    {
        isGrabbing = false;
        heldControl = null;
        followControlTransform = null;
        animator.SetInteger("Pose", 0);
    }
}
