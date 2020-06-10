using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightController : MonoBehaviour
{
    public ThrottleLogic throttle;

    public StickLogic stick;

    private FlightModel flightModel;

    private void Awake()
    {
        flightModel = GetComponent<FlightModel>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnDisable()
    {
        flightModel.SetPitchRateNorm(0);
        flightModel.SetRollRateNorm(0);
        flightModel.SetForwardSpeedNorm(0);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStick();
        UpdateThrottle();
    }

    private void UpdateStick()
    {
        //Gets the setting of the stick
        float rollSetting = stick.CurrentRoll;
        float pitchSetting = stick.CurrentPitch;

        flightModel.SetPitchRateNorm(pitchSetting);
        flightModel.SetRollRateNorm(rollSetting);
    }

    private void UpdateThrottle()
    {
        float speedSetting = throttle.CurrentSetting;
        flightModel.SetForwardSpeedNorm(speedSetting);
    }
}
