using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkidEnabler : MonoBehaviour {

    public WheelCollider wheelCollider;
    public GameObject skidTrailRenderer;
    public float SkidLife = 2.5f;
    private TrailRenderer skidMark;


	// Use this for initialization
	void Start () {
        skidMark = skidTrailRenderer.GetComponent<TrailRenderer>();
        skidMark.time = SkidLife;

	}
	
	// Update is called once per frame
	void Update () {
		if (wheelCollider.forwardFriction.stiffness < 0.1 && wheelCollider.isGrounded)
        {
            if (skidMark.time == 0)
            {
                skidMark.time = SkidLife;
                skidTrailRenderer.transform.parent = wheelCollider.transform;
                skidTrailRenderer.transform.localPosition = wheelCollider.center + ((wheelCollider.radius - 0.1f) * -wheelCollider.transform.up);
            }

            if (skidTrailRenderer.transform.parent == null)
            {
                skidMark.time = 0;
            }
        } else
        {
            skidTrailRenderer.transform.parent = null;
        }
	}
}
