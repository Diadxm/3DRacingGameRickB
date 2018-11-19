using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {
    public float topSpeed = 150;
    private float currentSpeed;
    public float maxReverseSpeed = -50;
    public float maxTurnAngle = 10;
    public float maxTorque = 10;
    public float decelerationTorque = 30;
    public Vector3 centerOfMassAdjustment = new Vector3(0f, -0.9f, 0f);
    public float spoilerRatio = 0.1f;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelBL;
    public WheelCollider wheelBR;
    public Transform wheelTransformFL;
    public Transform wheelTransformFR;
    public Transform wheelTransformBL;
    public Transform wheelTransformBR;
    private Rigidbody body;
    public MeshRenderer brakeLights;

    private Vector3 LookRight;
    private Vector3 LookLeft;
    private Vector3 LookForward;

    public float MaxDistance;


    // Use this for initialization
    void Start () {
        //lower center of mass for roll-over resistance
        body = GetComponent<Rigidbody>();
        body.centerOfMass += centerOfMassAdjustment;

        LookForward = this.transform.TransformDirection(Vector3.forward);

        LookRight = (transform.forward + transform.right).normalized;

        LookLeft = (transform.forward - transform.right).normalized;

        MaxDistance = 50;

    }
	
    // Drives car forward
    public void MoveForward()
    {
        wheelBL.motorTorque = 3 * maxTorque;
        wheelBR.motorTorque = 3 * maxTorque;
    }

    //Handles turning Car
    public void TurnRight()
    {
        wheelFL.steerAngle = 5 * maxTurnAngle;
        wheelFR.steerAngle = 5 * maxTurnAngle;
    }

    public void TurnLeft()
    {
        wheelFL.steerAngle = -5 * maxTurnAngle;
        wheelFR.steerAngle = -5 * maxTurnAngle;
    }

    private void StopCar()
    {
        wheelBL.motorTorque = 0;
        wheelBR.motorTorque = 0;
    }

    private void SetWheel()
    {
        wheelFL.steerAngle = 0;
        wheelFR.steerAngle = 0;
    }

    //Raycasting to check for Checkpoint
    private void Lookahead()
    {
        RaycastHit Hit;
        RaycastHit Hit2;
        RaycastHit Hit3;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out Hit))
        {
            print(Hit.transform.name);
            if (Hit.collider.tag == "Checkpoint")
            {
                MoveForward();
            }
            else
            {
                StopCar();
            }
        }
        if (Physics.Raycast(transform.position, LookRight, out Hit2, MaxDistance))
        {
            //print(Hit2.transform.name);
            if (Hit2.collider.gameObject)
            {
                TurnRight();
            }
            else
            {
               SetWheel();
            }
        }

        if (Physics.Raycast(transform.position, LookLeft, out Hit3))
        {
            //print(Hit3.transform.name);
        }
    }

	// Update is called once per frame
	void Update () {
        Lookahead();
	}
}
