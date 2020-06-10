using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightModel : MonoBehaviour
{
    //Controls the wing lift, how the ship's velocity vector changes as the ship pitches up and down
    public float wingLift;              //How strong the wing's lift is. A lower value makes it more drifty and smooth, a higher value makes it more responsive
    public Vector3 wingLiftVector;      //The direction wing lift is applied in (Up)

    //Controls the rudder lift, how the ship's velocity vector changes as the ship yaws left and right. Much lower than the wing lift, but still important.
    public float rudderLift;            //How strong the rudder's lift is. A lower value makes it more drifty and smooth, a higher value makes it more responsive
    public Vector3 rudderLiftVector;    //The direction the rudder lift is applied in (Left/Right)

    public float maxEngineThrust;          //The maximum force the engine can provide in the forward direction
    public float maxEngineReverseThrust;   //The maximum force the engine can provide in the reverse direction
    public float minSpeed;              //The minimum speed the ship can travel at
    public float maxSpeed;              //The maximum speed the ship can travel at
    public float speedResponseK;        //The ratio of force applied to the difference between the ship's current speed and target speed.
                                        //A higher number will make the throttle more responsive, but also jerkier.
                                        //Units of force per meter per second

    public float rollThrusterTorque;    //The maximum torque the roll thrusters can provide in either direction
    public float maxRollRate;           //The maximum roll rate that can be commanded, in degrees per second
    public float rollResponseK;         //The ration of torque applied to the difference between teh ship's current roll rate and target roll rate.
                                        //A higher number will make the stick more responsive, a lower number will make it smoother and driftier.
                                        //Units of torque per degree per second

    public float pitchThrusterTorque;   //The maximum torque the pitch thrusters can provide in either direction
    public float maxPitchRate;          //The maximum pitch rate than can be commanded, in degrees per second
    public float pitchResponseK;        //The ratio of torque applied to the difference between the ship's current pitch rate and target roll rate.
                                        //A higher number will make the stick more responsive, a lower number will make it smoother and driftier.
                                        //Units of torque per degree per second

    public float yawThrusterTorque;     //The maximum torque the yaw thrusters can provide in either direction
    public float yawResponseK;          //The ration of torque applied to the ship's current yaw (this is similar to the other axes, but the commanded yaw is always considered to be 0.)

    private Rigidbody rigidbody;

    public Vector3 pitchAxis;
    public Vector3 rollAxis;
    public Vector3 yawAxis;
    public Vector3 forwardDirection;

    public float RollRate { get; private set; }
    public float PitchRate { get; private set; }
    public float ForwardSpeed { get; private set; }
    public float EngineThrust { get; private set; }

    public float TargetRollRate { get; private set; }
    public float TargetPitchRate { get; private set; }
    public float TargetForwardSpeed { get; private set; }

    private bool acceptInput = true;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody.inertiaTensor = new Vector3(1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 velocity = transform.InverseTransformDirection(rigidbody.velocity);
        //Controls rotation thrusters
        Vector3 angularVelocity = transform.InverseTransformDirection(rigidbody.angularVelocity) * Mathf.Rad2Deg;

        RollRate = Vector3.Dot(rollAxis, angularVelocity);
        float rollResponse = PResponse(TargetRollRate, RollRate, rollResponseK, rollThrusterTorque);
        rigidbody.AddRelativeTorque(rollAxis * rollResponse);

        PitchRate = Vector3.Dot(pitchAxis, angularVelocity);
        float pitchResponse = PResponse(TargetPitchRate, PitchRate, pitchResponseK, pitchThrusterTorque);
        rigidbody.AddRelativeTorque(pitchAxis * pitchResponse);

        float yawRate = Vector3.Dot(yawAxis, angularVelocity);
        float yawResponse = PResponse(0, yawRate, yawResponseK, yawThrusterTorque);
        rigidbody.AddRelativeTorque(yawAxis * yawResponse);

        //Controls engine thrust
        ForwardSpeed = Vector3.Dot(forwardDirection, velocity);
        EngineThrust = PResponse(TargetForwardSpeed, ForwardSpeed, speedResponseK, maxEngineThrust, -maxEngineReverseThrust);
        rigidbody.AddRelativeForce(forwardDirection * EngineThrust);
        

        //Controls the lift of the wing and rudder
        Vector3 liftForce = -Vector3.Project(velocity, wingLiftVector) * wingLift;
        Vector3 rudderForce = -Vector3.Project(velocity, rudderLiftVector) * rudderLift;
        rigidbody.AddRelativeForce(liftForce + rudderForce);

    }

    //Sets the roll rate, in degrees per second. If this is outside the range -maxRollRate - maxRollRate, it is clamped.
    public void SetRollRate(float rollRate)
    {
        if (!acceptInput) return;
        TargetRollRate = Mathf.Clamp(rollRate, -maxRollRate, maxRollRate);
    }

    //Sets the pitch rate, in degrees per second. If this is outside the range -maxPitchRate - maxPitchRate, it is clamped.
    public void SetPitchRate(float pitchRate)
    {
        if (!acceptInput) return;
        TargetPitchRate = Mathf.Clamp(pitchRate, -maxPitchRate, maxPitchRate);
    }

    //Sets the roll rate. A value of 1 means positive maxRollRate, a value of -1 means negative maxRollRate. Any values outside this range are clamped.
    public void SetRollRateNorm(float rollRate)
    {
        SetRollRate(rollRate * maxRollRate);
    }

    //Sets the pitch rate. A value of 1 means positive maxPitchRate, a value of -1 means negative maxPitchRate. Any values outside this range are clamped.
    public void SetPitchRateNorm(float pitchRate)
    {
        SetPitchRate(pitchRate * maxPitchRate);
    }

    //Sets the speed of the ship, in meters per second. If it's outside the range minSpeed to maxSpeed, it is clamped.
    public void SetForwardSpeed(float speed)
    {
        if (!acceptInput) return;
        TargetForwardSpeed = Mathf.Clamp(speed, minSpeed, maxSpeed);
    }

    //Sets the speed of the ship. 0 means minSpeed and 1 means maxSpeed
    public void SetForwardSpeedNorm(float speed)
    {
        SetForwardSpeed(speed * (maxSpeed - minSpeed) + minSpeed);
    }

    public void ShutDownEngine()
    {
        SetRollRateNorm(0);
        SetPitchRateNorm(0);
        TargetForwardSpeed = 0;
        acceptInput = false;
    }

    public void ActivateEngine()
    {
        TargetForwardSpeed = minSpeed;
        acceptInput = true;
    }

    private float PResponse(float target, float current, float k, float maxResponse)
    {
        return PResponse(target, current, k, maxResponse, -maxResponse);
    }

    private float PResponse(float target, float current, float k, float maxResponse, float maxNegativeResponse)
    {
        return Mathf.Clamp(k * (target - current), maxNegativeResponse, maxResponse);
    }
}
