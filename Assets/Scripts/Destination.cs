using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour {

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
            if (car.passengerInCar)
            {
                gameManager.CheckForDestination(this);
            }
        }
    }
}
