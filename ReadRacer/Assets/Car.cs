using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Car : MonoBehaviour 
{
	public float topSpeed = 150;
    private float currentSpeed;
	public float maxReverseSpeed = -50;
	public float maxTurnAngle = 10;
	public float maxTorque = 10;
	public float decelerationTorque = 30;
	public Vector3 centerOfMassAdjustment = new Vector3(0f,-0.9f,0f);
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

    public float maxBreakTorque = 100;
    private bool applyHandbrake = false;

    public float handbrakeForwardSlip = 0.04f;
    public float handbrakeSidewaysSlip = 0.08f;

    public GameObject leftHeadlight;
    public GameObject rightHeadlight;

    public Texture2D Speedometer;
    public Texture2D Needle;

    public List<GameObject> Checkpoints;
    public List<GameObject> DummyChecks;

    private int LapNumber = 1;

    void Start()
    {
        //lower center of mass for roll-over resistance
        body = GetComponent<Rigidbody>();
        body.centerOfMass += centerOfMassAdjustment;

        DummyChecks = new List<GameObject>();
        SetDummyList();
    }
	
    // Sets dummylist to Checkpoint list for refill 
    private void SetDummyList()
    {
        for (int i = 0; i < Checkpoints.Count; i++)
        {
            DummyChecks.Add(Checkpoints[i]);
        }
    }

	// FixedUpdate is called once per physics frame
	void FixedUpdate () 
	{
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            leftHeadlight.SetActive(!leftHeadlight.activeInHierarchy);
            rightHeadlight.SetActive(!rightHeadlight.activeInHierarchy);
        }

        if (Input.GetKey(KeyCode.S) || (Input.GetKey(KeyCode.DownArrow) || Input.GetButton("Jump")))
        {
            brakeLights.enabled = true;
        }
        else
        {
            brakeLights.enabled = false;
        }

        if (Input.GetButton("Jump"))
        {
            applyHandbrake = true;
            wheelFL.brakeTorque = maxBreakTorque;
            wheelFR.brakeTorque = maxBreakTorque;
            if(GetComponent<Rigidbody>().velocity.magnitude > 1)
            {
                SetSlipValues(handbrakeForwardSlip, handbrakeSidewaysSlip);
            } else
            {
                SetSlipValues(1f, 1f);
            }
        }
        else
        {
            applyHandbrake = false;
            wheelFL.brakeTorque = 0;
            wheelFR.brakeTorque = 0;
            SetSlipValues(1f, 1f);
        }


        //calculate max speed in KM/H (optimized calc)
        currentSpeed = wheelBL.radius*wheelBL.rpm*Mathf.PI*0.12f;
		if(currentSpeed < topSpeed && currentSpeed > maxReverseSpeed)
		{
			//rear wheel drive.
			wheelBL.motorTorque = Input.GetAxis("Vertical") * maxTorque;
			wheelBR.motorTorque = Input.GetAxis("Vertical") * maxTorque;
		}
		else
		{
			//can't go faster, already at top speed that engine produces.
			wheelBL.motorTorque = 0;
			wheelBR.motorTorque = 0;
		}
		
		//Spoilers add down pressure based on the car’s speed. (Upside-down lift)
		Vector3 localVelocity = transform.InverseTransformDirection(body.velocity);
		body.AddForce(-transform.up * (localVelocity.z * spoilerRatio),ForceMode.Impulse);
		
		//front wheel steering
		wheelFL.steerAngle = Input.GetAxis("Horizontal") * maxTurnAngle;
		wheelFR.steerAngle = Input.GetAxis("Horizontal")* maxTurnAngle;
		
		//apply deceleration when not pressing the gas or when breaking in either direction.
		if (!applyHandbrake && ((Input.GetAxis("Vertical") <= -0.5f && localVelocity.z > 0)||(Input.GetAxis("Vertical") >= 0.5f && localVelocity.z < 0)))
		{
			wheelBL.brakeTorque = decelerationTorque + maxTorque;
			wheelBR.brakeTorque = decelerationTorque + maxTorque;
		}
		else if(Input.GetAxis("Vertical") == 0)
		{
			wheelBL.brakeTorque = decelerationTorque;
			wheelBR.brakeTorque = decelerationTorque;
		}
		else
		{
			wheelBL.brakeTorque = 0;
			wheelBR.brakeTorque = 0;
		}
	}

    // setter for what lap the player is on 
    public void SetLapNum(int Lap)
    {
        LapNumber = Lap;
    }

    // Getter for the lap the player is on
    public int GetLapNum()
    {
        return LapNumber;
    }
    //Removes 1 from list of checkpoints, if none left, call resetcheckpoints;
    public void RemoveCheckpoint()
    {
        Checkpoints.RemoveAt(0);
    }
    //Resets checkpoints list to dummy list that holds the whole list of checkpoints;
    public void ResetCheckpoints()
    {
        Checkpoints = DummyChecks;
    }

    void SetSlipValues(float forward, float sideways)
    {
        WheelFrictionCurve tempStruct = wheelBR.forwardFriction;
        tempStruct.stiffness = forward;
        wheelBR.forwardFriction = tempStruct;

        tempStruct = wheelBR.sidewaysFriction;
        tempStruct.stiffness = sideways;
        wheelBR.sidewaysFriction = tempStruct;

        tempStruct = wheelBL.forwardFriction;
        tempStruct.stiffness = forward;
        wheelBL.forwardFriction = tempStruct;

        tempStruct = wheelBL.sidewaysFriction;
        tempStruct.stiffness = sideways;
        wheelBL.sidewaysFriction = tempStruct;
    }
	
	void UpdateWheelPositions()
	{
		//move wheels based on their suspension.
		WheelHit contact = new WheelHit();
		if(wheelFL.GetGroundHit(out contact))
		{
			Vector3 temp = wheelFL.transform.position;
			temp.y = (contact.point + (wheelFL.transform.up*wheelFL.radius)).y;
			wheelTransformFL.position = temp;
		}
		if(wheelFR.GetGroundHit(out contact))
		{
			Vector3 temp = wheelFR.transform.position;
			temp.y = (contact.point + (wheelFR.transform.up*wheelFR.radius)).y;
		    wheelTransformFR.position = temp;
		}
		if(wheelBL.GetGroundHit(out contact))
		{
			Vector3 temp = wheelBL.transform.position;
			temp.y = (contact.point + (wheelBL.transform.up*wheelBL.radius)).y;
			wheelTransformBL.position = temp;
		}
		if(wheelBR.GetGroundHit(out contact))
		{
			Vector3 temp = wheelBR.transform.position;
			temp.y = (contact.point + (wheelBR.transform.up*wheelBR.radius)).y;
			wheelTransformBR.position = temp;
		}
	}
    void OnGUI()
    {
        GUI.DrawTexture(new Rect(Screen.width - 300, Screen.height - 150, 300, 150), Speedometer);
        float speedFactor = currentSpeed / topSpeed;
        float rotationAngle = Mathf.Lerp(0, 100, Mathf.Abs(speedFactor));
        GUIUtility.RotateAroundPivot(rotationAngle, new Vector2(Screen.width - 150, Screen.height));
        GUI.DrawTexture(new Rect(Screen.width - 300, Screen.height - 150, 300, 300), Needle);
    }

    void Update()
	{
		//rotate the wheels based on RPM
		float rotationThisFrame = 360*Time.deltaTime;
		wheelTransformFL.Rotate(wheelFL.rpm/rotationThisFrame,0,0);
		wheelTransformFR.Rotate(wheelFR.rpm/rotationThisFrame,0,0);
		wheelTransformBL.Rotate(wheelBL.rpm/rotationThisFrame,0,0);
		wheelTransformBR.Rotate(wheelBR.rpm/rotationThisFrame,0,0);

        wheelTransformFL.localEulerAngles = new Vector3(wheelTransformFL.localEulerAngles.x, wheelFL.steerAngle - wheelTransformFL.localEulerAngles.z, wheelTransformFL.localEulerAngles.z);
        wheelTransformFR.localEulerAngles = new Vector3(wheelTransformFR.localEulerAngles.x, wheelFR.steerAngle - wheelTransformFR.localEulerAngles.z, wheelTransformFR.localEulerAngles.z);

		UpdateWheelPositions();

        //SpedoMeter.ShowSpeed(body.velocity.magnitude, 0, 50);
	}
}
