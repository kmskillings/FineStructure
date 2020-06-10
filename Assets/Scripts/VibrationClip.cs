using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationClip : MonoBehaviour
{
    public float frequency;
    public float intensity;
    public float duration;

    private bool playingOnLeft = false;
    private float cooldownOnLeft = 0f;

    private bool playingOnRight = false;
    private float cooldownOnRight = 0f;

    // Update is called once per frame
    void Update()
    {
        if (cooldownOnLeft > 0)
        {
            cooldownOnLeft -= Time.deltaTime;
        }
        else if (playingOnLeft)
        {
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
            playingOnLeft = false;
        }

        if (cooldownOnRight > 0)
        {
            cooldownOnRight -= Time.deltaTime;
        }
        else if (playingOnRight)
        {
            OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
            playingOnRight = false;
        }
    }

    public void BeginClip(OVRInput.Controller controller)
    {
        if(controller == OVRInput.Controller.LTouch)
        {
            cooldownOnLeft = duration;
            playingOnLeft = true;
            OVRInput.SetControllerVibration(frequency, intensity, OVRInput.Controller.LTouch);
        }
        else if (controller == OVRInput.Controller.RTouch)
        {
            cooldownOnRight = duration;
            playingOnRight = true;
            OVRInput.SetControllerVibration(frequency, intensity, OVRInput.Controller.RTouch);
        }
    }
}
