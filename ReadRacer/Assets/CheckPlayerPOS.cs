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


    private void SetLap()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.RemoveCheckpoint();
        }

        if (other.CompareTag("Racer"))
        {
            other.gameObject.GetComponent<CarController>().RemoveCheckpoint();
        }
    }

    private void CheckPositionForCars()
    {
        for (int i = 0; i < CarList.Count; i++)
        {
           EnemyLap[i] = CarList[i].GetComponent<CarController>().GetLapNum();
        }

        for (int i = 0; i < EnemyLap.Count; i++)
        {
            if (Player.GetLapNum() > EnemyLap[i])
            {
                Position = 1;
            }
        }
        if (Player.Checkpoints.Count > Racer.Checkpoints.Count)
        {
            Position = 1;
        }
        else if (Player.Checkpoints.Count < Racer.Checkpoints.Count)
        {
            Position = 2;
        }
    }


    // Update is called once per frame
    void Update () {
        if (Player.Checkpoints.Count == 1)
        {
            Player.ResetCheckpoints();
            Player.SetLapNum(Player.GetLapNum() + 1);
        }

        if (Racer.Checkpoints.Count == 1)
        {
            Racer.ResetCheckpoints();
            Racer.SetLapNum(Racer.GetLapNum() + 1);
        }
        CheckPositionForCars();
        SetPosition();
	}
}
