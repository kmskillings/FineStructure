using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlBarManager : MonoBehaviour
{
    public ControlBarMFD rollBar;
    public ControlBarMFD pitchBar;
    public ControlBarMFD throttleBar;

    public FlightModel flightModel;
    
    // Start is called before the first frame update
    void Start()
    {
        rollBar.min = -flightModel.maxRollRate;
        rollBar.max = flightModel.maxRollRate;
        rollBar.neutral = 0;
        rollBar.zero = 0;

        pitchBar.min = -flightModel.maxPitchRate;
        pitchBar.max = flightModel.maxPitchRate;
        pitchBar.neutral = 0;
        pitchBar.zero = 0;

        throttleBar.min = 0;
        throttleBar.max = flightModel.maxSpeed;
        throttleBar.neutral = flightModel.minSpeed;
        throttleBar.zero = 0;
    }

    // Update is called once per frame
    void Update()
    {
        rollBar.target = -flightModel.TargetRollRate;
        rollBar.current = -flightModel.RollRate;
        rollBar.Draw();

        pitchBar.target = flightModel.TargetPitchRate;
        pitchBar.current = flightModel.PitchRate;
        pitchBar.Draw();

        throttleBar.target = flightModel.TargetForwardSpeed;
        throttleBar.current = flightModel.ForwardSpeed;
        throttleBar.Draw();
    }
}
