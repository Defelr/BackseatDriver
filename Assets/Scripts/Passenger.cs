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
    public bool isCompleted;
    public List<Sprite> potraits;
    [Range(0f, 100f)]
    public float contentLevel;

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
            if (!car.passengerInCar && !isCompleted)
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

    public Sprite GetPotrait()
    {
        if (0f <= contentLevel && contentLevel < 20f)
        {
            return potraits[0];
        }
        else if (contentLevel >= 20f && contentLevel < 40f)
        {
            return potraits[1];
        }
        else if (contentLevel >= 40f && contentLevel < 60f)
        {
            return potraits[2];
        }
        else if (contentLevel >= 60f && contentLevel < 80f)
        {
            return potraits[3];
        }
        else if (contentLevel >= 80f)
        {
            return potraits[4];
        }
        else
        {
            return null;
        }
    }

    public void UpdateContentLevel(float change)
    {
        contentLevel = Mathf.Max(0f, Mathf.Min(contentLevel + change, 100f));
    }

    public float GetContentLevel()
    {
        return contentLevel;
    }
}