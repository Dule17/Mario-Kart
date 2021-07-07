using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    WheelCollider[] wheelsColliders;//0,1 Back || 2,3 Front
    Transform[] tires;//0,1 Back || 2,3 Front
    TrailRenderer[] trail;
    public Rigidbody rigidbody;
    public Joystick joystick;
    public ButtonHandbrake joystick_handbrake;
    public float speed,MaxSpeed;
    public float MotorTorque;
    public float MaxSteerAngle;
    int[] gearMaxSpeed = { 20, 40, 70, 100, 120, 150 };
    public int gear;
    AudioSource audio;
    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
        rigidbody.centerOfMass = new Vector3(0, -0.1f, 0);

        audio = GetComponent<AudioSource>();

        if (SystemInfo.operatingSystem.Split(' ')[0] == "Windows"|| SystemInfo.operatingSystem.Split(' ')[0] == "Linux") GameObject.FindGameObjectsWithTag("Android")[0].active = false;
        else
        {
            joystick = FindObjectOfType<Joystick>();
            joystick_handbrake = FindObjectOfType<ButtonHandbrake>();
        }
        if (this.name.Split('_')[1] == "Bike")
        {
            wheelsColliders = new WheelCollider[2];
            tires = new Transform[2];
            trail = new TrailRenderer[2];
            GameObject colliders = gameObject.transform.Find("Wheels Colliders").gameObject;
            wheelsColliders[0] = colliders.transform.Find("Tire_R").gameObject.GetComponent<WheelCollider>();
            wheelsColliders[1] = colliders.transform.Find("Tire_F").gameObject.GetComponent<WheelCollider>();


            GameObject tire = gameObject.transform.Find("Tires").gameObject.transform.Find("root").gameObject;
            tires[0] = tire.transform.Find("Tire_R").gameObject.transform;
            tires[1] = tire.transform.Find("Tire_F").gameObject.transform;

            GameObject tr = gameObject.transform.Find("Tire Tracks").gameObject;
            trail[0] = tr.transform.Find("Tire_R").gameObject.GetComponent<TrailRenderer>();
            trail[1] = tr.transform.Find("Tire_F").gameObject.GetComponent<TrailRenderer>();


        }
        else
        {
            wheelsColliders = new WheelCollider[4];
            tires = new Transform[4];
            trail = new TrailRenderer[4];
            GameObject colliders = gameObject.transform.Find("Wheels Colliders").gameObject;
            wheelsColliders[0] = colliders.transform.Find("Tire_RR").gameObject.GetComponent<WheelCollider>();
            wheelsColliders[1] = colliders.transform.Find("Tire_RL").gameObject.GetComponent<WheelCollider>();
            wheelsColliders[2] = colliders.transform.Find("Tire_FR").gameObject.GetComponent<WheelCollider>();
            wheelsColliders[3] = colliders.transform.Find("Tire_FL").gameObject.GetComponent<WheelCollider>();

            GameObject tire = gameObject.transform.Find("Tires").gameObject.transform.Find("root").gameObject;
            tires[0] = tire.transform.Find("Tire_RR").gameObject.transform;
            tires[1] = tire.transform.Find("Tire_RL").gameObject.transform;
            tires[2] = tire.transform.Find("Tire_FR").gameObject.transform;
            tires[3] = tire.transform.Find("Tire_FL").gameObject.transform;

            GameObject tr = gameObject.transform.Find("Tire Tracks").gameObject;
            trail[0] = tr.transform.Find("Tire_RR").gameObject.GetComponent<TrailRenderer>();
            trail[1] = tr.transform.Find("Tire_RL").gameObject.GetComponent<TrailRenderer>();
            trail[2] = tr.transform.Find("Tire_FR").gameObject.GetComponent<TrailRenderer>();
            trail[3] = tr.transform.Find("Tire_FL").gameObject.GetComponent<TrailRenderer>();
        }

        for(int i=0;i<trail.Length;i++)
        {
            trail[i].material.color = Color.black;
        }
        TireTracks(false);

        ClientSend.startGameReceived();
    }


    void TireTracks(bool state)
    {
        for (int i = 0; i < trail.Length; i++)
        {
            trail[i].emitting = state;
        }
    }
    void Drive(float accer)
    {
        speed = Mathf.Round(rigidbody.velocity.magnitude * 3.6f);
        accer = Mathf.Clamp(accer, -1, 1);
        
        if (speed < MaxSpeed)
        {
            for (int i = 0; i < wheelsColliders.Length; i++)
                wheelsColliders[i].motorTorque = accer * MotorTorque;
        }
        else
        {
            for (int i = 0; i < wheelsColliders.Length ; i++)
                wheelsColliders[i].motorTorque = 0;
        }

        
    }

    void Steer(float steer)
    {
        steer = Mathf.Clamp(steer, -1, 1) * MaxSteerAngle;
        for (int i = wheelsColliders.Length / 2; i < wheelsColliders.Length; i++)
            wheelsColliders[i].steerAngle = steer;

    }
    void WheelRotate()
    {
        Vector3 pos;
        Quaternion quat;
        for(int i=0;i < wheelsColliders.Length; i++)
        {
            wheelsColliders[i].GetWorldPose(out pos,out  quat);
            tires[i].position = pos;
            tires[i].rotation = quat;
        }
    }


    float driftFactor;

    void Drift(float handBrake,float accer)
    {
        float driftSmothFactor = .7f * Time.deltaTime;

        if (Input.GetKey("space"))
        {
           WheelFrictionCurve sidewaysFriction = wheelsColliders[0].sidewaysFriction;
            WheelFrictionCurve forwardFriction = wheelsColliders[0].forwardFriction;
            TireTracks(true);
            float velocity = 0;
            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue =
                Mathf.SmoothDamp(forwardFriction.asymptoteValue, driftFactor * 2, ref velocity, driftSmothFactor);

            for (int i = 0; i < wheelsColliders.Length; i++)
            {
                wheelsColliders[i].sidewaysFriction = sidewaysFriction;
                wheelsColliders[i].forwardFriction = forwardFriction;
            }

            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue = 1.1f;
        
            for (int i = wheelsColliders.Length/2; i < wheelsColliders.Length; i++)
            {
                wheelsColliders[i].sidewaysFriction = sidewaysFriction;
                wheelsColliders[i].forwardFriction = forwardFriction;
            }
            rigidbody.AddForce(transform.forward * (speed / 400) * 40000);
        }
        else
        {
            TireTracks(false);
            WheelFrictionCurve forwardFriction = wheelsColliders[0].forwardFriction;
            WheelFrictionCurve sidewaysFriction = wheelsColliders[0].sidewaysFriction;

            forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue =
                ((speed * 2) / 300) + 1;

            for (int i = 0; i < wheelsColliders.Length; i++)
            {
                wheelsColliders[i].forwardFriction = forwardFriction;
                wheelsColliders[i].sidewaysFriction = sidewaysFriction;

            }
        }

        for (int i = 0; i < wheelsColliders.Length/2; i++)
        {

            WheelHit wheelHit;

            wheelsColliders[i].GetGroundHit(out wheelHit);

            if (wheelHit.sidewaysSlip < 0) driftFactor = (1 + -accer) * Mathf.Abs(wheelHit.sidewaysSlip);

            if (wheelHit.sidewaysSlip > 0) driftFactor = (1 + accer) * Mathf.Abs(wheelHit.sidewaysSlip);
        }


    }


    void EngineSound()
    {
        float minGearValue, maxGearValue;
        for (int i = 0; i < gearMaxSpeed.Length; i++)
            if (speed < gearMaxSpeed[i])
            {
                gear = i + 1;
                break;
            }

        if (gear == 1)
        {
            minGearValue = 0;
        }
        else
        {
            minGearValue = gearMaxSpeed[gear - 2];
        }
        maxGearValue = gearMaxSpeed[gear - 1];

        audio.pitch = ((speed - minGearValue) / (maxGearValue - minGearValue)) + 0.5f;
    }


    void FixedUpdate()
    {
        float accer, steer, handbrake;
        if (joystick != null)
        {

             accer = joystick.Vertical;
            steer = joystick.Horizontal;
           /*if(joystick_handbrake.pressed==true) handbrake =1.0f;
           else */handbrake = 0.0f;
        }
        else
        {
             accer = Input.GetAxis("Vertical");
             steer = Input.GetAxis("Horizontal");
             handbrake = Input.GetAxis("Jump");
        }
        if(GameController.StartGame) Drive(accer);
        Steer(steer);
        Drift(handbrake,accer);
        EngineSound();
        WheelRotate();
        ClientSend.playerMovementReceived();
    }


}
