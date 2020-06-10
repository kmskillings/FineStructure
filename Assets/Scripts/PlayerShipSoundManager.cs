using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipSoundManager : MonoBehaviour
{
    public FlightModel shipFlightModel;

    public AudioSource engineAudioSource;
    public float engineIdlePitch;       //The pitch is determined by how quickly the ship is flying.
    public float engineMaxPitch;
    public float engineIdleVolume;           //The volume is determined by how hard the engine is working.
    public float engineMaxVolume;

    public AudioSource negativeRollSource;
    public AudioSource positiveRollSource;
    public AudioSource negativePitchSource;
    public AudioSource positivePitchSource;
    //public float thrusterMinVolume;
    public float thrusterMaxVolume;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void MuteAll()
    {
        engineAudioSource.mute = true;
        negativePitchSource.mute = true;
        positivePitchSource.mute = true;
        negativeRollSource.mute = true;
        positiveRollSource.mute = true;
    }

    public void UnMuteAll()
    {
        engineAudioSource.mute = false;
        negativePitchSource.mute = false;
        positivePitchSource.mute = false;
        negativeRollSource.mute = false;
        positiveRollSource.mute = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEngineSound();
        UpdateThrusterSounds();
    }

    private void UpdateEngineSound()
    {
        //The speed of this ship determines how the pitch.
        //The speed is LerpUnNormalized from between the minspeed and maxspeed to between idlePitchShift and maxPitchShift
        engineAudioSource.pitch = LerpUnNormalized(shipFlightModel.ForwardSpeed, shipFlightModel.minSpeed, shipFlightModel.maxSpeed, engineIdlePitch, engineMaxPitch);

        //The engine response determines the volume.
        //The EngineThrust is LerpUnNormalized from between 0 and maxEngineThrust to between idleVolume and maxVolume if the engine is thrusting forward.
        //If it's thrusting backward, maxEngineReverseThrust is used instead.
        float lerpB = shipFlightModel.maxEngineThrust;
        float thrust = shipFlightModel.EngineThrust;
        if (thrust < 0)
        {
            lerpB = shipFlightModel.maxEngineReverseThrust;
            thrust = -thrust;
        }
        engineAudioSource.volume = LerpUnNormalized(thrust, 0, lerpB, engineIdleVolume, engineMaxVolume);
    }

    private void UpdateThrusterSounds()
    {
        UpdateThrusterAxis(negativeRollSource, positiveRollSource, shipFlightModel.maxRollRate, thrusterMaxVolume, shipFlightModel.RollRate);
        UpdateThrusterAxis(negativePitchSource, positivePitchSource, shipFlightModel.maxPitchRate, thrusterMaxVolume, shipFlightModel.PitchRate);
    }

    private void UpdateThrusterAxis(AudioSource negative, AudioSource positive, float maxRate, float maxVolume, float rate)
    {
        AudioSource active = positive;
        AudioSource inactive = negative;

        if (rate < 0)
        {
            rate = -rate;
            active = negative;
            inactive = positive;
        }

        active.volume = LerpUnNormalized(rate, 0, maxRate, 0, maxVolume);
        inactive.volume = 0;
    }

    private float LerpUnNormalized(float t, float a, float b, float x, float y)
    {
        return (Mathf.Clamp(t, a, b) - a) / (b - a) * (y - x) + x;
    }
}
