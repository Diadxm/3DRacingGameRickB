using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckPlayerPOS : MonoBehaviour {
    public List<GameObject> CarList;

    public List<GameObject> Checkpoints;

    private List<GameObject> DummyChecks;

    private List<int> EnemyLap;

    public Text LapNum;

    private Car Player;

    private CarController Racer;

    private int Position;

    public Text PositionNum;

    public int Lap = 1;

	// Use this for initialization
	void Start () {
        DummyChecks = Checkpoints;

        Player = GameObject.Find("Player Car").GetComponent<Car>();
        Racer = GameObject.Find("Racer").GetComponent<CarController>();
	}

    private void SetPosition()
    {
        PositionNum.text = Position.ToString();
    }

    private void FillCheckpoints()
    {
        Checkpoints = DummyChecks;
    }

    // On trigger remove first in list of checkpoints 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.RemoveCheckpoint();
            CheckPositionForCars();
            SetPosition();
            if (Player.Checkpoints.Count <= 1)
            {
                SetLapPlayer();
                Debug.Log("NextLap");
            }
            Debug.Log("Player Checkpoints" + Player.Checkpoints.Count);
        }

        if (other.CompareTag("Racer"))
        {
            other.gameObject.GetComponent<CarController>().RemoveCheckpoint();
            CheckPositionForCars();
            SetPosition();
            //Debug.Log("Racer Checkpoints" + Racer.Checkpoints.Count);
        }
    }

    //Checking position of user and racer
    private void CheckPositionForCars()
    {
        if (Player.Checkpoints.Count > Racer.Checkpoints.Count)
        {
            Position = 2;
        }
        else if (Player.Checkpoints.Count < Racer.Checkpoints.Count)
        {
            Position = 1;
        }
    }

    private void SetLapPlayer()
    {
        Player.SetLapNum(Lap + 1);
    }

    private void SetLap()
    {
        LapNum.text = Lap.ToString();
    }

    // Update is called once per frame
    void Update () {
        if (Player.Checkpoints.Count == 0)
        {
            Player.ResetCheckpoints();
        }

        if (Racer.Checkpoints.Count == 0)
        {
            Racer.SetLapNum(Racer.GetLapNum() + 1);
            Racer.ResetCheckpoints();
        }
	}
}
