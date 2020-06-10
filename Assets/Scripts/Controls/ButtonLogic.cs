using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonLogic : MonoBehaviour
{
    public Transform plunger;

    public HandContactTracker contactTracker;

    public VibrationClip pressHapticsClip;
    public VibrationClip releaseHapticsClip;

    public float pressDepth;
    public float releaseDepth;
    private float startDepth;
    private bool isPressed = false;

    public UnityEvent OnPress;
    public UnityEvent OnRelease;

    private void Awake()
    {
        startDepth = plunger.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isPressed)
        {
            if(plunger.localPosition.y < pressDepth)
            {
                isPressed = true;
                contactTracker.PlayOnTouchingHands(pressHapticsClip);
                OnPress?.Invoke();
            }
        }

        if (isPressed)
        {
            if(plunger.localPosition.y > releaseDepth)
            {
                isPressed = false;
                contactTracker.PlayOnTouchingHands(releaseHapticsClip);
                OnRelease?.Invoke();
            }
        }
    }
}