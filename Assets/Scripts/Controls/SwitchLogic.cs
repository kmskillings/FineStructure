using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwitchLogic : MonoBehaviour
{
    public Transform rocker;

    public HingeJoint joint;

    public HandContactTracker contactTracker;

    public bool isOn;

    public VibrationClip flipHapticsClip;

    public float turnOnRotation;
    public float turnOffRotation;

    public float onRotation;
    public float offRotation;

    public UnityEvent OnTurnOn;
    public UnityEvent OnTurnOff;
    
    // Start is called before the first frame update
    void Start()
    {
        SetJointTarget();
        (isOn ? OnTurnOn : OnTurnOff)?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isOn)
        {
            if(Wrap(rocker.localEulerAngles.x) > turnOnRotation)
            {
                //turn on the switch
                isOn = true;
                SetJointTarget();
                contactTracker.PlayOnTouchingHands(flipHapticsClip);
                OnTurnOn?.Invoke();
            }
        }

        if (isOn)
        {
            if(Wrap(rocker.localEulerAngles.x) < turnOffRotation)
            {
                //turn the switch off
                isOn = false;
                SetJointTarget();
                contactTracker.PlayOnTouchingHands(flipHapticsClip);
                OnTurnOff?.Invoke();
            }
        }
    }

    private void SetJointTarget()
    {
        JointSpring spr = joint.spring;
        spr.targetPosition = isOn ? onRotation : offRotation;
        joint.spring = spr;
    }

    private float Wrap(float f)
    {
        if (f > 180) return f - 360;
        return f;
    }
}
