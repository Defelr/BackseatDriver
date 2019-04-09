using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Task
{
    public Destination destination;
    public float fee;
}

public class Passenger : MonoBehaviour {

    public Task task;
    public GameManager gameManager;

	// Use this for initialization
	void Start () {
		if (!gameManager)
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            CarController car = other.GetComponent<CarController>();
            if (!car.passengerInCar)
            {
                // Offer a journey
                gameManager.OfferToCar(this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            CarController car = other.GetComponent<CarController>();
            if (!car.passengerInCar)
            {
                gameManager.StartCheckingDialogueValidility();
            }
        }
    }
}
