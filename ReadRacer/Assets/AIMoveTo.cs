using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMoveTo : MonoBehaviour {
    public GameObject[] Checkpoint;
    private List<Vector3> RacingPath;

    private AIController aiController;

    public int CheckpointMax = 16;

    private int NextCheckpoint = 0;

	// Use this for initialization
	void Start () {
        RacingPath = new List<Vector3>();

        aiController = GameObject.Find("Racer").GetComponent<AIController>();
	}

    // Add all Checkpoint positions to List RacingPath
    void FillRacingPath()
    {
        for (int i = 0; i < CheckpointMax; i++)
        {
            RacingPath.Add(Checkpoint[i].transform.position);
        }
    }

    void MoveToCheckPoint()
    {
        if (NextCheckpoint == 0)
        {

        }
    }
	
	// Update is called once per frame
	void Update () {
        MoveToCheckPoint();
        //aiController.MoveForward();
        //aiController.Steering();
	}
}
